namespace ShoppingCart.WebApi
{
	using Microsoft.AspNetCore;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Serilog;
	using ShoppingCart.WebApi.Infrastructure.Logging;
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;

	[ExcludeFromCodeCoverage]
	public static class Program
	{
		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(Configuration(args))
				.Enrich.FromLogContext()
				.Enrich.WithUtcTimestamp()
				.Enrich.WithThreadId()
				.Filter.ByExcludingHealthCheck()
				.CreateLogger();

			try
			{
				Log.Information("Starting web host");
				CreateWebHostBuilder(args).Build().Run();
			}
			catch (Exception e)
			{
				Log.Fatal(e, "Host terminated unexpectedly");
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseSerilog();

		private static IConfiguration Configuration(string[] args) =>
			new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile(
				"appsettings.json",
				optional: false,
				reloadOnChange: true)
			.AddJsonFile(
				$"appsettings.{GetEnvironment() ?? "Production"}.json",
				optional: true)
			.AddUserSecrets<Startup>(optional: true)
			.AddEnvironmentVariables()
			.AddCommandLine(args)
			.Build();

		private static string GetEnvironment()
		{
			return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
		}
	}
}