namespace ShoppingCart.WebApi.Infrastructure.Logging
{
	using Serilog.Core;
	using Serilog.Events;

	public class HealthCheckFilter : ILogEventFilter
	{
		public bool IsEnabled(LogEvent logEvent)
		{
			return !(
				(logEvent.Properties.ContainsKey("Path") &&
				logEvent.Properties["Path"].ToString().Contains(
					"healthcheck",
					System.StringComparison.InvariantCultureIgnoreCase)) ||
				(logEvent.Properties.ContainsKey("RequestPath") &&
				logEvent.Properties["RequestPath"].ToString().Contains(
					"healthcheck",
					System.StringComparison.InvariantCultureIgnoreCase)));
		}
	}
}