using ERPApi.DTO;
using ERPApi.Models;

namespace ERPApi.Services.IService
{
	public interface IOrderServicecs
	{
		Task<ProductDto> FindProductByIDAsync(int productId);
		Task<OrderDto> CreateNewOrderAsync(int productId, string customerName, decimal quantity);
	}
}
