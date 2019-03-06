namespace ShoppingCart.Tests.WebApi.Component
{
	using AutoFixture.Xunit2;
	using Microsoft.AspNetCore.Mvc.Testing;
	using Microsoft.Extensions.DependencyInjection;
	using NSubstitute;
	using ShoppingCart.Contracts;
	using ShoppingCart.DataAccess.Repositories;
	using ShoppingCart.Tests.Helpers;
	using ShoppingCart.WebApi;
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Xunit;

	[Trait("TestCategory", "Component")]
	public class AddCartItemTests
		: IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly HttpClient client;
		private readonly ICartRepository repository = Substitute.For<ICartRepository>();

		public AddCartItemTests(
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
		public async Task WhenCartExists_AddsItemAndReturnsSuccess(Guid cartId, Item item)
		{
			this.repository.AddCartItem(
				Arg.Is<Guid>(p => p == cartId),
				Arg.Any<DataAccess.Models.Item>()).Returns(item.Id);

			var result = await this.client.PostAsJsonAsync(
				$"/v1/carts/{cartId}/items",
				item);

			AssertExt.ReturnsStatusCode_Created(result);
			await AssertExt.ReturnsIdOfCreatedItem(item.Id, result);
		}

		[Theory]
		[AutoData]
		public async Task WhenCartDoesNotExist_ReturnsFailure(Guid cartId, Item item)
		{
			this.repository.AddCartItem(
				Arg.Is<Guid>(p => p == cartId),
				Arg.Any<DataAccess.Models.Item>()).Returns(Guid.Empty);

			var result = await this.client.PostAsJsonAsync(
				$"/v1/carts/{cartId}/items",
				item);

			AssertExt.ReturnsStatusCode_NotFound(result);
			await AssertExt.ReturnsEmptyContent(result);
		}

		[Theory]
		[AutoData]
		public async Task WhenItemEmpty_ReturnsBadRequest(Guid cartId)
		{
			this.repository.AddCartItem(
				Arg.Is<Guid>(p => p == cartId),
				Arg.Any<DataAccess.Models.Item>()).Returns(Guid.Empty);

			var result = await this.client.PostAsJsonAsync<Item>(
				$"/v1/carts/{cartId}/items",
				default);

			AssertExt.ReturnsStatusCode_BadRequest(result);
		}

		[Theory]
		[AutoData]
		public async Task WhenItemDetailsEmpty_ReturnsBadRequest(Guid cartId, Item item)
		{
			item.Details = default;

			this.repository.AddCartItem(
				Arg.Is<Guid>(p => p == cartId),
				Arg.Any<DataAccess.Models.Item>()).Returns(Guid.Empty);

			var result = await this.client.PostAsJsonAsync(
				$"/v1/carts/{cartId}/items",
				item);

			AssertExt.ReturnsStatusCode_BadRequest(result);
		}

		[Theory]
		[InlineAutoData(0)]
		[InlineAutoData(-1)]
		public async Task WhenIncorrentQuantity_ReturnsBadRequest(
			int quantity,
			Guid cartId,
			Item item)
		{
			item.Quantity = quantity;

			this.repository.AddCartItem(
				Arg.Is<Guid>(p => p == cartId),
				Arg.Any<DataAccess.Models.Item>()).Returns(Guid.Empty);

			var result = await this.client.PostAsJsonAsync(
				$"/v1/carts/{cartId}/items",
				item);

			AssertExt.ReturnsStatusCode_BadRequest(result);
		}

		private void OverrideServices(IServiceCollection services)
		{
			services.AddSingleton(s => this.repository);
		}
	}
}