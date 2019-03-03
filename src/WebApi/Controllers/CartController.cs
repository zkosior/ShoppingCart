namespace ShoppingCart.WebApi.Controllers
{
	using Microsoft.AspNetCore.Mvc;
	using ShoppingCart.DataAccess.Repositories;
	using System;
	using System.Threading.Tasks;

	[Produces("application/json")]
	[ApiVersion("1")]
	[ApiController]
	[Route("v{version:apiVersion}")]
	public class CartController : ControllerBase
	{
		private readonly ICartRepository repository;

		public CartController(ICartRepository repository)
		{
			this.repository = repository;
		}

		[HttpPost("cart")]
#pragma warning disable CA1822 // Mark members as static
		public async Task<ActionResult<Guid>> CreateCart()
#pragma warning restore CA1822 // Mark members as static
		{
			return await this.repository.CreateCart();
		}
	}
}