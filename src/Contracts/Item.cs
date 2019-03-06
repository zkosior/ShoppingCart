namespace ShoppingCart.Contracts
{
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using System;
	using System.ComponentModel.DataAnnotations;

	public class Item
	{
		// External id might be better for some scenarios
		public Guid Id { get; set; }

		[BindRequired]
		[Range(1, int.MaxValue)]
		public int Quantity { get; set; }

		[Required]
		public ItemDetails Details { get; set; }
	}
}