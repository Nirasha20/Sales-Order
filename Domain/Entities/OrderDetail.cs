namespace SalesOrderAPI.Domain.Entities
{
    public class OrderDetail
    {
        public int DetailId { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public Item? Item { get; set; }
        public string Note { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }
        public decimal ExclAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal InclAmount { get; set; }
    }
}