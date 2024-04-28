
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
        private IMapService _mapService;

        public HomeController(IEntitiesRequest entitiesRequest, IMemoryCache memoryCache, IMapService mapService)
        {
            _entitiesRequest = entitiesRequest;
            _memoryCache = memoryCache;
            _mapService = mapService;
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

        /// <summary>
        /// This method affirms the possibilty of delivery to Customer's location.
        /// Gets the affirmation by the difference between the chosen delivery mileage by the restaurant and a Calculated mileage
        /// Calculated mileage is done using BingMapsRestToolkit
        /// </summary>
        /// <param name="deliveryPostcode"></param>
        /// <returns></returns>
        public async Task<IActionResult> AffirmDeliveryPossibilityAsync(string deliveryPostcode)
        {
            var Message = string.Empty;
            var info = _memoryCache.Get(ShoppingCart.CartSessionKey) as ApplicationUser;
            var distance = await _mapService.GetDistance(info.PostalCode, deliveryPostcode);
            if (distance > info.DeliveryDistance)
                Message = "Good News!! We deliver to your location";
            else
                Message = "Sorry!! We do not deliver to your location";

            return Json(Message);

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
