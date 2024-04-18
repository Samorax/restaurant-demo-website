
using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        private IMemoryCache _memoryCache;
        

        public HomeController(IEntitiesRequest entitiesRequest, IMemoryCache memoryCache)
        {
            _entitiesRequest = entitiesRequest;
            _memoryCache = memoryCache;
        }

        public async Task<IActionResult> IndexAsync()
        {
            ApplicationUser restaurantinfo = new ApplicationUser();
            if(ShoppingCart.CartSessionKey != null)
            {
                if(!_memoryCache.TryGetValue(ShoppingCart.CartSessionKey, out ApplicationUser u))
                {
                    restaurantinfo = await _entitiesRequest.GetRestaurantInfo();
                    _memoryCache.Set(ShoppingCart.CartSessionKey, restaurantinfo);
                }else{
                    restaurantinfo = u;
                }
                
            }
            ViewData["RestaurantName"] = restaurantinfo.BusinessName;
            return View(restaurantinfo);
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
