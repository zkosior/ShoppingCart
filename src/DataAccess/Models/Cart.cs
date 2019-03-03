namespace ShoppingCart.DataAccess.Models
{
	using System;
	using System.Collections.Generic;

	public class Cart
	{
		public Guid Id { get; set; }

		public List<Tuple<Item, int>> Items { get; } = new List<Tuple<Item, int>>();
	}
}