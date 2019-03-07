namespace ShoppingCart.DataAccess.Repositories
{
	using ShoppingCart.DataAccess.Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	// Temporary replacement/fake for data storage
	public class CartRepository : ICartRepository
	{
		private static readonly List<Cart> Carts = new List<Cart>();

		private readonly object lockObject = new object();

		// always success (Guid)
		public Task<Guid> CreateCart()
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

		// success (object) or not found (null)
		public Task<Cart> GetCart(Guid cartId)
		{
			lock (this.lockObject)
			{
				return Task.FromResult(Carts.SingleOrDefault(p => p.Id == cartId));
			}
		}

		// success (true), (not found (false) or failure? (false))
		public Task<bool> DeleteCart(Guid cartId)
		{
			lock (this.lockObject)
			{
				var cart = Carts.SingleOrDefault(p => p.Id == cartId);
				if (cart == null)
				{
					return Task.FromResult(false);
				}

				// I intend to lie here a little.
				// Will assume that I can always delete cart.
				// So failure will happen only when cart was not there.
				return Task.FromResult(Carts.Remove(cart));
			}
		}

		// success (true), not found (false)
		public Task<bool> DeleteAllCartItems(Guid cartId)
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

		// success (true), cart not found (false), item not found (false) or failure?
		public Task<bool> DeleteCartItem(Guid cartId, Guid itemId)
		{
			lock (this.lockObject)
			{
				var cart = Carts.SingleOrDefault(p => p.Id == cartId);
				if (cart == null)
				{
					return Task.FromResult(false);
				}

				var item = cart.Items.SingleOrDefault(q => q.Id == itemId);
				if (item == null)
				{
					return Task.FromResult(false);
				}

				// I intend to lie here a little.
				// Will assume that I can always delete item.
				// So failure will happen only when cart or item was not there.
				cart.Items.Remove(item);
				return Task.FromResult(true);
			}
		}

		// success (true), cart not found (false), item not found (false)
		public Task<bool> UpdateCartItemQuantity(Guid cartId, Guid itemId, int quantity)
		{
			lock (this.lockObject)
			{
				var cart = Carts.SingleOrDefault(p => p.Id == cartId);
				if (cart == null)
				{
					return Task.FromResult(false);
				}

				var item = cart.Items.SingleOrDefault(q => q.Id == itemId);
				if (item == null)
				{
					return Task.FromResult(false);
				}

				item.Quantity = quantity;
				return Task.FromResult(true);
			}
		}

		// success (Guid) or cart not found (Empty)
		public Task<Guid> AddCartItem(
			Guid cartId,
			Item item)
		{
			lock (this.lockObject)
			{
				var cart = Carts.SingleOrDefault(p => p.Id == cartId);
				if (cart == null)
				{
					return Task.FromResult(Guid.Empty);
				}

				var existingItem = cart.Items.SingleOrDefault(
					p => p.Details.ExternalId == item.Details.ExternalId);

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