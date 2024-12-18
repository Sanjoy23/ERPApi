using ERPApi.Repository;
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

		public async Task<tblProduct> FindProductByIDAsync(int productId)
		{
			var product = await _context.Products.FindAsync(productId);
			return product ?? new tblProduct();
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
