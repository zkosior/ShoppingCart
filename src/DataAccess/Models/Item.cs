namespace ShoppingCart.DataAccess.Models
{
	using System;

	public class Item
	{
		public Guid Id { get; set; }

		public Guid ExternalId { get; set; }
	}
}