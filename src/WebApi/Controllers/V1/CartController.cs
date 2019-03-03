namespace ShoppingCart.WebApi.Controllers.V1
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

		[HttpPost("carts")]
		public async Task<ActionResult<Guid>> CreateCart()
		{
			return await this.repository.CreateCart();
		}
	}
}