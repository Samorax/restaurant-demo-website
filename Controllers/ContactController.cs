using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using restaurant_demo_website.Models;
using restaurant_demo_website.Services;

namespace restaurant_demo_website.Controllers
{
    public class ContactController : Controller
    {
        private IMemoryCache _memoryCache;
        private IEntitiesRequest _entitiesRequest;

        public ContactController(IEntitiesRequest entitiesRequest,IMemoryCache memoryCache) 
        {
            _memoryCache = memoryCache;
            _entitiesRequest = entitiesRequest;
        }

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
            var info = _memoryCache.Get(ShoppingCart.CartSessionKey);
            return View(info);
        }
    }
}
