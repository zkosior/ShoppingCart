namespace ShoppingCart.HttpClients
{
	using ShoppingCart.Contracts;
	using System;
	using System.Threading.Tasks;

	public interface ICartHttpClient
	{
		Task<Guid> CreateCart();

		Task<Cart> GetCart(Guid id);
	}
}