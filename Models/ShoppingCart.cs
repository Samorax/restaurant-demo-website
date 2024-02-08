using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using restaurant_demo_website.Extensions;
using restaurant_demo_website.Services;
using SQLitePCL;


namespace restaurant_demo_website.Models
{
    public partial class ShoppingCart
    {
       
       private HttpClient _httpClient = new HttpClient();
       private IConfiguration _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        
        string ShoppingCartId { get; set; }
        
        public static IEnumerable<CartOrder> Carts { get; set; }
        public IEntitiesRequest _entitiesRequest;
        public const string CartSessionKey = "CartId";
        
        

        private static ShoppingCart GetCart(ControllerBase controller)
        {
            return GetCart(controller.Request.HttpContext);
        }

        public static ShoppingCart GetCart(HttpContext httpContext)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(httpContext);
            return cart;
        }

        public async Task AddToCart(Product product)
        {
            // Get the matching cart and album instances
            CartOrder cartItem;
            
            Carts = await new EntitiesRequest(_configuration,_httpClient).GetCartOrdersAsync();
            if(Carts.Any()){
                cartItem = Carts.FirstOrDefault(
                c => c.CartOrderId == ShoppingCartId
                && c.ProductId == product.ProductID);

                if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new CartOrder
                {
                    ProductId = product.ProductID,
                    CartOrderId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
               await new EntitiesRequest(_configuration,_httpClient).AddCartOrderAsync(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                var x = cartItem.Count + 1;
                cartItem.Count = x;
                await new EntitiesRequest(_configuration,_httpClient).UpdateCartOrderAsync(cartItem);
            }
            }else{
                cartItem = new CartOrder
                {
                    ProductId = product.ProductID,
                    CartOrderId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                await new EntitiesRequest(_configuration, _httpClient).AddCartOrderAsync(cartItem);
            }
             
            
        }

        

        public async Task<int> RemoveFromCartAsync(int id)
        {
            // Get the cart
            var cartItems = Carts;
            var cartItem = cartItems.Single(
                cart => cart.CartOrderId.ToString() == ShoppingCartId
                && cart.ProductId == id);

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
                    await new EntitiesRequest(_configuration, _httpClient).DeleteCartOrderAsync(cartItem);
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
                await new EntitiesRequest(_configuration,_httpClient).DeleteCartOrderAsync(cartItem);
            }
   
           
        }
        public async Task<IEnumerable<CartOrder>> GetCartItemsAsync()
        {
            Carts = await new EntitiesRequest(_configuration, _httpClient).GetCartOrdersAsync();
            return Carts.Where(
                cart => cart.CartOrderId == ShoppingCartId).ToList();
        }
        public async Task<int> GetCountAsync()
        {
            // Get the count of each item in the cart and sum them up
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
            decimal? total = (from cartItems in Carts
                              where cartItems.CartOrderId == ShoppingCartId
                              select (int?)cartItems.Count *
                              cartItems.Product.Price).Sum();

            return total ?? 0;
        }
        public async Task<int> CreateOrderAsync(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = Carts.Where(c=>c.CartOrderId == ShoppingCartId).ToList();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    OrderId = order.OrderID,
                    UnitPrice = item.Product.Price,
                    Quantity = item.Count
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Count * item.Product.Price);

                await new EntitiesRequest(_configuration,_httpClient).AddOrderDetailsAsync(orderDetail);

            }
            // Set the order's total to the orderTotal count
            order.TotalAmount = orderTotal;

            // Save the order
            await new EntitiesRequest(_configuration,_httpClient).UpdateOrderAsync(order);
            // Empty the shopping cart
            await EmptyCartAsync();
            // Return the OrderId as the confirmation number
            return order.OrderID;
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
            
             var shoppingCart = Carts.Where(
                c => c.CartOrderId == ShoppingCartId);

            foreach (CartOrder item in shoppingCart)
            {
                item.CartOrderId = userName;
                await new EntitiesRequest(_configuration,_httpClient).UpdateCartOrderAsync(item);
            } 
            
        }

    }
}