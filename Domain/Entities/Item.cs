namespace SalesOrderAPI.Domain.Entities
{
    public class Item
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}