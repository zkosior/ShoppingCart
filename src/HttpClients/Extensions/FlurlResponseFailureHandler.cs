namespace ShoppingCart.HttpClients.Extensions
{
	using Flurl;
	using Flurl.Http;
	using Flurl.Http.Configuration;
	using System.Collections.Generic;
	using System.Net;
	using System.Net.Http;
	using System.Threading;
	using System.Threading.Tasks;

	internal class FlurlResponseFailureHandler : IFlurlRequest
	{
		private readonly IFlurlRequest request;
		private readonly bool allowEmptyResponse;

		public FlurlResponseFailureHandler(IFlurlRequest request, bool allowEmptyResponse)
		{
			this.request = request;
			this.allowEmptyResponse = allowEmptyResponse;
		}

		public IFlurlClient Client { get => this.request.Client; set => this.request.Client = value; }

		public Url Url { get => this.request.Url; set => this.request.Url = value; }

		public FlurlHttpSettings Settings { get => this.request.Settings; set => this.request.Settings = value; }

		public IDictionary<string, object> Headers => this.request.Headers;

		public IDictionary<string, Cookie> Cookies => this.request.Cookies;

		public async Task<HttpResponseMessage> SendAsync(
			HttpMethod verb,
			HttpContent content = null,
			CancellationToken cancellationToken = default,
			HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
		{
			try
			{
				return await this.request.SendAsync(verb, content, cancellationToken, completionOption);
			}
			catch (FlurlHttpException e)
			{
				if (this.allowEmptyResponse &&
					e.Call.Response?.StatusCode == HttpStatusCode.NotFound)
				{
					return null;
				}

				throw FailureHandler.GetException(e);
			}
		}
	}
}