namespace ShoppingCart.Contracts
{
	using System;

	public class Item
	{
		public Guid Id { get; set; }

		public int Quantity { get; set; }
	}
}