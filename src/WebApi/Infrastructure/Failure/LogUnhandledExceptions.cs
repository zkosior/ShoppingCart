namespace ShoppingCart.WebApi.Infrastructure.Failure
{
	using Microsoft.AspNetCore.Http;
	using Serilog;
	using System;
	using System.Threading.Tasks;

	public class LogUnhandledExceptions
	{
		private readonly RequestDelegate next;
		private readonly ILogger log;

		public LogUnhandledExceptions(
			RequestDelegate next,
			ILogger logger)
		{
			this.next = next;
			this.log = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await this.next(context);
			}
			catch (Exception ex)
			{
				this.log.Error(ex, "An unhandled exception has occurred.");
				throw;
			}
		}
	}
}