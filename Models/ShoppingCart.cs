using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Http;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using restaurant_demo_website.Extensions;
using restaurant_demo_website.Services;
using SQLitePCL;
using System.Net.Http;


namespace restaurant_demo_website.Models
{
    public partial class ShoppingCart
    {

        public ShoppingCart(IEntitiesRequest entitiesRequest)
        {
            _entitiesRequest = entitiesRequest;
        }
       
        private IEntitiesRequest _entitiesRequest;

        public static string ShoppingCartId { get; set; }
        
        public static IEnumerable<CartOrder> Carts { get; set; }
        
        public const string CartSessionKey = "CartId";


       /*  private static ShoppingCart GetCart(ControllerBase controller)
        {
            return GetCart(controller.Request.HttpContext);
        } */

        public void GetCart(HttpContext httpContext)
        {
            ShoppingCartId = GetCartId(httpContext);
        }

        public async Task AddToCart(Product product)
        {
            // Get the matching cart and album instances
            CartOrder cartItem;
            
            Carts = await _entitiesRequest.GetCartOrdersAsync();
            if(Carts.Any())
            {
                cartItem = Carts.Where(c => c.CartOrderId == ShoppingCartId && c.Name == product.Name).FirstOrDefault();

                if (cartItem == null)
                {
                    // Create a new cart item if no cart item exists
                    cartItem = new CartOrder
                    {
                        Name = product.Name,
                        CartOrderId = ShoppingCartId,
                        Count = 1,
                        DateCreated = DateTime.Now,
                        Price = product.Price
                    };
                await _entitiesRequest.AddCartOrderAsync(cartItem);
                }
                else
                {
                    // If the item does exist in the cart, 
                    // then add one to the quantity
                    cartItem.Count++;
                    await _entitiesRequest.UpdateCartOrderAsync(cartItem);
                }
                }else{
                    cartItem = new CartOrder
                    {
                        Name = product.Name,
                        CartOrderId = ShoppingCartId,
                        Count = 1,
                        DateCreated = DateTime.Now,
                        Price = product.Price
                    };
                    await _entitiesRequest.AddCartOrderAsync(cartItem);
                }
        }

        

        public async Task<int> RemoveFromCartAsync(int id,CartOrder cartItem)
        {
            // Get the cart

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    await _entitiesRequest.DeleteCartOrderAsync(cartItem);
                }
                // Save changes
                
            }
            return itemCount;
        }

       

        public async Task EmptyCartAsync()
        {
            
            var cartItems = Carts.Where(
                cart => cart.CartOrderId.ToString() == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                await _entitiesRequest.DeleteCartOrderAsync(cartItem);
            }
   
           
        }
        public async Task<IEnumerable<CartOrder>> GetCartItemsAsync()
        {
            var Carts = await _entitiesRequest.GetCartOrdersAsync();
            return Carts.Where(
                cart => cart.CartOrderId == ShoppingCartId).ToList();
        }
        public async Task<int> GetCountAsync()
        {
            // Get the count of each item in the cart and sum them up
            var Carts = await _entitiesRequest.GetCartOrdersAsync();
            int? count = (from cartItems in Carts
                          where cartItems.CartOrderId.ToString() == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }
        public async Task<decimal> GetTotalAsync()
        {
            //var Carts = await GetEntities().GetCartOrders();
            // Multiply album price by count of that album to get 
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            var Carts = await _entitiesRequest.GetCartOrdersAsync();
            decimal? total = (from cartItems in Carts
                              where cartItems.CartOrderId == ShoppingCartId
                              select (int?)cartItems.Count *
                              cartItems.Price).Sum();

            return total ?? 0;
        }
        public async Task<string> CreateOrderAsync(Order order)
        {
            decimal orderTotal = 0;
            Carts = await _entitiesRequest.GetCartOrdersAsync();
            var cartItems = Carts.Where(c=>c.CartOrderId == ShoppingCartId).ToList();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    Name = item.Name,
                    OrderId = order.OrderID,
                    UnitPrice = item.Price,
                    Quantity = item.Count
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Count * item.Price);

                await _entitiesRequest.AddOrderDetailsAsync(orderDetail);

            }
            // Set the order's total to the orderTotal count
            order.TotalAmount = orderTotal;

            // Generate the payment token for future payment
            var _paymentObject = new PaymentObject{Amount = (double)orderTotal, OrderId = order.OrderID, Description = $"This order has sum total of {orderTotal}", Currency = "GBP" };
            var paymentToken = await _entitiesRequest.CreateSetupIntent(_paymentObject);
            order.PaymentToken = paymentToken;

            // Save the order
            await _entitiesRequest.UpdateOrderAsync(order);
            await _entitiesRequest.PostOrderToQueue(order);
            // Empty the shopping cart
            await EmptyCartAsync();
            // Return the OrderId as the confirmation number
            return paymentToken;
        }

        

        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContext httpContext)
        {
            if (!httpContext.Session.Keys.Contains(CartSessionKey))
            {
                if (!string.IsNullOrWhiteSpace(httpContext.User.Identity.Name))
                {
                    httpContext.Session.SetString(CartSessionKey, httpContext.User.Identity.Name);
                        
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    httpContext.Session.SetString(CartSessionKey,tempCartId.ToString());
                }
            }
            return httpContext.Session.GetString(CartSessionKey);
        }
        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public async Task MigrateCartAsync(string userName)
        {
             Carts = await _entitiesRequest.GetCartOrdersAsync();
             var shoppingCart = Carts.Where(
                c => c.CartOrderId == ShoppingCartId);

            foreach (CartOrder item in shoppingCart)
            {
                item.CartOrderId = userName;
                await _entitiesRequest.UpdateCartOrderAsync(item);
            } 
            
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}