namespace ShoppingCart.DataAccess.Repositories
{
	using ShoppingCart.DataAccess.Models;
	using System;
	using System.Collections.Generic;
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
	}
}