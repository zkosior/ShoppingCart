namespace ShoppingCart.Tests.Helpers
{
	using Microsoft.AspNetCore.Mvc.Testing;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using System;
	using System.Collections.Generic;

	public static class IntegrationTestExtensions
	{
		public static WebApplicationFactory<TEntryPoint> Create<TEntryPoint>(
					this WebApplicationFactory<TEntryPoint> fixture,
					Dictionary<string, string> configuration,
					Action<IServiceCollection> configureServices)
					where TEntryPoint : class
		{
			return fixture.WithWebHostBuilder(builder => builder
				.ConfigureAppConfiguration((contect, conf) =>
					conf.AddInMemoryCollection(configuration))
				.ConfigureServices(configureServices));
		}
	}
}