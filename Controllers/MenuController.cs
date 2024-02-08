using restaurant_demo_website.Models;
using Microsoft.AspNetCore.Mvc;
using restaurant_demo_website.Services;

namespace restaurant_demo_website.Controllers
{
    public class MenuController : Controller
    {
        private readonly IEntitiesRequest _entitiesRequest;
        private static IEnumerable<Product> products { get; set; }

        public MenuController(IEntitiesRequest entitiesRequest)
        {
            _entitiesRequest = entitiesRequest;
        }

        //Return the list of Products for the Restaurant.
        public async Task<IActionResult> IndexAsync()
        {
            
             products = await _entitiesRequest.GetProductsAsync();

                if (products.Any())
                {
                    foreach (var p in products)
                    {
                        p.imgUrl = GetImagesFromByteArray(p.photosUrl);
                    }
                }

            
            return View(products);
        }

        //images are stored in the database as byte. Convert them to base64 string.
        private string GetImagesFromByteArray(byte[]? photosUrl)
        {
            var dataString = Convert.ToBase64String(photosUrl);
            var imgString = string.Format("data:image/png;base64,{0}", dataString);
            return imgString;
        }


        public IActionResult Details(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductID == id);
            if (product == null)
                return View("Error");
            return View(product);
        }
    }
}
