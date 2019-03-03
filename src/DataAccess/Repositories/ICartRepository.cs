namespace ShoppingCart.DataAccess.Repositories
{
	using System;
	using System.Threading.Tasks;

	public interface ICartRepository
	{
		Task<Guid> CreateCart();
	}
}