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
		private readonly WebApplicationFactory<Startup> webApi;
		private readonly ICartRepository repository = Substitute.For<ICartRepository>();

		public GetCartTests(
			WebApplicationFactory<Startup> webApiFixture)
		{
			this.webApi = webApiFixture.Create(
				new Dictionary<string, string>()
				{
				},
				this.OverrideServices);
		}

		[Theory]
		[AutoData]
		public async Task WhenCartExists_ReturnsCart(Guid id, Cart cart)
		{
			this.repository.GetCart(Arg.Is<Guid>(p => p == id)).Returns(cart);
			var requestUrl = $"/v1/carts/{id}";
			using (var client = this.webApi.CreateClient())
			{
				var result = await client.GetAsync(requestUrl);

				Assert.Equal(HttpStatusCode.OK, result.StatusCode);
				(await result.Content.ReadAsAsync<Contracts.Cart>()).Should().BeEquivalentTo(cart);
			}
		}

		private void OverrideServices(IServiceCollection services)
		{
			services.AddSingleton(s => this.repository);
		}
	}
}