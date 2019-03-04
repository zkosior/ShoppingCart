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
	public class DeleteAllCartItemsTests
		: IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly HttpClient client;
		private readonly ICartRepository repository = Substitute.For<ICartRepository>();

		public DeleteAllCartItemsTests(
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
		public async Task WhenCartExists_DeletesAndReturnsSuccess(Guid id)
		{
			this.repository.DeleteAllCartItems(Arg.Is<Guid>(p => p == id)).Returns(true);

			var result = await this.client.DeleteAsync($"/v1/carts/{id}/items");

			ReturnsStatusCode_NoContent(result);
			await ReturnsEmptyContent(result);
		}

		[Theory]
		[AutoData]
		public async Task WhenCartDoesNotExist_ReturnsFailure(Guid id)
		{
			this.repository.DeleteAllCartItems(Arg.Is<Guid>(p => p == id)).Returns(false);

			var result = await this.client.DeleteAsync($"/v1/carts/{id}/items");

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