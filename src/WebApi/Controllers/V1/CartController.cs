namespace ShoppingCart.WebApi.Controllers.V1
{
	using Microsoft.AspNetCore.Mvc;
	using ShoppingCart.Contracts;
	using ShoppingCart.WebApi.Infrastructure.Filters;
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
			return this.StatusCode(
				(int)HttpStatusCode.Created,
				await this.service.CreateCart());
		}

		[NotFoundResultFilter]
		[HttpGet("carts/{cartId}")]
		public async Task<ActionResult<Cart>> GetCart(Guid cartId)
		{
			return await this.service.GetCart(cartId);
		}

		[HttpDelete("carts/{cartId}")]
		public async Task<IActionResult> DeleteCart(Guid cartId)
		{
			if (await this.service.DeleteCart(cartId))
			{
				return this.NoContent();
			}
			else
			{
				return this.NotFound(default);
			}
		}

		[HttpDelete("carts/{cartId}/items")]
		public async Task<IActionResult> DeleteAllCartItems(Guid cartId)
		{
			if (await this.service.ClearCartItems(cartId))
			{
				return this.NoContent();
			}
			else
			{
				return this.NotFound(default);
			}
		}

		[HttpDelete("carts/{cartId}/items/{itemId}")]
		public async Task<IActionResult> DeleteCartItem(Guid cartId, Guid itemId)
		{
			if (await this.service.DeleteCartItem(cartId, itemId))
			{
				return this.NoContent();
			}
			else
			{
				return this.NotFound(default);
			}
		}

		[HttpPut("carts/{cartId}/items/{itemId}/quantity/{quantity}")]
		public async Task<IActionResult> UpdateCartItemQuantity(
			[FromRoute] UpdateCartItemQuantityParams parameters)
		{
			if (await this.service.UpdateCartItemQuantity(
				parameters.cartId,
				parameters.itemId,
				parameters.quantity))
			{
				return this.NoContent();
			}
			else
			{
				return this.NotFound(default);
			}
		}

		[HttpPost("carts/{cartId}/items")]
		public async Task<IActionResult> AddCartItem(
			Guid cartId,
			Item item)
		{
			var id = await this.service.AddCartItem(cartId, item);
			if (id != Guid.Empty)
			{
				return this.StatusCode(
					(int)HttpStatusCode.Created,
					id);
			}
			else
			{
				return this.NotFound(default);
			}
		}
	}
}