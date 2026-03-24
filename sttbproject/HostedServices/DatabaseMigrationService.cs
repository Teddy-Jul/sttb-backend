using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

            // We safely apply pending structure migrations first if any exist so EF Core gets initialized properly over Master connection
            var pendingMigrations = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Applying {Count} pending migrations: {Migrations}", pendingMigrations.Count, string.Join(", ", pendingMigrations));
                await context.Database.MigrateAsync(cancellationToken);
                _logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                _logger.LogInformation("No pending EF migrations found.");
            }

            // Locate initial.sql
            string[] possiblePaths = {
                Path.Combine(Directory.GetCurrentDirectory(), "..", "sttbproject.entities", "Migration", "initial.sql"),
                Path.Combine(AppContext.BaseDirectory, "Migration", "initial.sql"),
                Path.Combine(AppContext.BaseDirectory, "initial.sql")
            };

            string? sqlFilePath = possiblePaths.FirstOrDefault(File.Exists);

            if (sqlFilePath != null)
            {
                _logger.LogInformation("Executing base script from {Path}", sqlFilePath);
                var script = await File.ReadAllTextAsync(sqlFilePath, cancellationToken);
                
                // Split GO blocks safely
                var commands = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                // Use Raw ADO.NET connection to avoid DbContext String Formatter `{}` breaks (JSON parsing issues)
                var connection = context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken);
                }

                foreach (var cmdStr in commands)
                {
                    var sql = cmdStr.Trim();
                    if (string.IsNullOrWhiteSpace(sql)) continue;

                    try
                    {
                        using var dbCommand = connection.CreateCommand();
                        dbCommand.CommandText = sql;
                        
                        if (context.Database.CurrentTransaction != null)
                        {
                            dbCommand.Transaction = context.Database.CurrentTransaction.GetDbTransaction();
                        }

                        await dbCommand.ExecuteNonQueryAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        // "CREATE DATABASE" or "USE" statements frequently fail from EF context connection policies. Safe to skip.
                        if (sql.StartsWith("CREATE DATABASE", StringComparison.OrdinalIgnoreCase) || 
                            sql.StartsWith("USE ", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogWarning("Skipping DB CREATE/USE command due to context limitations.");
                        }
                        else
                        {
                            // If a query still fails here, it is an actual SQL syntax or structural problem.
                            _logger.LogError(ex, "Script execution failed on SQL block:\n{CommandText}", sql.Substring(0, Math.Min(300, sql.Length)));
                        }
                    }
                }
                
                _logger.LogInformation("initial.sql applied successfully.");
            }
            else
            {
                _logger.LogWarning("initial.sql not found! Skipped executing base script.");
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
