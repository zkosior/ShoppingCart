namespace ShoppingCart.HttpClients.Configuration
{
	using Flurl.Http;
	using Flurl.Http.Configuration;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Generic;
	using System.Net;

	public static class HttpClientConfiguration
	{
		public static void ConfigureHttpClients(
			Func<Dictionary<string, string>> propagationHeaders)
		{
			FlurlHttp.Configure(
				globalConfiguration =>
				{
					globalConfiguration.JsonSerializer = CamelCaseCompact();
					globalConfiguration.BeforeCall =
					SetPropagationHeaders(propagationHeaders);
				});
		}

		public static void SetConnectionTimeout(
			Uri clientUri,
			int connectionTimeoutSeconds = 60)
		{
			var sp = ServicePointManager.FindServicePoint(clientUri);
			sp.ConnectionLeaseTimeout = connectionTimeoutSeconds * 1000;
		}

		private static NewtonsoftJsonSerializer CamelCaseCompact() =>
			new NewtonsoftJsonSerializer(
				new JsonSerializerSettings
				{
					// This is not required as .NET apps accept any formatting,
					// but to be consistent I'm specifying Camel Case explicitly
					// as it is the default behaviour in .NET Core 1.0+
					// Without it Flurl was sending Pascal Case.
					ContractResolver = new DefaultContractResolver
					{
						NamingStrategy = new CamelCaseNamingStrategy()
					},
					Formatting = Formatting.None
				});

		private static Action<HttpCall> SetPropagationHeaders(
			Func<Dictionary<string, string>> propagationHeaders)
		{
			return call =>
			{
				var headers = propagationHeaders();

				foreach (var header in headers)
				{
					if (!call.Request.Headers.Contains(header.Key))
					{
						call.Request.Headers.Add(
							header.Key,
							header.Value);
					}
				}
			};
		}
	}
}