using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using sttbproject.Commons.Extensions;
using sttbproject.entities;
using sttbproject.HostedServices;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/sttbproject-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "STTB CMS API",
        Version = "v1",
        Description = "Content Management System API for Sekolah Tinggi Teologi Bethel",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "STTB Development Team",
            Email = "dev@sttb.ac.id"
        }
    });
    
    // Enable file upload in Swagger
    options.MapType<IFormFile>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});

// Database
builder.Services.AddDbContext<SttbprojectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Application Services (MediatR, Validators, Infrastructure)
builder.Services.AddApplicationServices(builder.Configuration);

// Hosted Services
builder.Services.AddHostedService<DatabaseMigrationService>();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    // Global rate limit policy
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:GeneralLimit", 100),
                QueueLimit = 0,
                Window = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("RateLimiting:WindowSeconds", 60))
            }));

    // Policy for general API endpoints
    options.AddPolicy("general", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100, // 100 requests per minute
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));

    // Policy for media upload (stricter)
    options.AddPolicy("upload", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10, // 10 uploads per minute
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));

    // Policy for authentication endpoints (very strict)
    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new SlidingWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 5, // Only 5 login attempts per minute
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 2
            }));

    // Policy for database write operations (create/update/delete)
    options.AddPolicy("write", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 50, // 50 writes per minute
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));

    // Custom response when rate limit is exceeded
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429; // Too Many Requests
        
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = $"Too many requests. Please try again after {retryAfter.TotalSeconds} seconds.",
                retryAfter = retryAfter.TotalSeconds
            }, cancellationToken: cancellationToken);
        }
        else
        {
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = "Too many requests. Please slow down and try again later."
            }, cancellationToken: cancellationToken);
        }
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "STTB CMS API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "STTB CMS API Documentation";
    });
    app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors();

// IMPORTANT: Add rate limiter middleware
app.UseRateLimiter();

app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting CMS Web API with Rate Limiting - Swagger available at /swagger");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

