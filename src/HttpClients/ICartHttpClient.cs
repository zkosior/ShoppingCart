namespace ShoppingCart.HttpClients
{
	using ShoppingCart.Contracts;
	using System;
	using System.Threading.Tasks;

	public interface ICartHttpClient
	{
		Task<Guid> CreateCart();

		Task<Cart> GetCart(Guid id);

		Task<bool> DeleteCart(Guid id);

		Task<bool> ClearCartItems(Guid cartId);
	}
}