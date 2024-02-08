using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using restaurant_demo_website.Models;
using restaurant_demo_website.Services;
using restaurant_demo_website.ViewModels;


namespace restaurant_demo_website.Controllers
{
    public class ShoppingCartController : Controller
    {
        private IEntitiesRequest _entitiesRequest;

        public ShoppingCartController(IEntitiesRequest entitiesRequest) 
        {
            _entitiesRequest = entitiesRequest;
        }
        
        //
        // GET: /ShoppingCart/
        public async Task<ActionResult> IndexAsync()
        {
            var cart = ShoppingCart.GetCart(HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = await cart.GetCartItemsAsync(),
                CartTotal = await cart.GetTotalAsync()
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
                var cart = ShoppingCart.GetCart(this.HttpContext);
                await cart.AddToCart(addedProduct);
            }
            

            // Go back to the main store page for more shopping
            return RedirectToAction("Index");
        }
        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public async Task<ActionResult> RemoveFromCartAsync(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Get the name of the album to display confirmation
            var cartitems = await _entitiesRequest.GetCartOrdersAsync();
            string productName = cartitems
                .Single(item => item.RecordId == id).Product.Name;

            // Remove from cart
            int itemCount = await cart.RemoveFromCartAsync(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message =  (productName) +
                    " has been removed from your shopping cart.",
                CartTotal = await cart.GetTotalAsync(),
                CartCount = await cart.GetCountAsync(),
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
            var cart = ShoppingCart.GetCart(this.HttpContext);

            ViewData["CartCount"] = cart.GetCountAsync();
            return PartialView("CartSummary");
        }
    }
}
