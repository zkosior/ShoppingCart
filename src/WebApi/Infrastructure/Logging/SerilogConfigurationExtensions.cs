namespace ShoppingCart.WebApi.Infrastructure.Logging
{
	using Serilog;
	using Serilog.Configuration;
	using System;
	using System.Diagnostics.CodeAnalysis;

	[ExcludeFromCodeCoverage]
	public static class SerilogConfigurationExtensions
	{
		public static LoggerConfiguration WithUtcTimestamp(
			this LoggerEnrichmentConfiguration enrichmentConfiguration)
		{
			if (enrichmentConfiguration == null)
			{
				throw new ArgumentNullException(nameof(enrichmentConfiguration));
			}

			return enrichmentConfiguration.With<UtcTimestampEnricher>();
		}

		public static LoggerConfiguration ByExcludingHealthCheck(
			this LoggerFilterConfiguration filterConfiguration)
		{
			if (filterConfiguration == null)
			{
				throw new ArgumentNullException(nameof(filterConfiguration));
			}

			return filterConfiguration.With<HealthCheckFilter>();
		}
	}
}