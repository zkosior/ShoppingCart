namespace ShoppingCart.WebApi.Infrastructure.Logging
{
	using Serilog.Core;
	using Serilog.Events;

	public class UtcTimestampEnricher : ILogEventEnricher
	{
		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory pf)
		{
			logEvent.AddPropertyIfAbsent(pf.CreateProperty("UtcTimestamp", logEvent.Timestamp.UtcDateTime));
		}
	}
}