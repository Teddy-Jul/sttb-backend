using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using sttbproject.entities;

namespace sttbproject.HostedServices;

public class DatabaseMigrationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMigrationService> _logger;

    public DatabaseMigrationService(
        IServiceProvider serviceProvider,
        ILogger<DatabaseMigrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting database migration service...");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SttbprojectContext>();

            var pendingMigrations = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Applying {Count} pending migrations: {Migrations}", pendingMigrations.Count, string.Join(", ", pendingMigrations));
                await context.Database.MigrateAsync(cancellationToken);
                _logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                _logger.LogInformation("No pending migrations found.");
            }

            // Auto-patch: Add 'slug' column to 'study_programs' if it doesn't exist
            _logger.LogInformation("Checking for database schema patches...");

            var addColumnSql = @"
                IF NOT EXISTS (SELECT * FROM sys.columns 
                               WHERE Name = N'slug' 
                               AND Object_ID = Object_ID(N'study_programs'))
                BEGIN
                    ALTER TABLE study_programs ADD slug NVARCHAR(200) NULL;
                END";

            try
            {
                await context.Database.ExecuteSqlRawAsync(addColumnSql, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to ensure 'slug' column exists. Continuing without blocking startup.");
            }

            var populateSlugSql = @"
                UPDATE study_programs 
                SET slug = LOWER(REPLACE(REPLACE(REPLACE(REPLACE(program_name, ' ', '-'), '.', ''), '(', ''), ')', ''))
                WHERE slug IS NULL OR LTRIM(RTRIM(slug)) = '';";

            try
            {
                await context.Database.ExecuteSqlRawAsync(populateSlugSql, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to populate 'slug' values. Manual review may be required.");
            }

            _logger.LogInformation("Database schema patches checked/applied.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Database migration service stopped.");
        return Task.CompletedTask;
    }
}
