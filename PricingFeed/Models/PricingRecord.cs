namespace PricingFeed.Models
{
    public class PricingRecord
    {
        public int Id { get; set; }
        public string StoreId { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
    }
}
