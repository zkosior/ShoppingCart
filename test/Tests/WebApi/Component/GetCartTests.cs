namespace ShoppingCart.Tests.WebApi.Component
{
	using AutoFixture.Xunit2;
	using FluentAssertions;
	using Microsoft.AspNetCore.Mvc.Testing;
	using Microsoft.Extensions.DependencyInjection;
	using NSubstitute;
	using ShoppingCart.DataAccess.Models;
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
		public async Task WhenCartExists_ReturnsCart(Guid id, Cart cart)
		{
			this.repository.GetCart(Arg.Is<Guid>(p => p == id)).Returns(cart);

			var result = await this.client.GetAsync($"/v1/carts/{id}");

			ReturnsStatusCode_Ok(result);
			await ReturnsCartReturnedFromReporitory(cart, result);
		}

		private static async Task ReturnsCartReturnedFromReporitory(Cart cart, HttpResponseMessage result)
		{
			(await result.Content.ReadAsAsync<Contracts.Cart>()).Should().BeEquivalentTo(cart);
		}

		private static void ReturnsStatusCode_Ok(HttpResponseMessage result)
		{
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		}

		private void OverrideServices(IServiceCollection services)
		{
			services.AddSingleton(s => this.repository);
		}
	}
}