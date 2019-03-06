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

		public Task<Cart> GetCart(Guid cartId)
		{
			lock (this.lockObject)
			{
				return Task.FromResult(Carts.Single(p => p.Id == cartId));
			}
		}

		public Task<bool> DeleteCart(Guid cartId)
		{
			// I intend to lie here a little.
			// Will assume that I can always delete cart.
			// So failure will happen only when cart was not there.
			lock (this.lockObject)
			{
				Cart cart;
				try
				{
					cart = Carts.Single(p => p.Id == cartId);
				}
				catch (InvalidOperationException)
				{
					return Task.FromResult(false);
				}

				return Task.FromResult(Carts.Remove(cart));
			}
		}

		public Task<bool> DeleteAllCartItems(Guid cartId)
		{
			lock (this.lockObject)
			{
				Cart cart;
				try
				{
					cart = Carts.Single(p => p.Id == cartId);
				}
				catch (InvalidOperationException)
				{
					return Task.FromResult(false);
				}

				cart.Items = new List<Item>();
				return Task.FromResult(true);
			}
		}

		public Task<bool> DeleteCartItem(Guid cartId, Guid itemId)
		{
			lock (this.lockObject)
			{
				Cart cart;
				Item item;
				try
				{
					cart = Carts.Single(p => p.Id == cartId);
					item = cart.Items.Single(q => q.Id == itemId);
				}
				catch (InvalidOperationException)
				{
					return Task.FromResult(false);
				}

				cart.Items.Remove(item);

				return Task.FromResult(true);
			}
		}

		public Task<bool> UpdateCartItemQuantity(Guid cartId, Guid itemId, int quantity)
		{
			lock (this.lockObject)
			{
				try
				{
					Carts.Single(p => p.Id == cartId)
						.Items.Single(q => q.Id == itemId)
						.Quantity = quantity;
				}
				catch (InvalidOperationException)
				{
					return Task.FromResult(false);
				}
			}

			return Task.FromResult(true);
		}

		public Task<Guid> AddCartItem(
			Guid cartId,
			Item item)
		{
			// Add item could find existing and update quantity
			// but external id would be required
			lock (this.lockObject)
			{
				Cart cart;
				try
				{
					cart = Carts.Single(p => p.Id == cartId);
				}
				catch (InvalidOperationException)
				{
					return Task.FromResult(Guid.Empty);
				}

				item.Id = Guid.NewGuid();
				cart.Items.Add(item);
				return Task.FromResult(item.Id);
			}
		}
	}
}