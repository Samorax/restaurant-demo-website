namespace restaurant_demo_website.Models
{
    public class PaymentObject
    {
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string Description { get; set; }
        public int OrderId { get; set; }
    }
}