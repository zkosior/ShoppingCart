namespace ShoppingCart.WebApi.Services
{
	using AutoMapper;
	using ShoppingCart.Contracts;
	using ShoppingCart.DataAccess.Repositories;
	using System;
	using System.Threading.Tasks;

	public class CartService
	{
		private readonly ICartRepository repository;
		private readonly IMapper mapper;

		public CartService(ICartRepository repository, IMapper mapper)
		{
			this.repository = repository;
			this.mapper = mapper;
		}

		public Task<Guid> CreateCart()
		{
			return this.repository.CreateCart();
		}

		public async Task<Cart> GetCart(Guid id)
		{
			var cart = await this.repository.GetCart(id);
			return this.mapper.Map<Cart>(cart);
		}

		public Task<bool> DeleteCart(Guid id)
		{
			return this.repository.DeleteCart(id);
		}
	}
}