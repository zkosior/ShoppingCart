namespace ShoppingCart.Tests.WebApi.Component
{
	using AutoFixture.Xunit2;
	using FluentAssertions;
	using Microsoft.AspNetCore.Mvc.Testing;
	using Microsoft.Extensions.DependencyInjection;
	using NSubstitute;
	using ShoppingCart.DataAccess.Repositories;
	using ShoppingCart.Tests.Helpers;
	using ShoppingCart.WebApi;
	using System;
	using System.Collections.Generic;
	using System.Net;
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

			ReturnsStatusCode_NoContent(result);
			await ReturnsEmptyContent(result);
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

			ReturnsStatusCode_NotFound(result);
			await ReturnsEmptyContent(result);
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

			ReturnsStatusCode_BadRequest(result);
		}

		private static async Task ReturnsEmptyContent(HttpResponseMessage result)
		{
			(await result.Content.ReadAsStringAsync()).Should().BeEmpty();
		}

		private static void ReturnsStatusCode_NoContent(HttpResponseMessage result)
		{
			Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
		}

		private static void ReturnsStatusCode_NotFound(HttpResponseMessage result)
		{
			Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
		}

		private static void ReturnsStatusCode_BadRequest(HttpResponseMessage result)
		{
			Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
		}

		private void OverrideServices(IServiceCollection services)
		{
			services.AddSingleton(s => this.repository);
		}
	}
}