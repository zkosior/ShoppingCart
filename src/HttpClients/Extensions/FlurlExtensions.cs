namespace ShoppingCart.HttpClients.Extensions
{
	using Flurl;
	using Flurl.Http;

	public static class FlurlExtensions
	{
		public static IFlurlRequest HandleFailure(this IFlurlRequest request, bool allowEmptyResponse = false)
		{
			return new FlurlResponseFailureHandler(request, allowEmptyResponse);
		}

		public static IFlurlRequest HandleFailure(this Url url, bool allowEmptyResponse = false)
		{
			return new FlurlResponseFailureHandler(new FlurlRequest(url), allowEmptyResponse);
		}

#pragma warning disable CA1054 // Uri parameters should not be strings

		public static IFlurlRequest HandleFailure(this string url, bool allowEmptyResponse = false)
		{
			return new FlurlResponseFailureHandler(new FlurlRequest(url), allowEmptyResponse);
		}

#pragma warning restore CA1054 // Uri parameters should not be strings

		public static IFlurlRequest WithApplicationJsonHeader(this IFlurlRequest request)
		{
			return request.WithHeaders(new { Content_Type = "application/json" });
		}
	}
}