namespace ShoppingCart.DataAccess.Repositories
{
	using System;
	using System.Threading.Tasks;

	public interface ICartRepository
	{
		// Full data acces will probably be async anyway, so simulating it with this fake one
		Task<Guid> CreateCart();
	}
}