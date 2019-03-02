namespace ShoppingCart.WebApi.Infrastructure.Extensions
{
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ApiExplorer;
	using Microsoft.Extensions.DependencyInjection;
	using MoreLinq;
	using Newtonsoft.Json.Converters;
	using Swashbuckle.AspNetCore.Swagger;
	using System.Linq;

	public static class ServiceExtensions
	{
		public static IServiceCollection AddMvcWithDefaults(
			this IServiceCollection services)
		{
			services
				.AddMvc(options =>
				 {
					 options.RespectBrowserAcceptHeader = true;
					 options.ReturnHttpNotAcceptable = true;
				 })
				.AddXmlSerializerFormatters()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
				.AddJsonOptions(options =>
				{
					options.SerializerSettings.Converters.Add(
						new StringEnumConverter());
				});
			services
				.AddApiVersioning(options =>
				{
					options.ReportApiVersions = true;
				})
				.AddVersionedApiExplorer(options =>
				{
					options.GroupNameFormat = "'v'VVV";
					options.SubstituteApiVersionInUrl = true;
				})
				.AddSwaggerWithVersioning("Shopping Cart Web Api");
			return services;
		}

		private static IServiceCollection AddSwaggerWithVersioning(
			this IServiceCollection services,
			string swaggerPageTitle)
		{
			services.AddSwaggerGen(options =>
			{
				using (ServiceProvider provider = services.BuildServiceProvider())
				{
					var descriptions = provider
						.GetRequiredService<IApiVersionDescriptionProvider>()
						.ApiVersionDescriptions;
					(from x in descriptions
					 orderby x.GroupName descending
					 select x)
					.ForEach(item =>
					{
						options.SwaggerDoc(
							item.GroupName,
							CreateInfoForApiVersion(item, swaggerPageTitle));
					});
				}
				options.CustomSchemaIds(x => x.FullName);
				options.DescribeAllEnumsAsStrings();
				options.ResolveConflictingActions(apiDescriptions =>
					(from x in apiDescriptions
					 orderby x.GroupName descending
					 select x).First());
			});
			return services;
		}

		private static Info CreateInfoForApiVersion(
			ApiVersionDescription description,
			string swaggerPageTitle)
		{
			Info info = new Info
			{
				Title = swaggerPageTitle,
				Version = description.ApiVersion.ToString()
			};
			if (description.IsDeprecated)
			{
				info.Description += " This API version has been deprecated.";
			}

			return info;
		}
	}
}