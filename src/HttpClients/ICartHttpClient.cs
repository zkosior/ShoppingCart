namespace ShoppingCart.HttpClients
{
	using ShoppingCart.Contracts;
	using System;
	using System.Threading.Tasks;

	public interface ICartHttpClient
	{
		Task<Guid> CreateCart();

		Task<Cart> GetCart(Guid cartId);

		Task<bool> DeleteCart(Guid cartId);

		Task<bool> DeleteAllCartItems(Guid cartId);

		Task<bool> DeleteCartItem(Guid cartId, Guid itemId);

		Task<bool> UpdateCartItemQuantity(Guid cartId, Guid itemId, int quantity);

		Task<Guid> AddCartItem(Guid cartId, Item item);
	}
}