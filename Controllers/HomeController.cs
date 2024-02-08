using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Mvc;
using restaurant_demo_website.Models;
using restaurant_demo_website.Services;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace restaurant_demo_website.Controllers
{
    public class HomeController : Controller
    {
      
        private IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        private IEntitiesRequest _entitiesRequest;

        public HomeController(ILogger<HomeController> logger,  IConfiguration configuration, IEntitiesRequest entitiesRequest)
        {
           
            _configuration = configuration;
            _logger = logger;
            _entitiesRequest = entitiesRequest;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Menu()
        {
            IEnumerable<Product> products;
            using(var _httpClient = new HttpClient())
            {

                products = await _entitiesRequest.GetProductsAsync();
               
                if(products.Any()) 
                {
                    foreach (var p in products)
                    {
                        p.imgUrl = GetImagesFromByteArray(p.photosUrl);
                    }
                }
                
            }
            return View(products);

        }

        private string GetImagesFromByteArray(byte[] x) 
        {
            var dataString = Convert.ToBase64String(x);
            var imgString = string.Format("data:image/png;base64,{0}", dataString);
            return imgString;

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
