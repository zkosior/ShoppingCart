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
	public class UpdateCartItemQuantityTests
	{
		private const string BaseUrl = "http://localhost:8443";
		private readonly ShoppingCartHttpClients configuration;

		public UpdateCartItemQuantityTests()
		{
			this.configuration = new ShoppingCartHttpClients
			{
				BaseUri = new Uri(BaseUrl),
				HttpTimeout = new TimeSpan(0, 0, 5),
			};
		}

		[Theory]
		[AutoData]
		public async Task WhenExists_ReturnsSuccess(
			Guid cartId,
			Guid itemId,
			uint quantity)
		{
			if (quantity == 0)
			{
				quantity = 100; // just to make it work
			}

			using (var httpTest = new HttpTest())
			{
				httpTest.RespondWith(string.Empty, 204);

				var result = await new CartHttpClient(this.configuration)
					.UpdateCartItemQuantity(cartId, itemId, (int)quantity);

				httpTest
					.ShouldHaveCalled(
						$"{BaseUrl}/v1/carts/{cartId}/items/{itemId}/quantity/{quantity}")
					.WithVerb(HttpMethod.Put)
					.Times(1);
				result.Should().BeTrue();
			}
		}

		[Theory]
		[AutoData]
		public async Task WhenDosNotExist_ReturnsFailure(
			Guid cartId,
			Guid itemId,
			uint quantity)
		{
			if (quantity == 0)
			{
				quantity = 100; // just to make it work
			}

			using (var httpTest = new HttpTest())
			{
				httpTest.RespondWith(string.Empty, 404);

				var result = await new CartHttpClient(this.configuration)
					.UpdateCartItemQuantity(cartId, itemId, (int)quantity);

				httpTest
					.ShouldHaveCalled(
						$"{BaseUrl}/v1/carts/{cartId}/items/{itemId}/quantity/{quantity}")
					.WithVerb(HttpMethod.Put)
					.Times(1);
				result.Should().BeFalse();
			}
		}

		[Theory]
		[InlineAutoData(0)]
		[InlineAutoData(-1)]
		public async Task WhenQuantityIsIncorrect_ReturnsFailure(
			int quantity,
			Guid cartId,
			Guid itemId)
		{
			var result = await Assert.ThrowsAsync<ArgumentException>(
				() => new CartHttpClient(this.configuration)
				.UpdateCartItemQuantity(cartId, itemId, quantity));

			result.Message.Should().Be("Parameter 'quantity' must be at least 1.");
		}
	}
}