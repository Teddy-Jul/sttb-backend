using Microsoft.EntityFrameworkCore;
using Serilog;
using sttbproject.Commons.Extensions;
using sttbproject.entities;
using sttbproject.HostedServices;

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

// Application Services (MediatR, Validators, Infrastructure) - Pass configuration
builder.Services.AddApplicationServices(builder.Configuration);

// Hosted Services
builder.Services.AddHostedService<DatabaseMigrationService>();

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
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting CMS Web API - Swagger available at /swagger");
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

