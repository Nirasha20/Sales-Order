namespace SalesOrderAPI.Domain.Entities
{
    public class SalesOrder
    {
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string ReferenceNo { get; set; } = string.Empty;
        public decimal TotalExcl { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalIncl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}