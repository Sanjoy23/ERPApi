using System.ComponentModel.DataAnnotations.Schema;

namespace ERPApi.DTO
{
	public class ProductDto
	{
		public string ProductName { get; set; } = string.Empty;
		public decimal UnitPrice { get; set; }
		public decimal Stock { get; set; }
	}
}
