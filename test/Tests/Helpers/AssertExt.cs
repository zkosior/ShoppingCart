namespace ShoppingCart.Tests.Helpers
{
	using FluentAssertions;
	using ShoppingCart.DataAccess.Models;
	using System;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Xunit;

	public static class AssertExt
	{
		public static void ReturnsStatusCode_Ok(
			HttpResponseMessage result)
		{
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		}

		public static void ReturnsStatusCode_NotFound(
			HttpResponseMessage result)
		{
			Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
		}

		public static void ReturnsStatusCode_NoContent(HttpResponseMessage result)
		{
			Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
		}

		public static void ReturnsStatusCode_BadRequest(HttpResponseMessage result)
		{
			Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
		}

		public static void ReturnsStatusCode_Created(HttpResponseMessage result)
		{
			Assert.Equal(HttpStatusCode.Created, result.StatusCode);
		}

		public static async Task ReturnsEmptyContent(
			HttpResponseMessage result)
		{
			(await result.Content.ReadAsStringAsync()).Should().BeEmpty();
		}

		public static async Task ReturnsCartReturnedFromReporitory(
			Cart cart,
			HttpResponseMessage result)
		{
			(await result.Content.ReadAsAsync<Contracts.Cart>())
				.Should().BeEquivalentTo(cart);
		}

		public static async Task ReturnsIdOfCreatedCart(Guid id, HttpResponseMessage result)
		{
			(await result.Content.ReadAsAsync<Guid>()).Should().Be(id);
		}

		public static async Task ReturnsIdOfCreatedItem(Guid id, HttpResponseMessage result)
		{
			(await result.Content.ReadAsAsync<Guid>()).Should().Be(id);
		}
	}
}