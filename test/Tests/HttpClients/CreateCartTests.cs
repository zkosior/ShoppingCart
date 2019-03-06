namespace ShoppingCart.Tests.HttpClients
{
	using AutoFixture.Xunit2;
	using FluentAssertions;
	using Flurl.Http.Testing;
	using Newtonsoft.Json;
	using ShoppingCart.HttpClients;
	using ShoppingCart.Tests.Helpers;
	using System;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Xunit;

	[Trait("TestCategory", "Unit")]
	public class CreateCartTests
	{
		private const string BaseUrl = "http://localhost:8443";
		private readonly ShoppingCartHttpClients configuration;

		public CreateCartTests()
		{
			this.configuration = new ShoppingCartHttpClients
			{
				BaseUri = new Uri(BaseUrl),
				HttpTimeout = new TimeSpan(0, 0, 5),
			};
		}

		[Theory]
		[AutoData]
		public async Task WhenSuccessfulyCreated_ReturnsId(Guid id)
		{
			using (var httpTest = new HttpTest())
			{
				httpTest.RespondWith(
					JsonConvert.SerializeObject(id),
					(int)HttpStatusCode.Created);

				var result = await new CartHttpClient(this.configuration)
					.CreateCart();

				httpTest
					.ShouldHaveCalled($"{BaseUrl}/v1/cart")
					.WithVerb(HttpMethod.Post)
					.Times(1);
				result.Should().Be(id);
			}
		}
	}
}