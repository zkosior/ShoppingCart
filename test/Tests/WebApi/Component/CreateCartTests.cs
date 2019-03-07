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
	public class CreateCartTests
		: IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly HttpClient client;
		private readonly ICartRepository repository = Substitute.For<ICartRepository>();

		public CreateCartTests(
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
		public async Task WhenSuccessfullyCreated_ReturnsId(Guid id)
		{
			// Given
			this.repository.CreateCart().Returns(id);

			// When
			var result = await this.client.PostAsJsonAsync(
				"/v1/carts",
				default(object));

			// Then
			AssertExt.ReturnsStatusCode_Created(result);
			await AssertExt.ReturnsIdOfCreatedCart(id, result);
		}

		private void OverrideServices(IServiceCollection services)
		{
			services.AddSingleton(s => this.repository);
		}
	}
}