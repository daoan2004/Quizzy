using Microsoft.Extensions.Diagnostics.HealthChecks;
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
            return canConnect
                ? HealthCheckResult.Healthy("Database connection is available.")
                : HealthCheckResult.Unhealthy("Database connection is unavailable.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(
                "Database connection check failed.",
                exception);
        }
    }
}
