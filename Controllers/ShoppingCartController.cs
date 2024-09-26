using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using restaurant_demo_website.Models;
using restaurant_demo_website.Services;
using restaurant_demo_website.ViewModels;


namespace restaurant_demo_website.Controllers
{
    public class ShoppingCartController : Controller
    {
        private IEntitiesRequest _entitiesRequest;
        private ShoppingCart _shoppingCart;
        private IMemoryCache _memoryCache;

        public ShoppingCartController(IEntitiesRequest entitiesRequest, ShoppingCart shoppingCart, IMemoryCache memoryCache) 
        {
            _entitiesRequest = entitiesRequest;
            _shoppingCart = shoppingCart;
            _memoryCache = memoryCache;
        }
        
        //
        // GET: /ShoppingCart/
        public async Task<ActionResult> IndexAsync()
        {
            //var cart = ShoppingCart.GetCart(HttpContext);

            ApplicationUser restaurantinfo = await GetCache();
            bool opened = false;
            ViewData["RestaurantName"] = restaurantinfo.BusinessName;

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = await _shoppingCart.GetCartItemsAsync(),
                CartTotal = await _shoppingCart.GetTotalAsync(),
                CultureName = "en-GB"
            };
            // Return the view
            return View(viewModel);
        }
        //
        // GET: /Store/AddToCart/5
        public async Task<ActionResult> AddToCart(int id)
        {
            // Retrieve the album from the database
            var products = await _entitiesRequest.GetProductsAsync();
            if(products.Any())
            {
                var addedProduct = products.FirstOrDefault(p => p.ProductID == id);
                // Add it to the shopping cart
                if(ShoppingCart.ShoppingCartId == null){
                    _shoppingCart.GetCart(this.HttpContext);
                }
                
                await _shoppingCart.AddToCart(addedProduct);
                
            }


            // Go back to the main store page for more shopping
            var count = await _shoppingCart.GetCountAsync();
            return Json(count);
        }
        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        
        public async Task<IActionResult> RemoveFromCartAsync(int id)
        {
            // Remove the item from the cart
           // var cart = ShoppingCart.GetCart(this.HttpContext);

            // Get the name of the album to display confirmation
            var cartitems = await _entitiesRequest.GetCartOrdersAsync();
            CartOrder cartItem = cartitems.Where(item => item.CartOrderId.Equals(ShoppingCart.ShoppingCartId) && item.RecordId == id).FirstOrDefault();

            // Remove from cart
            int itemCount = await _shoppingCart.RemoveFromCartAsync(id, cartItem);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message =  (cartItem.Name) +
                    " has been removed from your shopping cart.",
                CartTotal = await _shoppingCart.GetTotalAsync(),
                CartCount = await _shoppingCart.GetCountAsync(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }
        //
        // GET: /ShoppingCart/CartSummary
        //[ChildActionOnly]
        public ActionResult CartSummary()
        {
            //var cart = ShoppingCart.GetCart(this.HttpContext);

            ViewData["CartCount"] = _shoppingCart.GetCountAsync();
            return PartialView("CartSummary");
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
