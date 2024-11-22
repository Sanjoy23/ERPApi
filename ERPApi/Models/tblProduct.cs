using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class tblProduct
    {
		[Key]
		public int intProductId { get; set; }
		public string strProductName { get; set; } = string.Empty;
		public decimal numUnitPrice { get; set; }
		public decimal numStock { get; set; }
	}
}
