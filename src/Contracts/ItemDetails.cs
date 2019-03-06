namespace ShoppingCart.Contracts
{
	using Microsoft.AspNetCore.Mvc.ModelBinding;

	public class ItemDetails
	{
		// Something that itentifies item externally
		// maybe id, maybe combination of properties
		// but it would be better to use external id for communication
		[BindRequired]
		public string Description { get; set; }
	}
}