namespace ShoppingCart.WebApi.Controllers.V1
{
	using Microsoft.AspNetCore.Mvc;
	using ShoppingCart.Contracts;
	using ShoppingCart.WebApi.Services;
	using System;
	using System.Net;
	using System.Threading.Tasks;

	[Produces("application/json")]
	[ApiVersion("1")]
	[ApiController]
	[Route("v{version:apiVersion}")]
	public class CartController : ControllerBase
	{
		private readonly CartService service;

		public CartController(CartService service)
		{
			this.service = service;
		}

		[HttpPost("carts")]
		public async Task<ActionResult<Guid>> CreateCart()
		{
			var id = await this.service.CreateCart();
			return this.StatusCode((int)HttpStatusCode.Created, id);
		}

		[HttpGet("carts/{cartId}")]
		public async Task<ActionResult<Cart>> GetCart(Guid cartId)
		{
			return await this.service.GetCart(cartId);
		}
	}
}