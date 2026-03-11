using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using sttbproject.Commons.Services;

namespace sttbproject.Commons.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        // Infrastructure Services
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();
        services.AddScoped<ISlugService, SlugService>();
        services.AddScoped<IPermissionService, PermissionService>();

        return services;
    }
}

