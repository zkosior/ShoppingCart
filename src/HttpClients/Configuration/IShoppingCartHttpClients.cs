namespace ShoppingCart.HttpClients.Configuration
{
	using System;

	public interface IShoppingCartHttpClients
	{
		Uri BaseUri { get; }

		TimeSpan? HttpTimeout { get; }
	}
}