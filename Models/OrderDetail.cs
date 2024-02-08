namespace restaurant_demo_website.Models
{
    public class OrderDetail
    {
        public int ProductId { get; internal set; }
        public int OrderId { get; internal set; }
        public decimal UnitPrice { get; internal set; }
        public int Quantity { get; internal set; }
    }
}