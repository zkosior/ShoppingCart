namespace ShoppingCart.Tests.WebApi.Mapping
{
	using AutoMapper;
	using ShoppingCart.WebApi.Mappings;
	using Xunit;

	[Trait("TestCategory", "Unit")]
	public class ProfileTests
	{
		[Fact]
		public void ProfilesAreComplete()
		{
			CreateMapper()
				.ConfigurationProvider
				.AssertConfigurationIsValid();
		}

		private static IMapper CreateMapper()
		{
			return new MapperConfiguration(
				mc => mc.AddProfile(new MappingProfile()))
				.CreateMapper();
		}
	}
}