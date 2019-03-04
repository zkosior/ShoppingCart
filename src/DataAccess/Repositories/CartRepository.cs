namespace ShoppingCart.DataAccess.Repositories
{
	using ShoppingCart.DataAccess.Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	public class CartRepository : ICartRepository
	{
		private static readonly List<Cart> Carts = new List<Cart>();

		private readonly object lockObject = new object();

		public Task<Guid> CreateCart()
		{
			lock (this.lockObject)
			{
				var cart = new Cart { Id = Guid.NewGuid() };
				Carts.Add(cart);
				return Task.FromResult(cart.Id);
			}
		}

		public Task<Cart> GetCart(Guid id)
		{
			lock (this.lockObject)
			{
				return Task.FromResult(Carts.Single(p => p.Id == id));
			}
		}

		public Task<bool> DeleteCart(Guid id)
		{
			// I intend to lie here a little.
			// Will assume that I can always delete cart.
			// So failure will happen only when cart was not there.
			lock (this.lockObject)
			{
				Cart cart;
				try
				{
					cart = Carts.Single(p => p.Id == id);
				}
				catch (InvalidOperationException)
				{
					return Task.FromResult(false);
				}

				return Task.FromResult(Carts.Remove(cart));
			}
		}
	}
}