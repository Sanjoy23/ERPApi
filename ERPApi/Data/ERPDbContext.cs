using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApp.Models;

namespace WebApp.Data
{
	public class ERPDbContext : DbContext
	{
		public ERPDbContext(DbContextOptions<ERPDbContext> options) : base(options)
		{
		}
		public DbSet<tblProduct> Products { get; set; }
		public DbSet<tblOrder> Orders { get; set; }
		public DbSet<TopCustomers> Customers { get; set; }
		public DbSet<User> Users { get; set; }
	}
}
