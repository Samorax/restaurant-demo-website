using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NuGet.Common;
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
        private IMemoryCache _memoryCache;

        public CheckoutController(IEntitiesRequest entitiesRequest, ShoppingCart shoppingCart,IMemoryCache memoryCache)
        {
            _entitiesRequest = entitiesRequest;
            _shoppingCart = shoppingCart;
            _memoryCache = memoryCache;
        }
        
        const string PromoCode = "FREE";
        

        //
        // GET: /Checkout/AddressAndPayment
        public async Task<ActionResult> OrderSummaryAsync()
        {

            ApplicationUser restaurantinfo = new ApplicationUser();
            if (ShoppingCart.CartSessionKey != null)
            {
                
                restaurantinfo = await _entitiesRequest.GetRestaurantInfo();
              
            }
            ViewData["RestaurantName"] = restaurantinfo.BusinessName;
             
            var cartTotal = await _shoppingCart.GetTotalAsync();
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = await _shoppingCart.GetCartItemsAsync(),
                CartTotal = cartTotal + restaurantinfo.DeliveryFee + ((restaurantinfo.VATCharge /100) * cartTotal),
                DeliveryFee = restaurantinfo.DeliveryFee,
                VAT = (restaurantinfo.VATCharge / 100) * cartTotal
            }; 
            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> ChargeVoucherAsync(string voucher)
        {
            var o = await _shoppingCart.GetTotalAsync();
            var c = new ChargeVoucherModel {VoucherNumber = voucher, OrderTotalAmount = o};
            var r = await _entitiesRequest.ChargeVoucher(c);
            AjaxVoucherResult result;
            if(r.Status == "Successful")
            {
                //add voucher as cartitems
                var voucherOrder = new CartOrder 
                { 
                    Name = voucher,
                    Count = 1, 
                    Price = r.OrderTotalAmount - o, 
                    DateCreated = DateTime.Now, 
                    CartOrderId = ShoppingCart.ShoppingCartId
                };
                await _entitiesRequest.AddCartOrderAsync(voucherOrder);
                //create a new row in the table for the voucher 
                //var voucherRow = $"<tr><td>{voucherOrder.Name}</td><td>{voucherOrder.Price}</td><td>{voucherOrder.Count}</td></tr>";
                //result = new AjaxVoucherResult{Reason = r.Reason, UpdateItem = voucherRow, NewCartTotal = r.OrderTotalAmount};
                return Json(r.Reason);
                
            }
           //result = new AjaxVoucherResult{Reason = r.Reason};
            return Json(r.Reason);
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
                var deliveryAddress = string.Concat(values["FirstLineAddress"]," ",values["SecondLineAddress"]," ",values["City"]," ",values["State"]," ",values["Country"]," ",values["PostalCode"]);
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
                    var paymentUrl = string.Concat("https://pay.dojo.tech/checkout/", paymentToken);
                    return Redirect(paymentUrl);

                    //return RedirectToAction("Complete",
                    //    new { id = orderAdded.OrderID });
            }
            catch
            {
                //var viewModel = new AddressandPaymentViewModel();
                return RedirectToAction("OrderSummary");
            }
        }

        //
        // GET: /Checkout/Complete
        public async Task<ActionResult> Complete(int id)
        {
            // Validate customer owns this order
            var orders = await _entitiesRequest.GetOrdersAsync();
            Order order = orders.SingleOrDefault(
                o => o.OrderID == id &&
                o.Username == User.Identity.Name);

            if (order != null)
            {

                await _entitiesRequest.PostOrderToQueue(order);
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }
    }
}