using ERPApi.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
	public class ProductsController : Controller
	{
		private readonly ERPDbContext _context;
		private readonly OrderRepository _orderRepo;

		public ProductsController(ERPDbContext context, OrderRepository orderRepo)
		{
			_context = context;
			_orderRepo = orderRepo;
		}

		//API 01: Create new Order
		[HttpPost("CreateOrder")]
		public async Task<IActionResult> CreateOrder(int productId, string customerName, decimal quantity)
		{
			//var product = await _context.Products.FindAsync(productId);
			var product = _orderRepo.FindProductByIDAsync(productId);
			if (product == null) return NotFound("Product not found.");

			if (product.numStock < quantity)
				return BadRequest("Insufficient stock.");

			product.numStock -= quantity;

			var order = new tblOrder
			{
				intProductId = productId,
				strCustomerName = customerName,
				numQuantity = quantity,
				dtOrderDate = DateTime.Now
			};

			//_context.Orders.Add(order);
			_orderRepo.AddOrder(order);
			_orderRepo.SaveOrder();

			return Ok("Order created successfully.");
		}

		//API 02: Update an Order
		[HttpPut("UpdateOrder")]
		public async Task<IActionResult> UpdateOrder(int orderId, decimal newQuantity)
		{
			//var order = await _context.Orders.FirstOrDefaultAsync(o => o.intOrderId == orderId);
			var order = _orderRepo.FindOrderByID(orderId);
			if (order == null) return NotFound("Order not found.");

			var difference = newQuantity - order.numQuantity;
			var product = _orderRepo.FindProductByIDAsync(order.intProductId);

			if (product.numStock < difference)
				return BadRequest("Insufficient stock.");

			product.numStock -= difference;
			order.numQuantity = newQuantity;

			_orderRepo.SaveOrder();
			return Ok("Order updated successfully.");
		}

		//API 03: Delete an Order
		[HttpDelete("DeleteOrder")]
		public async Task<IActionResult> DeleteOrder(int orderId)
		{
			var order = await _context.Orders.FirstOrDefaultAsync(o => o.intOrderId == orderId);
			if (order == null) return NotFound("Order not found.");

			var product = await _context.Products.FirstOrDefaultAsync(o => o.intProductId == order.intProductId);
			product.numStock += order.numQuantity;

			_context.Orders.Remove(order);
			await _context.SaveChangesAsync();
			return Ok("Order deleted successfully.");
		}

		//API 04: Oders with details
		[HttpGet("GetOrdersWithDetails")]
		public async Task<IActionResult> GetOrdersWithDetails()
		{
			var orders = await _context.Orders
				.Join(_context.Products, order => order.intProductId, product => product.intProductId,
				(order, product) => new {
					order.intProductId,
					order.strCustomerName,
					order.numQuantity,
					order.dtOrderDate,
					product.strProductName,
					product.numUnitPrice
				}).ToListAsync();


			return Ok(orders);
		}

		// API 05: Product summary
		[HttpGet("GetProductSummary")]
		public async Task<IActionResult> GetProductSummary()
		{

			/*
			SELECT p.strProductName, SUM(o.numQuantity) AS TotalQuantity, SUM(o.numQuantity * p.numUnitPrice) AS TotalRevenue
			FROM tblOrders o INNER JOIN tblProducts p ON o.intProductId = p.intProductId
			GROUP BY p.strProductName, p.numUnitPrice;*/

			var summary = await _context.Orders
						.Join(
							_context.Products,             
							order => order.intProductId,    
							product => product.intProductId,
							(order, product) => new 
							{
								product.strProductName,
								unitPrice = (double)product.numUnitPrice,
								quantity = (double)order.numQuantity
							})
							.GroupBy(
								x => new { x.strProductName, x.unitPrice }
							)
							.Select(g => new
							{
								g.Key.strProductName,
								TotalQuantity = g.Sum(x => x.quantity),
								TotalRevenue = g.Sum(x => x.quantity * g.Key.unitPrice)
							})
							.ToListAsync();

			return Ok(summary);
		}


		// API 06: findig prouducts with lower Stock
		[HttpGet("GetLowStockProducts")]
		public async Task<IActionResult> GetLowStockProducts(decimal threshold)
		{
			var products = await _context.Products
				.Where(p => p.numStock < threshold)
				.Select(p => new
				{
					p.strProductName,
					p.numUnitPrice,
					p.numStock
				}).ToListAsync();

			return Ok(products);
		}

		//API 07: Customers who order maximum 
		[HttpGet("GetTopCustomers")]
		public async Task<List<TopCustomers>> GetTopCustomers()
		{
			var query = @" SELECT strCustomerName AS CustomerName, SUM(numQuantity) AS TotalQuantity 
						FROM Orders 
						GROUP BY strCustomerName 
						ORDER BY TotalQuantity DESC LIMIT 3";

		//var sqlParams = new SqliteParameter[] { };

			return await _context.Customers.FromSqlRaw(query)
				.ToListAsync();
		}


		//API 08: Product which get no order
		[HttpGet("GetUnorderedProducts")]
		public async Task<IActionResult> GetUnorderedProducts()
		{
			var products = await _context.Products
				.Where(p => !_context.Orders.Any(o => o.intProductId == p.intProductId))
				.Select(p => new
				{
					p.strProductName,
					p.numUnitPrice,
					p.numStock
				}).ToListAsync();

			return Ok(products);
		}

		// API 09: Transactional  Bulk order creation
		[HttpPost("BulkCreateOrders")]
		public async Task<IActionResult> BulkCreateOrders(List<tblOrder> orders)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				foreach (var order in orders)
				{
					var product = await _context.Products.FindAsync(order.intProductId);
					if (product == null || product.numStock < order.numQuantity)
					{
						await transaction.RollbackAsync();
						return BadRequest("Insufficient stock or invalid product.");
					}

					product.numStock -= order.numQuantity;
					order.dtOrderDate = DateTime.Now;
					_context.Orders.Add(order);
				}

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();
				return Ok("Bulk orders created successfully.");
			}
			catch
			{
				await transaction.RollbackAsync();
				return BadRequest("Bulk order creation failed.");
			}
		}
	}
}
