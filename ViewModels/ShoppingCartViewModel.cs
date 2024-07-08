using System.Collections.Generic;
using restaurant_demo_website.Models;

namespace restaurant_demo_website.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<CartOrder> CartItems { get; set; }
        public decimal CartTotal { get; set; }
        public decimal DeliveryFee { get; set;}
        public decimal VAT {  get; set;}

        public string CultureName { get; set; }
    }
}