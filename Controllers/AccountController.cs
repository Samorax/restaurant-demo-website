using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using restaurant_demo_website.Models;
using restaurant_demo_website.Services;

namespace restaurant_demo_website.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IEntitiesRequest _entitiesRequest;
        private IMemoryCache _memoryCache;

        public AccountController(IEntitiesRequest entitiesRequest, IMemoryCache memoryCache) 
        {
            _entitiesRequest = entitiesRequest;
            _memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser restaurantinfo = await GetCache();
            bool opened = false;
            ViewData["RestaurantName"] = restaurantinfo.BusinessName;
            var customerId = User.Claims.Single(c => c.Type == "UserId").Value;
            Customer customer = await _entitiesRequest.GetCustomerAsync(customerId);
            _memoryCache.Set(customerId, customer);
            return View(customer);
        }

        public IActionResult Edit()
        {
            var customerId = User.Claims.Single(c => c.Type == "UserId").Value;
            var customer = _memoryCache.Get(customerId);
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Customer customer) 
        {
            var customerId = User.Claims.Single(c => c.Type == "UserId").Value;
            Customer c = (Customer)_memoryCache.Get(customerId);
            customer.Id = customerId;
            customer.ApplicationUserID = c.ApplicationUserID;
            customer.RecordId = c.RecordId;
            await _entitiesRequest.UpdateCustomerAsync(customer);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteAsync()
        {
            var customerId = User.Claims.Single(c => c.Type == "UserId").Value;
            Customer customer = (Customer)_memoryCache.Get(customerId);
            await _entitiesRequest.RemoveCustomerAsync(customer);
             await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
            
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

            return restaurantinfo;
        }
    }
}
