namespace ShoppingCart.Tests.HttpClients.Extensions
{
	using FluentAssertions;
	using Flurl.Http;
	using Flurl.Http.Testing;
	using ShoppingCart.HttpClients.Exceptions;
	using ShoppingCart.HttpClients.Extensions;
	using System;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Xunit;

	[Trait("TestCategory", "Unit")]
	public class FailureTests
	{
		public FailureTests()
		{
			FlurlHttp.Configure(a => a.ResetDefaults());
		}

		[Fact]
		public async Task WhenTimeout_Throws()
		{
			using (var httpTest = new HttpTest())
			{
				// Arrange
				var baseUri = "http://localhost";
				httpTest.SimulateTimeout();

				// Act
				var response = await Assert.ThrowsAsync<TimeoutException>(
					() => baseUri
					.HandleFailure()
					.GetAsync());

				// Assert
				httpTest.ShouldHaveCalled($"http://localhost")
					.WithVerb(HttpMethod.Get)
					.Times(1);
				response.Message.Should().BeEquivalentTo(
					$"Call to http://localhost/ failed with timeout.");
			}
		}

		[Fact]
		public async Task WhenUnhandledTimeout_Throws()
		{
			using (var httpTest = new HttpTest())
			{
				// Arrange
				var baseUri = "http://localhost";
				httpTest.SimulateTimeout();

				// Act
				var response = await Assert.ThrowsAsync<FlurlHttpTimeoutException>(
					() => baseUri
					.GetAsync());

				// Assert
				httpTest.ShouldHaveCalled($"http://localhost")
					.WithVerb(HttpMethod.Get)
					.Times(1);
				response.Message.Should().BeEquivalentTo(
					$"Call timed out: GET http://localhost");
			}
		}

		[Fact]
		public async Task AllowsEmptyResponse()
		{
			using (var httpTest = new HttpTest())
			{
				// Arrange
				var baseUri = "http://localhost";
				httpTest.RespondWithJson(default, 404);

				// Act
				var response = await baseUri
					.HandleFailure(allowEmptyResponse: true)
					.GetAsync()
					.ReceiveJson();

				// Assert
				httpTest.ShouldHaveCalled($"http://localhost")
					.WithVerb(HttpMethod.Get)
					.Times(1);
				Assert.Null(response);
			}
		}

		[Fact]
		public async Task WithoutHandling_DoesNotAllowEmptyResponse()
		{
			using (var httpTest = new HttpTest())
			{
				// Arrange
				var baseUri = "http://localhost";
				httpTest.RespondWithJson(default, 404);

				// Act
				var response = await Assert.ThrowsAsync<UnexpectedHttpResponseException>(
					() => baseUri
					.HandleFailure(allowEmptyResponse: false)
					.GetAsync()
					.ReceiveJson());

				// Assert
				httpTest.ShouldHaveCalled($"http://localhost")
					.WithVerb(HttpMethod.Get)
					.Times(1);
				response.Message.Should().BeEquivalentTo(
					"Call to http://localhost/ failed with status NotFound and no content.");
			}
		}

		[Fact]
		public async Task IfNotConfigured_DoesNotAllowEmptyResponse()
		{
			using (var httpTest = new HttpTest())
			{
				// Arrange
				var baseUri = "http://localhost";
				httpTest.RespondWithJson(default, 404);

				// Act
				var response = await Assert.ThrowsAsync<FlurlHttpException>(
					() => baseUri
					.GetAsync()
					.ReceiveJson());

				// Assert
				httpTest.ShouldHaveCalled($"http://localhost")
					.WithVerb(HttpMethod.Get)
					.Times(1);
				response.Message.Should().BeEquivalentTo(
					"Call failed with status code 404 (Not Found): GET http://localhost");
			}
		}
	}
}