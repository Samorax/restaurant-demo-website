using restaurant_demo_website.Models;

namespace restaurant_demo_website.ViewModels
{
    public class MenuDetailsViewModel
    {
        public Product ProductInView { get; set; }
        public List<Product> Recommendations {  get; set; }
    }
}
