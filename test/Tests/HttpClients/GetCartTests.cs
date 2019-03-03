namespace ShoppingCart.Tests.HttpClients
{
	using AutoFixture.Xunit2;
	using FluentAssertions;
	using Flurl.Http.Testing;
	using Newtonsoft.Json;
	using ShoppingCart.Contracts;
	using ShoppingCart.HttpClients;
	using ShoppingCart.Tests.Helpers;
	using System;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Xunit;

	[Trait("TestCategory", "Unit")]
	public class GetCartTests
	{
		private const string BaseUrl = "http://localhost:8443";
		private readonly ShoppingCartHttpClients configuration;

		public GetCartTests()
		{
			this.configuration = new ShoppingCartHttpClients
			{
				BaseUri = new Uri(BaseUrl),
				HttpTimeout = new TimeSpan(0, 0, 5),
			};
		}

		[Theory]
		[AutoData]
		public async Task WhenSuccessfulyCreated_ReturnsId(Guid id, Cart cart)
		{
			using (var httpTest = new HttpTest())
			{
				httpTest.RespondWith(JsonConvert.SerializeObject(cart), 200);

				var result = await new CartHttpClient(this.configuration)
					.GetCart(id);

				httpTest
					.ShouldHaveCalled($"{BaseUrl}/v1/carts/{id}")
					.WithVerb(HttpMethod.Get)
					.Times(1);
				result.Should().BeEquivalentTo(cart);
			}
		}
	}
}