using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;

namespace ProjectBase.Health;

public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly DataContext _context;

    public DatabaseHealthCheck(DataContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            if (!canConnect)
            {
                return HealthCheckResult.Unhealthy(
                    "Database connection is unavailable.");
            }

            if (_context.Database.IsRelational())
            {
                var pendingMigrations = await _context.Database
                    .GetPendingMigrationsAsync(cancellationToken);
                var pendingCount = pendingMigrations.Count();
                if (pendingCount > 0)
                {
                    return HealthCheckResult.Unhealthy(
                        $"{pendingCount} database migration(s) are pending.",
                        data: new Dictionary<string, object>
                        {
                            ["pendingMigrationCount"] = pendingCount
                        });
                }
            }

            return HealthCheckResult.Healthy(
                "Database connection is available and schema is current.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(
                "Database connection check failed.",
                exception);
        }
    }
}
