namespace ShoppingCart.Tests.HttpClients.Configuration
{
	using AutoFixture.Xunit2;
	using FluentAssertions;
	using Flurl.Http;
	using Flurl.Http.Testing;
	using ShoppingCart.HttpClients.Configuration;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;
	using Xunit;

	[Trait("TestCategory", "Unit")]
	public class HttpClientConfigurationTests
	{
		public HttpClientConfigurationTests()
		{
			FlurlHttp.Configure(a => a.ResetDefaults());
		}

		[Fact]
		public void FlurlHttpGlobalSettings_AreConfigured()
		{
			// Arrange
			var headers = new Dictionary<string, string>();
			var testObject = new { Property = "value" };

			// Initial Assert
			FlurlHttp.GlobalSettings.BeforeCall.Should().BeNull();
			var initialSerializedTestObject = FlurlHttp.GlobalSettings
				.JsonSerializer.Serialize(testObject);
			initialSerializedTestObject.Should().Be("{\"Property\":\"value\"}");

			// Act
			HttpClientConfiguration.ConfigureHttpClients(() => headers);

			// Final Assert
			FlurlHttp.GlobalSettings.BeforeCall.Should().NotBeNull();
			var serializedTestObject = FlurlHttp.GlobalSettings
				.JsonSerializer.Serialize(testObject);
			serializedTestObject.Should().Be("{\"property\":\"value\"}");
		}

		[Theory]
		[AutoData]
		public void SetConnectionTimeout_ConfiguresConnectionLeaseTimeout(
			Uri uri,
			int timeout)
		{
			// Arrange

			// Act
			HttpClientConfiguration.SetConnectionTimeout(uri, timeout);

			// Assert
			var sp = ServicePointManager.FindServicePoint(uri);
			sp.ConnectionLeaseTimeout.Should().Be(timeout * 1000);
		}

		[Theory]
		[AutoData]
		public async Task CorrelationId_IsUsedInRequestHeader(string correlationId)
		{
			using (var httpTest = new HttpTest())
			{
				// Arrange
				var baseUri = "http://localhost";
				httpTest.RespondWithJson("{}");
				var headers = new Dictionary<string, string>
				{
					{ "X-Correlation-Id", correlationId }
				};

				HttpClientConfiguration.ConfigureHttpClients(() => headers);

				// Act
				await baseUri
					.GetAsync();

				// Assert
				httpTest.CallLog.Single().Request.Headers.Single()
					.Key.Should().Be("X-Correlation-Id");
				httpTest.CallLog.Single().Request.Headers.Single()
					.Value.Single().Should().Be(correlationId);
			}
		}

		[Fact]
		public async Task CorrelationId_IsNotSpecifiedInRequest_Header()
		{
			using (var httpTest = new HttpTest())
			{
				// Arrange
				var baseUri = "http://localhost";
				httpTest.RespondWithJson("{}");
				var headers = new Dictionary<string, string>();

				HttpClientConfiguration.ConfigureHttpClients(() => headers);

				// Act
				await baseUri
					.GetAsync();

				// Assert
				httpTest.CallLog.Single().Request.Headers.Should().NotContain(
					x => x.Key.Equals("X-Correlation-Id", StringComparison.OrdinalIgnoreCase));
			}
		}
	}
}