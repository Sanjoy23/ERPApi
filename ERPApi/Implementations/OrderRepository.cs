using ERPApi.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApp.Data;
using WebApp.Models;

namespace ERPApi.Implementations
{
	public class OrderRepository : IOrderRepository
	{
		private readonly ERPDbContext _context;

		public OrderRepository(ERPDbContext context)
		{
			_context = context;
		}

		public void AddOrder(tblOrder order)
		{
			_context.Orders.Add(order);
		}

		public async void DeleteOrderByIDAsync(int orderId)
		{
			throw new NotImplementedException();
		}

		public tblOrder FindOrderByID(int orderId)
		{
			return _context.Orders.Find(orderId);
		}

		public tblProduct FindProductByIDAsync(int productId)
		{
			return _context.Products.Find(productId);
		}

		public IEnumerable<tblOrder> GetAllOrdersWithDetailsAsync()
		{
			throw new NotImplementedException();
		}

		public void SaveOrder()
		{
			_context.SaveChanges();
		}
	}
}
