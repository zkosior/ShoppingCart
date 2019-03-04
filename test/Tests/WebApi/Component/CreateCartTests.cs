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
		public async Task WhenSuccessfulyCreated_ReturnsId(Guid id)
		{
			// Given
			this.repository.CreateCart().Returns(id);

			// When
			var result = await this.client.PostAsJsonAsync(
				"/v1/carts",
				default(object));

			// Then
			ReturnsStatusCode_Created(result);
			await ReturnsIdOfCreatedCart(id, result);
		}

		private static async Task ReturnsIdOfCreatedCart(Guid id, HttpResponseMessage result)
		{
			(await result.Content.ReadAsAsync<Guid>()).Should().Be(id);
		}

		private static void ReturnsStatusCode_Created(HttpResponseMessage result)
		{
			Assert.Equal(HttpStatusCode.Created, result.StatusCode);
		}

		private void OverrideServices(IServiceCollection services)
		{
			services.AddSingleton(s => this.repository);
		}
	}
}