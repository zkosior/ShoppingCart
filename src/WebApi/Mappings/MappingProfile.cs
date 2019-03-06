namespace ShoppingCart.WebApi.Mappings
{
	using AutoMapper;

	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			this.CreateMap<DataAccess.Models.Cart, Contracts.Cart>()
				.ReverseMap();
			this.CreateMap<DataAccess.Models.Item, Contracts.Item>()
				.ReverseMap();
			this.CreateMap<DataAccess.Models.ItemDetails, Contracts.ItemDetails>()
				.ReverseMap();
		}
	}
}