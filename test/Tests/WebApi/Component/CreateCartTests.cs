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
		private readonly WebApplicationFactory<Startup> webApi;
		private readonly ICartRepository repository = Substitute.For<ICartRepository>();

		public CreateCartTests(
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
		public async Task WhenSuccessfulyCreated_ReturnsId(Guid id)
		{
			this.repository.CreateCart().Returns(id);
			var requestUrl = "/v1/cart";
			using (var client = this.webApi.CreateClient())
			{
				var result = await client.PostAsJsonAsync(requestUrl, default(object));

				Assert.Equal(HttpStatusCode.OK, result.StatusCode);
				(await result.Content.ReadAsAsync<Guid>()).Should().Be(id);
			}
		}

		private void OverrideServices(IServiceCollection services)
		{
			services.AddSingleton(s => this.repository);
		}
	}
}