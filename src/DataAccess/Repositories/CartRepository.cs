namespace ShoppingCart.DataAccess.Repositories
{
	using ShoppingCart.DataAccess.Models;
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public class CartRepository : ICartRepository
	{
#pragma warning disable CA1823 // Avoid unused private fields
		private static readonly List<Cart> Carts = new List<Cart>();
#pragma warning restore CA1823 // Avoid unused private fields

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