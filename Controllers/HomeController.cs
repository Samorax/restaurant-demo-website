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

        public HomeController()
        {
           
            
    
        }

        public IActionResult Index()
        {
            return View();
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
