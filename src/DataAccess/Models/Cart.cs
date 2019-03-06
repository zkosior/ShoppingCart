namespace ShoppingCart.DataAccess.Models
{
	using System;
	using System.Collections.Generic;

	public class Cart
	{
		public Guid Id { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only

		public List<Item> Items { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
	}
}