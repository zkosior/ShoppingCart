namespace ShoppingCart.Contracts
{
	using Microsoft.AspNetCore.Mvc.ModelBinding;

	public class ItemDetails
	{
		[BindRequired]
		public string ExternalId { get; set; }
	}
}