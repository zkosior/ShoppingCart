namespace ShoppingCart.HttpClients.Extensions
{
	using Flurl.Http;
	using ShoppingCart.HttpClients.Exceptions;
	using System;

	internal static class FailureHandler
	{
		public static Exception GetException(FlurlHttpException exception)
		{
			var requestUri = exception.Call.Request.RequestUri;

			if (exception is FlurlHttpTimeoutException)
			{
				return new TimeoutException(FormattableString.Invariant(
					$"Call to {requestUri} failed with timeout."));
			}

			return new UnexpectedHttpResponseException(
				$"Call to {requestUri} failed with status NotFound and no content.");
		}
	}
}