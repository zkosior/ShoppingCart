namespace ShoppingCart.WebApi.Infrastructure.Logging
{
	using Microsoft.AspNetCore.Builder;

	public static class SerilogServiceExtensions
	{
		public static IApplicationBuilder UseCorrelationIdHandler(
			this IApplicationBuilder app)
		{
			return app.UseMiddleware<FetchCorrelationId>();
		}
	}
}