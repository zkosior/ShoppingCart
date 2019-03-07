namespace ShoppingCart.HttpClients
{
	using Flurl;
	using Flurl.Http;
	using Newtonsoft.Json;
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
				.AllowHttpStatus(HttpStatusCode.Created)
				.AppendPathSegment("v1/carts")
				.WithTimeout(this.configuration.HttpTimeout.Value)
				.HandleFailure(allowEmptyResponse: false)
				.PostJsonAsync(default)
				.ReceiveJson<Guid>();
		}

		public async Task<Cart> GetCart(Guid cartId)
		{
			return await this.configuration.BaseUri.AbsoluteUri
				.AllowHttpStatus(
					HttpStatusCode.OK,
					HttpStatusCode.NotFound)
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

		public async Task<bool> UpdateCartItemQuantity(
			Guid cartId,
			Guid itemId,
			int quantity)
		{
			if (quantity <= 0)
			{
				throw new ArgumentException(
					$"Parameter '{nameof(quantity)}' must be at least 1.");
			}

			var result = await this.configuration.BaseUri.AbsoluteUri
				.AllowHttpStatus(
					HttpStatusCode.NoContent,
					HttpStatusCode.NotFound,
					HttpStatusCode.BadRequest)
				.AppendPathSegment(
					$"v1/carts/{cartId}/items/{itemId}/quantity/{quantity}")
				.WithTimeout(this.configuration.HttpTimeout.Value)
				.HandleFailure(allowEmptyResponse: true)
				.PutAsync(null);

			if (result.StatusCode == HttpStatusCode.BadRequest)
			{
				throw new ArgumentException(
					$"Bad request. Delails: '{await result.Content.ReadAsStringAsync()}'");
			}

			return result.StatusCode == HttpStatusCode.NoContent;
		}

		public async Task<Guid> AddCartItem(Guid cartId, Item item)
		{
			ValidateInput(item);

			var result = await this.configuration.BaseUri.AbsoluteUri
				.AllowHttpStatus(
					HttpStatusCode.NoContent,
					HttpStatusCode.NotFound,
					HttpStatusCode.BadRequest)
				.AppendPathSegment(
					$"v1/carts/{cartId}/items")
				.WithTimeout(this.configuration.HttpTimeout.Value)
				.HandleFailure(allowEmptyResponse: false)
				.PostJsonAsync(item);

			if (result.StatusCode == HttpStatusCode.NotFound)
			{
				throw new ArgumentException($"Cart with id '{cartId}' was not found.");
			}
			else if (result.StatusCode == HttpStatusCode.BadRequest)
			{
				throw new ArgumentException(
					$"Bad request. Delails: '{await result.Content.ReadAsStringAsync()}'");
			}
			else if (result.StatusCode == HttpStatusCode.Created)
			{
				var id = JsonConvert.DeserializeObject<Guid>(
					await result.Content.ReadAsStringAsync());
				if (id != Guid.Empty)
				{
					return id;
				}
				else
				{
					throw new InvalidOperationException("Invalid id returned by server.");
				}
			}

			throw new InvalidOperationException("This should never happen.");
		}

		private static void ValidateInput(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException($"Parameter '{nameof(item)}' can not be null.");
			}

			if (item.Quantity <= 0)
			{
				throw new ArgumentException(
					$"Parameter '{nameof(item.Quantity)}' must be at least 1.");
			}

			if (item.Details == null || string.IsNullOrWhiteSpace(item.Details.ExternalId))
			{
				throw new ArgumentNullException(
					$"Item details must be provided.");
			}
		}
	}
}