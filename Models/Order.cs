using restaurant_demo_website.Models;

namespace FoodloyaleApi.Models
{
    public class Order
    {
        public Guid ApplicationUserID { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    
        public decimal TotalAmount { get; set; }
        public int OrderID { get; set; }

        public string OrderStatus { get; set; }
        public string Channel {get;set;}

        public bool Opened {get;set;} = false;
        public string PaymentToken{get;set;}
        public DateTime OrderDate {get;set;}
        public string Payment {get;set;}

        public string DeliveryAddress { get; set; }

        public string? CustomerID {get;set;}
        public string? Username { get; set; }

    }
}
