namespace ShoppingCart.Tests.HttpClients
{
	using AutoFixture.Xunit2;
	using FluentAssertions;
	using Flurl.Http.Testing;
	using ShoppingCart.HttpClients;
	using ShoppingCart.Tests.Helpers;
	using System;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Xunit;

	[Trait("TestCategory", "Unit")]
	public class ClearCartItemsTests
	{
		private const string BaseUrl = "http://localhost:8443";
		private readonly ShoppingCartHttpClients configuration;

		public ClearCartItemsTests()
		{
			this.configuration = new ShoppingCartHttpClients
			{
				BaseUri = new Uri(BaseUrl),
				HttpTimeout = new TimeSpan(0, 0, 5),
			};
		}

		[Theory]
		[AutoData]
		public async Task WhenExists_ReturnsSuccess(Guid id)
		{
			using (var httpTest = new HttpTest())
			{
				httpTest.RespondWith(string.Empty, 204);

				var result = await new CartHttpClient(this.configuration)
					.ClearCartItems(id);

				httpTest
					.ShouldHaveCalled($"{BaseUrl}/v1/carts/{id}/items")
					.WithVerb(HttpMethod.Delete)
					.Times(1);
				result.Should().BeTrue();
			}
		}

		[Theory]
		[AutoData]
		public async Task WhenDosNotExist_ReturnsFailure(Guid id)
		{
			using (var httpTest = new HttpTest())
			{
				httpTest.RespondWith(string.Empty, 404);

				var result = await new CartHttpClient(this.configuration)
					.ClearCartItems(id);

				httpTest
					.ShouldHaveCalled($"{BaseUrl}/v1/carts/{id}/items")
					.WithVerb(HttpMethod.Delete)
					.Times(1);
				result.Should().BeFalse();
			}
		}
	}
}