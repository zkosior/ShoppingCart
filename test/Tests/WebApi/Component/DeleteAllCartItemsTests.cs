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

			AssertExt.ReturnsStatusCode_NoContent(result);
			await AssertExt.ReturnsEmptyContent(result);
		}

		[Theory]
		[AutoData]
		public async Task WhenCartDoesNotExist_ReturnsFailure(Guid id)
		{
			this.repository.DeleteAllCartItems(Arg.Is<Guid>(p => p == id)).Returns(false);

			var result = await this.client.DeleteAsync($"/v1/carts/{id}/items");

			AssertExt.ReturnsStatusCode_NotFound(result);
			await AssertExt.ReturnsEmptyContent(result);
		}

		private void OverrideServices(IServiceCollection services)
		{
			services.AddSingleton(s => this.repository);
		}
	}
}