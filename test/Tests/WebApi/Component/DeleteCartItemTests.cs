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
	public class DeleteCartItemTests
		: IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly HttpClient client;
		private readonly ICartRepository repository = Substitute.For<ICartRepository>();

		public DeleteCartItemTests(
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
		public async Task WhenCartAndItemExists_DeletesAndReturnsSuccess(Guid cartId, Guid itemId)
		{
			this.repository.DeleteCartItem(
				Arg.Is<Guid>(p => p == cartId),
				Arg.Is<Guid>(p => p == itemId)).Returns(true);

			var result = await this.client.DeleteAsync($"/v1/carts/{cartId}/items/{itemId}");

			AssertExt.ReturnsStatusCode_NoContent(result);
			await AssertExt.ReturnsEmptyContent(result);
		}

		[Theory]
		[AutoData]
		public async Task WhenCartOrItemDoesNotExist_ReturnsFailure(Guid cartId, Guid itemId)
		{
			this.repository.DeleteCartItem(
				Arg.Is<Guid>(p => p == cartId),
				Arg.Is<Guid>(p => p == itemId)).Returns(false);

			var result = await this.client.DeleteAsync($"/v1/carts/{cartId}/items/{itemId}");

			AssertExt.ReturnsStatusCode_NotFound(result);
			await AssertExt.ReturnsEmptyContent(result);
		}

		private void OverrideServices(IServiceCollection services)
		{
			services.AddSingleton(s => this.repository);
		}
	}
}