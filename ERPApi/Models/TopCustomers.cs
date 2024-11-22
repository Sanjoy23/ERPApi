using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class TopCustomers
	{
		[Key]
		public string? CustomerName { get; set; }
		public decimal TotalQuantity {  get; set; }
        //public decimal Stock { get; set; }
    }
}
