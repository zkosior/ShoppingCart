namespace ShoppingCart.HttpClients
{
	using Flurl;
	using Flurl.Http;
	using ShoppingCart.HttpClients.Configuration;
	using System;
	using System.Threading.Tasks;

	public class CartHttpClient : ICartHttpClient
	{
		private readonly IShoppingCartHttpClients configuration;

		public CartHttpClient(IShoppingCartHttpClients configuration)
		{
			this.configuration = configuration;
		}

		public async Task<Guid> CreateCart()
		{
			return await this.configuration.BaseUri.AbsoluteUri
				.AppendPathSegment("v1/carts")
				.WithTimeout(this.configuration.HttpTimeout.Value)
				.PostJsonAsync(default)
				.ReceiveJson<Guid>();
		}
	}
}