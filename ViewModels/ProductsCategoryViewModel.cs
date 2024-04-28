using restaurant_demo_website.Models;

namespace restaurant_demo_website.ViewModels
{
    public class ProductsCategoryViewModel
    {
        public required IEnumerable<Product> Products { get; set; }
        public required List<String> Categories { get; set; }
        public required string CurrentCategory { get; set; }
        public bool ShopOpened { get; internal set; }
        public string CultureName { get; internal set; }
    }
}
