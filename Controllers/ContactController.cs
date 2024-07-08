using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using restaurant_demo_website.Models;
using restaurant_demo_website.Services;
using restaurant_demo_website.ViewModels;

namespace restaurant_demo_website.Controllers
{
    public class ContactController : Controller
    {
        private IMemoryCache _memoryCache;
        private IEntitiesRequest _entitiesRequest;
        private IMapService _mapService;

        public ContactController(IEntitiesRequest entitiesRequest,IMemoryCache memoryCache, IMapService mapService) 
        {
            _memoryCache = memoryCache;
            _entitiesRequest = entitiesRequest;
            _mapService = mapService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            ApplicationUser restaurantinfo = await GetCache();
            await _mapService.GetAddressImage(string.Concat(restaurantinfo.BusinessAddress1,"",restaurantinfo.BusinessAddress2),restaurantinfo.State,restaurantinfo.PostalCode,restaurantinfo.Country);
            var viewmodel = new ContactViewModel {info = restaurantinfo };
            return View(viewmodel);
        }

        private async Task<ApplicationUser> GetCache()
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
            return restaurantinfo;
        }
    }
}
