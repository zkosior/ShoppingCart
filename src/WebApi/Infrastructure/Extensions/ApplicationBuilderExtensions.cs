namespace ShoppingCart.WebApi.Infrastructure.Extensions
{
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Mvc.ApiExplorer;
	using MoreLinq;
	using ShoppingCart.WebApi.Infrastructure.Failure;
	using System.Linq;

	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseInternalErrorHandlers(
			this IApplicationBuilder app)
		{
			// order of registered middleware is important
			app.UseMiddleware<LogUnhandledExceptions>();
			return app;
		}

		public static IApplicationBuilder UseSwaggerWithVersioning(
			this IApplicationBuilder app,
			IApiVersionDescriptionProvider provider)
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				(from x in provider.ApiVersionDescriptions
				 orderby x.GroupName descending
				 select x)
				 .ForEach(item =>
				 {
					 options.SwaggerEndpoint(
						 $"/swagger/{item.GroupName}/swagger.json",
						 item.GroupName.ToUpperInvariant());
				 });
			});
			return app;
		}
	}
}