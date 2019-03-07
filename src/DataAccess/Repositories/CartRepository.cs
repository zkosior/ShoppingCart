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

		public Task<Guid> CreateCart() // always success
		{
			lock (this.lockObject)
			{
				var cart = new Cart
				{
					Id = Guid.NewGuid(),
					Items = new List<Item>(),
				};
				Carts.Add(cart);
				return Task.FromResult(cart.Id);
			}
		}

		public Task<Cart> GetCart(Guid cartId) // success or not found
		{
			lock (this.lockObject)
			{
				return Task.FromResult(Carts.SingleOrDefault(p => p.Id == cartId));
			}
		}

		public Task<bool> DeleteCart(Guid cartId) // success, (not found or failure?)
		{
			// I intend to lie here a little.
			// Will assume that I can always delete cart.
			// So failure will happen only when cart was not there.
			lock (this.lockObject)
			{
				var cart = Carts.SingleOrDefault(p => p.Id == cartId);
				if (cart == null)
				{
					return Task.FromResult(false);
				}

				return Task.FromResult(Carts.Remove(cart));
			}
		}

		public Task<bool> DeleteAllCartItems(Guid cartId) // success, not found or failure
		{
			lock (this.lockObject)
			{
				var cart = Carts.SingleOrDefault(p => p.Id == cartId);
				if (cart == null)
				{
					return Task.FromResult(false);
				}

				cart.Items = new List<Item>();
				return Task.FromResult(true);
			}
		}

		public Task<bool> DeleteCartItem(Guid cartId, Guid itemId) // success, cart not found, item not found or failure
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

		public Task<bool> UpdateCartItemQuantity(Guid cartId, Guid itemId, int quantity) // success, cart not found, item not found
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
			Item item) // success or cart not found
		{
			// Add item could find existing and update quantity
			// but external id would be required
			lock (this.lockObject)
			{
				Cart cart;
				Item existingItem;
				try
				{
					cart = Carts.Single(p => p.Id == cartId);
					existingItem = cart.Items.SingleOrDefault(p => p.Details.Description == item.Details.Description);
				}
				catch (InvalidOperationException)
				{
					return Task.FromResult(Guid.Empty);
				}

				if (existingItem != null)
				{
					existingItem.Quantity += item.Quantity;
					return Task.FromResult(existingItem.Id);
				}
				else
				{
					item.Id = Guid.NewGuid();
					cart.Items.Add(item);
					return Task.FromResult(item.Id);
				}
			}
		}
	}
}