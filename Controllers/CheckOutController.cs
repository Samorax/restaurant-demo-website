using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using restaurant_demo_website.Models;
using restaurant_demo_website.Services;
using restaurant_demo_website.ViewModels;
using System.Data;

namespace restaurant_demo_website.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private IEntitiesRequest _entitiesRequest;
        private ShoppingCart _shoppingCart;

        public CheckoutController(IEntitiesRequest entitiesRequest, ShoppingCart shoppingCart)
        {
            _entitiesRequest = entitiesRequest;
            _shoppingCart = shoppingCart;
        }
        
        const string PromoCode = "FREE";
        

        //
        // GET: /Checkout/AddressAndPayment
        public async Task<ActionResult> AddressAndPaymentAsync()
        {
            
            // Return the view
            return View();

        }
        //
        // POST: /Checkout/AddressAndPayment
        [HttpPost]
        public async Task<ActionResult> AddressAndPaymentAsync(IFormCollection values)
        {
            var order = new Order();
            //TryUpdateModel(order);

            try
            {
                var deliveryAddress = values["FirstLineAddress"] + values["SecondLineAddress"] + values["City"] +
                    values["State"] + values["Country"] + values["PostalCode"];
                    order.Username = User.Identity.Name;
                    order.OrderDate = DateTime.Now;
                    order.Channel = "Website";
                    order.DeliveryAddress = deliveryAddress;
                    order.CustomerID =  User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value; ;
                    
                    
                    //Save Order
                    var orderAdded = await _entitiesRequest.AddOrderAsync(order);
                    
                    //Process the order
                    //var cart = ShoppingCart.GetCart(this.HttpContext);
                    var paymentToken = await _shoppingCart.CreateOrderAsync(orderAdded);

                    //redirect to payment page
                    return Redirect($"https://pay.dojo.tech/checkout/{paymentToken}");

                    //return RedirectToAction("Complete",
                    //    new { id = orderAdded.OrderID });
            }
            catch
            {
                var viewModel = new AddressandPaymentViewModel();
                return View(viewModel);
            }
        }

        //
        // GET: /Checkout/Complete
        public async Task<ActionResult> Complete(int id)
        {
            // Validate customer owns this order
            var orders = await _entitiesRequest.GetOrdersAsync();
            bool isValid = orders.Any(
                o => o.OrderID == id &&
                o.Username == User.Identity.Name);

            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }
    }
}