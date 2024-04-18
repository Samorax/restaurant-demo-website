using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using restaurant_demo_website.Models;
using restaurant_demo_website.Services;

namespace restaurant_demo_website.Controllers
{
    public class ContactController : Controller
    {
        private IMemoryCache _memoryCache;
       

        public ContactController(IMemoryCache memoryCache) 
        {
            _memoryCache = memoryCache;

        }

        public IActionResult Index()
        {
            var info = _memoryCache.Get(ShoppingCart.CartSessionKey);
            return View(info);
        }
    }
}
