namespace ShoppingCart.Contracts
{
	using System;
	using System.Collections.Generic;

	public class Cart
	{
		public Guid Id { get; set; }

		public IEnumerable<Item> Items { get; set; }
	}
}