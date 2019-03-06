namespace ShoppingCart.WebApi.Services
{
	using AutoMapper;
	using ShoppingCart.Contracts;
	using ShoppingCart.DataAccess.Repositories;
	using System;
	using System.Diagnostics;
	using System.Threading.Tasks;

	public class CartService
	{
		private readonly ICartRepository repository;
		private readonly IMapper mapper;

		public CartService(ICartRepository repository, IMapper mapper)
		{
			this.repository = repository;
			this.mapper = mapper;
		}

		public Task<Guid> CreateCart()
		{
			return this.repository.CreateCart();
		}

		public async Task<Cart> GetCart(Guid cartId)
		{
			var cart = await this.repository.GetCart(cartId);
			return this.mapper.Map<Cart>(cart);
		}

		public Task<bool> DeleteCart(Guid cartId)
		{
			return this.repository.DeleteCart(cartId);
		}

		public Task<bool> ClearCartItems(Guid cartId)
		{
			return this.repository.DeleteAllCartItems(cartId);
		}

		public Task<bool> DeleteCartItem(Guid cartId, Guid itemId)
		{
			return this.repository.DeleteCartItem(cartId, itemId);
		}

		public Task<bool> UpdateCartItemQuantity(
			Guid cartId,
			Guid itemId,
			int quantity)
		{
			Debug.Assert(quantity > 0, "Quantity must be higher than 0.");

			return this.repository.UpdateCartItemQuantity(
				cartId,
				itemId,
				quantity);
		}
	}
}