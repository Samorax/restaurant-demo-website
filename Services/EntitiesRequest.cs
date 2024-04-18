using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using restaurant_demo_website.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;

namespace restaurant_demo_website.Services
{
    public class EntitiesRequest : IEntitiesRequest
    {
        private IConfiguration _configuration;
        private  readonly HttpClient _httpClient;

        public Uri BaseAddress { get; set; }
        public IDictionary<string,string> DefaultRequestHeaders = new Dictionary<string, string>();

        public EntitiesRequest(IConfiguration configuration, HttpClient httpClient)
        {
           
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_configuration.GetSection("BaseUrl").Value);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _configuration.GetSection("ApiKey").Value);

        }
  
        public async Task<CartOrder> AddCartOrderAsync(CartOrder cartOrder)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/cartOrders",cartOrder);
            return JsonConvert.DeserializeObject<CartOrder>(await response.Content.ReadAsStringAsync());
         
        }

        public async Task<Order> AddOrderAsync(Order order)
        {
          
           
            var response = await _httpClient.PostAsJsonAsync("/api/Orders", order);
            return JsonConvert.DeserializeObject<Order>(await response.Content.ReadAsStringAsync());
            
        }

        public async Task<OrderDetail> AddOrderDetailsAsync(OrderDetail orderDetail)
        {
           
          
            var response = await _httpClient.PostAsJsonAsync("/api/orderdetails", orderDetail);
            return JsonConvert.DeserializeObject<OrderDetail>(await response.Content.ReadAsStringAsync());
        }

        public async Task DeleteCartOrderAsync(CartOrder cartOrder)
        {
           
            
            var response = await _httpClient.DeleteAsync($"/api/cartorders/{cartOrder.RecordId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<CartOrder>> GetCartOrdersAsync()
        {
            
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<CartOrder>>("/api/cartorders");
            return response;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
           
            IEnumerable<Order> response = await _httpClient.GetFromJsonAsync<IEnumerable<Order>>("/api/Orders");
            return response;
        }

        public async Task PostOrderToQueue(Order order)
        {
            var response = await _httpClient.PostAsJsonAsync<Order>("/api/queue/orderqueue", order);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            
                
            IEnumerable<Product> response = await _httpClient.GetFromJsonAsync<IEnumerable<Product>>("/api/products");
            return response;
        }

        public async Task UpdateCartOrderAsync(CartOrder cartOrder)
        {
            
            var response = await _httpClient.PutAsJsonAsync($"/api/cartorders/{cartOrder.RecordId}", cartOrder);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateOrderAsync(Order order)
        {
           
            var response = await _httpClient.PutAsJsonAsync($"/api/orders/{order.OrderID}", order);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Customer> CreateCustomerAsync(CustomerTDO customer)
        {
          
            var response = await _httpClient.PostAsJsonAsync("/api/customers", customer);
            return JsonConvert.DeserializeObject<Customer>(await response.Content.ReadAsStringAsync());
        }


        public async Task<Customer>CustomerSignInAsync(CustomerSignIn customer)
        {
            
            var response = await _httpClient.PostAsJsonAsync("/api/login/customer", customer);
            return JsonConvert.DeserializeObject<Customer>(await response.Content.ReadAsStringAsync());
        }

        

        public async Task<string> CreateSetupIntent(PaymentObject paymentObject)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/dojo/setupintent", paymentObject);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<IEnumerable<Rewards>> GetRewardsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Rewards>>("/api/rewards");
            return response;
        }

        
        public async Task<RewardClaimResponse> ClaimRewardAsync(ClaimRewardModel claimRewardModel)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/rewards/claimreward", claimRewardModel);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<RewardClaimResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<Customer> GetCustomerAsync(string customerId)
        {
            var response = await _httpClient.GetFromJsonAsync<Customer>($"/api/customers/{customerId}");
            return response;
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/customers/{customer.Id}",customer);
            response.EnsureSuccessStatusCode();
            
        }

        public async Task RemoveCustomerAsync(Customer? customer)
        {
            var response = await _httpClient.DeleteAsync($"/api/customers/{customer.Id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<VoucherChargeResponse> ChargeVoucher(ChargeVoucherModel c)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/vouchers/chargevoucher",c);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<VoucherChargeResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<ApplicationUser> GetRestaurantInfo()
        {
            var response = await _httpClient.GetFromJsonAsync<ApplicationUser>("/api/restaurant");
            return response;
        }
    }
}
