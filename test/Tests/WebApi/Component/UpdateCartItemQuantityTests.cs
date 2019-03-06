namespace ShoppingCart.Tests.WebApi.Component
{
	using AutoFixture.Xunit2;
	using Microsoft.AspNetCore.Mvc.Testing;
	using Microsoft.Extensions.DependencyInjection;
	using NSubstitute;
	using ShoppingCart.DataAccess.Repositories;
	using ShoppingCart.Tests.Helpers;
	using ShoppingCart.WebApi;
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Xunit;

	[Trait("TestCategory", "Component")]
	public class UpdateCartItemQuantityTests
		: IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly HttpClient client;
		private readonly ICartRepository repository = Substitute.For<ICartRepository>();

		public UpdateCartItemQuantityTests(
			WebApplicationFactory<Startup> webApiFixture)
		{
			this.client = webApiFixture
				.Create(
					new Dictionary<string, string>()
					{
					},
					this.OverrideServices)
				.CreateClient();
		}

		[Theory]
		[AutoData]
		public async Task WhenCartAndItemExists_UpdatesQuantityAndReturnsSuccess(
			Guid cartId,
			Guid itemId,
			uint quantity)
		{
			if (quantity == 0)
			{
				quantity = 100; // just to make it work
			}

			this.repository.UpdateCartItemQuantity(
				Arg.Is<Guid>(p => p == cartId),
				Arg.Is<Guid>(p => p == itemId),
				Arg.Is<int>(p => p == quantity)).Returns(true);

			var result = await this.client.PutAsync(
				$"/v1/carts/{cartId}/items/{itemId}/quantity/{quantity}",
				null);

			AssertExt.ReturnsStatusCode_NoContent(result);
			await AssertExt.ReturnsEmptyContent(result);
		}

		[Theory]
		[AutoData]
		public async Task WhenCartOrItemDoesNotExist_ReturnsFailure(
			Guid cartId,
			Guid itemId,
			uint quantity)
		{
			if (quantity == 0)
			{
				quantity = 100; // just to make it work
			}

			this.repository.UpdateCartItemQuantity(
				Arg.Is<Guid>(p => p == cartId),
				Arg.Is<Guid>(p => p == itemId),
				Arg.Is<int>(p => p == quantity)).Returns(false);

			var result = await this.client.PutAsync(
				$"/v1/carts/{cartId}/items/{itemId}/quantity/{quantity}",
				null);

			AssertExt.ReturnsStatusCode_NotFound(result);
			await AssertExt.ReturnsEmptyContent(result);
		}

		[Theory]
		[InlineAutoData(0)]
		[InlineAutoData(-1)]
		public async Task WhenQuantityIncorrect_ReturnsBadRequest(
			int quantity,
			Guid cartId,
			Guid itemId)
		{
			var result = await this.client.PutAsync(
				$"/v1/carts/{cartId}/items/{itemId}/quantity/{quantity}",
				null);

			AssertExt.ReturnsStatusCode_BadRequest(result);
		}

		private void OverrideServices(IServiceCollection services)
		{
			services.AddSingleton(s => this.repository);
		}
	}
}