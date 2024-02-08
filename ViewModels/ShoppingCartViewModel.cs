using System.Collections.Generic;
using restaurant_demo_website.Models;

namespace restaurant_demo_website.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<CartOrder> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}