using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace ERPApi.Repository
{
	public interface IOrderRepository
	{
	    Task<tblProduct> FindProductByIDAsync(int productId);
		tblOrder FindOrderByID(int orderId);
		void DeleteOrderByIDAsync(int orderId);
		void AddOrder(tblOrder order);
		void SaveOrder();
		IEnumerable<tblOrder> GetAllOrdersWithDetailsAsync();
	}
}
