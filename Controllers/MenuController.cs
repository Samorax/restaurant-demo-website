using restaurant_demo_website.Models;
using Microsoft.AspNetCore.Mvc;
using restaurant_demo_website.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using restaurant_demo_website.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using FoodloyaleApi.Models;

namespace restaurant_demo_website.Controllers
{
    public class MenuController : Controller
    {
        private readonly IEntitiesRequest _entitiesRequest;
        private IMemoryCache _memoryCache;

        private static IEnumerable<Product> products { get; set; }

        public MenuController(IEntitiesRequest entitiesRequest, IMemoryCache memoryCache)
        {
            _entitiesRequest = entitiesRequest;
            _memoryCache = memoryCache;
        }

        
        /// <summary>
        /// Get the list of Produce by the restaurant and all Categories
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> IndexAsync()
        {
            ApplicationUser restaurantinfo = new ApplicationUser();
            if (ShoppingCart.CartSessionKey != null)
            {
                if (!_memoryCache.TryGetValue(ShoppingCart.CartSessionKey, out ApplicationUser u))
                {
                    restaurantinfo = await _entitiesRequest.GetRestaurantInfo();
                    _memoryCache.Set(ShoppingCart.CartSessionKey, restaurantinfo);
                }
                else
                {
                    restaurantinfo = u;
                }

            }
            ViewData["RestaurantName"] = restaurantinfo.BusinessName;
            products = await _entitiesRequest.GetProductsAsync();
            List<string> categories = new List<string>();

                if (products.Any())
                {
                    foreach (var p in products)
                    {
                        p.imgUrl = GetImagesFromByteArray(p.photosUrl);
                        categories.Add(p.Category);
                    }
                    categories = categories.Distinct().ToList();
                }

            var ProductCategoryPage = new ProductsCategoryViewModel
            {
                Products = products,
                Categories = categories,
                CurrentCategory = "All"
            };

            
            return View(ProductCategoryPage);
        }

        //images are stored in the database as byte. Convert them to base64 string.
        private string GetImagesFromByteArray(byte[]? photosUrl)
        {
            var dataString = Convert.ToBase64String(photosUrl);
            var imgString = string.Format("data:image/png;base64,{0}", dataString);
            return imgString;
        }


        public async Task<IActionResult> DetailsAsync(int id)
        {
            products = await _entitiesRequest.GetProductsAsync();
            Product product = products.FirstOrDefault(p => p.ProductID == id);
            product.imgUrl = GetImagesFromByteArray(product.photosUrl);
            if (!string.IsNullOrEmpty(product.Allergens))
            {
                product.DiffAllergen = product.Allergens.Split(",");
            }
                    
              
            if (product == null)
                return View("Error");
            return View(product);
        }

        

        /// <summary>
        /// Gets the list of products for a category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns>Products for a Category</returns>
        public async Task<IActionResult> Category(string categoryName)
        {
            products = await _entitiesRequest.GetProductsAsync();
            List<string> categories = new List<string>();
            var CategoryProducts = products.Where(p => p.Category.Equals(categoryName)).ToList();
            if (CategoryProducts.Any())
            {
                
                foreach (var p in products)
                {
                    p.imgUrl = GetImagesFromByteArray(p.photosUrl);
                    
                    categories.Add(p.Category);
                }
                categories = categories.Distinct().ToList();
            }

            var ProductCategoryPage = new ProductsCategoryViewModel
            {
                Products = CategoryProducts,
                Categories = categories,
                CurrentCategory = categoryName
            };


            return View(ProductCategoryPage);

        }
    }
}
