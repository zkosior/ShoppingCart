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
	public class AddCartItemTests
	{
		private const string BaseUrl = "http://localhost:8443";
		private readonly ShoppingCartHttpClients configuration;

		public AddCartItemTests()
		{
			this.configuration = new ShoppingCartHttpClients
			{
				BaseUri = new Uri(BaseUrl),
				HttpTimeout = new TimeSpan(0, 0, 5),
			};
		}

		[Theory]
		[AutoData]
		public async Task WhenCartExistsAndRequestIsCorrect_ReturnsSuccess(
			Guid cartId,
			Item item,
			Guid newItemId)
		{
			using (var httpTest = new HttpTest())
			{
				httpTest.RespondWith(JsonConvert.SerializeObject(newItemId), 201);

				var result = await new CartHttpClient(this.configuration)
					.AddCartItem(cartId, item);

				httpTest
					.ShouldHaveCalled($"{BaseUrl}/v1/carts/{cartId}/items")
					.WithVerb(HttpMethod.Post)
					.WithRequestJson(item)
					.Times(1);
				result.Should().Be(newItemId);
			}
		}

		[Theory]
		[AutoData]
		public async Task WhenCartDosNotExist_ReturnsFailure(
			Guid cartId,
			Item item)
		{
			using (var httpTest = new HttpTest())
			{
				httpTest.RespondWith(string.Empty, 404);

				var result = await Assert.ThrowsAsync<ArgumentException>(
					() => new CartHttpClient(this.configuration)
					.AddCartItem(cartId, item));

				httpTest
					.ShouldHaveCalled($"{BaseUrl}/v1/carts/{cartId}/items")
					.WithVerb(HttpMethod.Post)
					.WithRequestJson(item)
					.Times(1);
				result.Message.Should().Be($"Cart with id '{cartId}' was not found.");
			}
		}
	}
}