namespace ERPApi.Models
{
	public class OrderDto
	{
		public string CustomerName { get; set; } = string.Empty;
		public decimal Quantity { get; set; }
		public DateTime OrderDate { get; set; }
	}
}
