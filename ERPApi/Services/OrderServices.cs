using ERPApi.DTO;
using ERPApi.Repository;
using ERPApi.Services.IService;
using WebApp.Models;

namespace ERPApi.Services
{
	public class OrderServices : IOrderServicecs
	{
		private readonly IOrderRepository _repo;

		public OrderServices(IOrderRepository repo)
		{
			_repo = repo;
		}

		public async Task<ProductDto> FindProductByIDAsync(int productId)
		{
			tblProduct product = await _repo.FindProductByIDAsync(productId);
			if(product == null) return new ProductDto();

			return new ProductDto
			{
				ProductName = product.strProductName,
				UnitPrice = product.numUnitPrice,
				Stock = product.numStock
			};

		}
	}
}
