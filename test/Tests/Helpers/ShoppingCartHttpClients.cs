namespace ShoppingCart.Tests.Helpers
{
	using ShoppingCart.HttpClients.Configuration;
	using System;

	public class ShoppingCartHttpClients : IShoppingCartHttpClients
	{
		public Uri BaseUri { get; set; }

		public TimeSpan? HttpTimeout { get; set; }
	}
}