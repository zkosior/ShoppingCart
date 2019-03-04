namespace ShoppingCart.HttpClients
{
	using Flurl;
	using Flurl.Http;
	using ShoppingCart.Contracts;
	using ShoppingCart.HttpClients.Configuration;
	using ShoppingCart.HttpClients.Extensions;
	using System;
	using System.Net;
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
				.HandleFailure(allowEmptyResponse: false)
				.PostJsonAsync(default)
				.ReceiveJson<Guid>();
		}

		public async Task<Cart> GetCart(Guid cartId)
		{
			return await this.configuration.BaseUri.AbsoluteUri
				.AppendPathSegment($"v1/carts/{cartId}")
				.WithTimeout(this.configuration.HttpTimeout.Value)
				.HandleFailure(allowEmptyResponse: true)
				.GetAsync()
				.ReceiveJson<Cart>();
		}

		public async Task<bool> DeleteCart(Guid cartId)
		{
			var result = await this.configuration.BaseUri.AbsoluteUri
				.AllowHttpStatus(
					HttpStatusCode.NoContent,
					HttpStatusCode.NotFound)
				.AppendPathSegment($"v1/carts/{cartId}")
				.WithTimeout(this.configuration.HttpTimeout.Value)
				.HandleFailure(allowEmptyResponse: true)
				.DeleteAsync();

			return result.StatusCode == HttpStatusCode.NoContent;
		}

		public async Task<bool> DeleteAllCartItems(Guid cartId)
		{
			var result = await this.configuration.BaseUri.AbsoluteUri
				.AllowHttpStatus(
					HttpStatusCode.NoContent,
					HttpStatusCode.NotFound)
				.AppendPathSegment($"v1/carts/{cartId}/items")
				.WithTimeout(this.configuration.HttpTimeout.Value)
				.HandleFailure(allowEmptyResponse: true)
				.DeleteAsync();

			return result.StatusCode == HttpStatusCode.NoContent;
		}

		public async Task<bool> DeleteCartItem(Guid cartId, Guid itemId)
		{
			var result = await this.configuration.BaseUri.AbsoluteUri
				.AllowHttpStatus(
					HttpStatusCode.NoContent,
					HttpStatusCode.NotFound)
				.AppendPathSegment($"v1/carts/{cartId}/items/{itemId}")
				.WithTimeout(this.configuration.HttpTimeout.Value)
				.HandleFailure(allowEmptyResponse: true)
				.DeleteAsync();

			return result.StatusCode == HttpStatusCode.NoContent;
		}
	}
}