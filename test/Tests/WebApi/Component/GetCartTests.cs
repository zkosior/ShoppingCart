namespace ShoppingCart.Tests.WebApi.Component
{
	using AutoFixture.Xunit2;
	using Microsoft.AspNetCore.Mvc.Testing;
	using Microsoft.Extensions.DependencyInjection;
	using NSubstitute;
	using ShoppingCart.DataAccess.Models;
	using ShoppingCart.DataAccess.Repositories;
	using ShoppingCart.Tests.Helpers;
	using ShoppingCart.WebApi;
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Xunit;

	[Trait("TestCategory", "Component")]
	public class GetCartTests
		: IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly HttpClient client;
		private readonly ICartRepository repository = Substitute.For<ICartRepository>();

		public GetCartTests(
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
		public async Task WhenCartExists_ReturnsCart(Cart cart)
		{
			this.repository.GetCart(Arg.Is<Guid>(p => p == cart.Id)).Returns(cart);

			var result = await this.client.GetAsync($"/v1/carts/{cart.Id}");

			AssertExt.ReturnsStatusCode_Ok(result);
			await AssertExt.ReturnsCartReturnedFromRepository(cart, result);
		}

		[Theory]
		[InlineAutoData(default)]
		public async Task WhenCartDoesNotExist_ReturnsNotFound(Cart newCart, Guid id)
		{
			this.repository.GetCart(Arg.Any<Guid>()).Returns(newCart);

			var result = await this.client.GetAsync($"/v1/carts/{id}");

			AssertExt.ReturnsStatusCode_NotFound(result);
			await AssertExt.ReturnsEmptyContent(result);
		}

		private void OverrideServices(IServiceCollection services)
		{
			services.AddSingleton(s => this.repository);
		}
	}
}