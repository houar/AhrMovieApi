using Microsoft.Extensions.Diagnostics.HealthChecks;
using Movies.Application.Database;

namespace Movies.Api.Health
{
    public class DBHealthCheck : IHealthCheck
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ILogger<DBHealthCheck> _logger;

        public const string HealthCheckName = "_dbHealthCheck";

        public DBHealthCheck(IDbConnectionFactory dbConnectionFactory, ILogger<DBHealthCheck> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
			try
			{
                await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
                return HealthCheckResult.Healthy("Database connection is healthy.");
            }
			catch (Exception ex)
			{
                _logger.LogError(ex, "Database connection check failed.");
                return HealthCheckResult.Unhealthy("Database connection is unhealthy.");
            }
        }
    }
}
