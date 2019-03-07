namespace ShoppingCart.WebApi
{
	using HealthChecks.UI.Client;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Diagnostics.HealthChecks;
	using Microsoft.AspNetCore.Mvc.ApiExplorer;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Serilog;
	using ShoppingCart.WebApi.Infrastructure.Extensions;
	using ShoppingCart.WebApi.Infrastructure.Logging;

	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			this.Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddSingleton(Log.Logger)
				.RegisterServices()
				.AddMvcWithDefaults()
				.AddHealthChecks();
		}

		public void Configure(
			IApplicationBuilder app,
			IApiVersionDescriptionProvider provider)
		{
			app
				.UseCorrelationIdHandler()
				.UseHealthChecks("/healthcheck", new HealthCheckOptions
				{
					Predicate = _ => true,
					ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
				})
				.UseInternalErrorHandlers()
				.UseSwaggerWithVersioning(provider)
				.UseMvc();
		}
	}
}