using Amazon.S3;
using Amazon.Runtime;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using sttbproject.Commons.Configuration;
using sttbproject.Commons.Services;

namespace sttbproject.Commons.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        // Cloudflare R2 Configuration
        services.Configure<CloudflareR2Options>(options => 
            configuration.GetSection(CloudflareR2Options.SectionName).Bind(options));

        // AWS S3 Client for R2
        var r2Section = configuration.GetSection(CloudflareR2Options.SectionName);
        var r2Options = new CloudflareR2Options();
        r2Section.Bind(r2Options);

        if (!string.IsNullOrEmpty(r2Options.AccessKeyId))
        {
            var credentials = new BasicAWSCredentials(r2Options.AccessKeyId, r2Options.SecretAccessKey);
            var s3Config = new AmazonS3Config
            {
                ServiceURL = r2Options.GetEndpoint(),
                ForcePathStyle = true,
                UseHttp = false
            };
            
            services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(credentials, s3Config));
        }

        // File Storage Service - Choose based on configuration
        var storageProvider = configuration["FileStorage:Provider"] ?? "Local";
        
        if (storageProvider.Equals("CloudflareR2", StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IFileStorageService, CloudflareR2StorageService>();
        }
        else
        {
            services.AddScoped<IFileStorageService, FileStorageService>();
        }

        // Infrastructure Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();
        services.AddScoped<ISlugService, SlugService>();
        services.AddScoped<IPermissionService, PermissionService>();

        return services;
    }
}

