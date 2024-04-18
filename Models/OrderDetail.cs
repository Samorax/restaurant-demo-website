namespace restaurant_demo_website.Models
{
    public class OrderDetail
    {
        public string Name {get;set;}
        public int OrderId { get; internal set; }
        public decimal UnitPrice { get; internal set; }
        public int Quantity { get; internal set; }
    }
}