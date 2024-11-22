using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class tblOrder
	{
		[Key]
		public int intOrderId { get; set; }

		[ForeignKey("tblProduct")]
		public int intProductId { get; set; }
		public string strCustomerName { get; set; } = string.Empty;
		public decimal numQuantity { get; set; }
		public DateTime dtOrderDate { get; set; }
	}
}
