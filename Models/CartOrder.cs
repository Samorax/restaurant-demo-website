using FoodloyaleApi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace restaurant_demo_website.Models
{
    public class CartOrder
    {
        public int RecordId { get;set; }
        public string CartOrderId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }
        public Product Product { get; set; }

        public Guid ApplicationUserID {get;set;}
    }
}
