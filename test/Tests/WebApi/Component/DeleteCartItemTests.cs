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

			ReturnsStatusCode_NoContent(result);
			await ReturnsEmptyContent(result);
		}

		[Theory]
		[AutoData]
		public async Task WhenCartOrItemDoesNotExist_ReturnsFailure(Guid cartId, Guid itemId)
		{
			this.repository.DeleteCartItem(
				Arg.Is<Guid>(p => p == cartId),
				Arg.Is<Guid>(p => p == itemId)).Returns(false);

			var result = await this.client.DeleteAsync($"/v1/carts/{cartId}/items/{itemId}");

			ReturnsStatusCode_NotFound(result);
			await ReturnsEmptyContent(result);
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

		private void OverrideServices(IServiceCollection services)
		{
			services.AddSingleton(s => this.repository);
		}
	}
}