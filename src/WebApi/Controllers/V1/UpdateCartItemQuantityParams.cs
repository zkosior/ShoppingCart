namespace ShoppingCart.WebApi.Controllers.V1
{
	using System;
	using System.ComponentModel.DataAnnotations;

#pragma warning disable SA1300 // Element must begin with upper-case letter
#pragma warning disable IDE1006 // Naming Styles

	public class UpdateCartItemQuantityParams
	{
		public Guid cartId { get; set; }

		public Guid itemId { get; set; }

		[Range(1, int.MaxValue)]
		public int quantity { get; set; }
	}

#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore SA1300 // Element must begin with upper-case letter
}