namespace ShoppingCart.HttpClients
{
	using System;
	using System.Threading.Tasks;

	public interface ICartHttpClient
	{
		Task<Guid> CreateCart();
	}
}