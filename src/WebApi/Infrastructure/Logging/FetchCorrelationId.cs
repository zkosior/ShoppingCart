namespace ShoppingCart.WebApi.Infrastructure.Logging
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Primitives;
	using Serilog.Context;
	using System.Threading.Tasks;

	public class FetchCorrelationId
	{
		private const string CorrelationIdHeaderName = "X-Correlation-Id";
		private const string RequestIdHeaderName = "X-Request-Id";
		private const string CorrelationIdPropertyName = "CorrelationId";

		private readonly RequestDelegate next;

		public FetchCorrelationId(
			RequestDelegate next)
		{
			this.next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			string correlationId;
			if (context.Request.Headers.TryGetValue(
				CorrelationIdHeaderName,
				out StringValues correlationIdHeaderValues))
			{
				correlationId = string.Join(";", correlationIdHeaderValues.ToArray());
			}
			else
			{
				correlationId = context.TraceIdentifier;
			}

			context.Response.OnStarting(
				state =>
				{
					var httpContext = (HttpContext)state;

					httpContext.Response.Headers.Append(
						RequestIdHeaderName,
						context.TraceIdentifier);
					httpContext.Response.Headers.Append(
						CorrelationIdHeaderName,
						correlationId);

					return Task.FromResult(0);
				},
				context);

			using (LogContext.PushProperty(CorrelationIdPropertyName, correlationId))
			{
				await this.next(context);
			}
		}
	}
}