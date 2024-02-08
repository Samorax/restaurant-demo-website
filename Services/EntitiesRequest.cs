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
        private HttpClient _httpClient;
    


        public EntitiesRequest(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _configuration.GetSection("ApiKey").Value);
            _httpClient.BaseAddress = new Uri(_configuration.GetSection("BaseUrl").Value);
          
        }
  
        public async Task<CartOrder> AddCartOrderAsync(CartOrder cartOrder)
        {
            
            var response = await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress + "/cartOrders",cartOrder);
            return JsonConvert.DeserializeObject<CartOrder>(await response.Content.ReadAsStringAsync());

          
        }

        public async Task<Order> AddOrderAsync(Order order)
        {
            var response = await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress + "/Orders", order);
          
            return JsonConvert.DeserializeObject<Order>(await response.Content.ReadAsStringAsync());
            
        }

        public async Task<OrderDetail> AddOrderDetailsAsync(OrderDetail orderDetail)
        {

            var response = await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress + "/orderdetails", orderDetail);
            return JsonConvert.DeserializeObject<OrderDetail>(await response.Content.ReadAsStringAsync());
        }

        public async Task DeleteCartOrderAsync(CartOrder cartOrder)
        {
            var response = await _httpClient.DeleteAsync(_httpClient.BaseAddress + $"/cartorders/{cartOrder.RecordId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<CartOrder>> GetCartOrdersAsync()
        {
           // _httpClient.DefaultRequestHeaders.Add("x-api-key", _configuration.GetSection("ApiKey").Value);
           //_httpClient.BaseAddress = new Uri(_configuration.GetSection("BaseUrl").Value);
           
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<CartOrder>>(_httpClient.BaseAddress + "/cartorders");

            return response;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            IEnumerable<Order> response = await _httpClient.GetFromJsonAsync<IEnumerable<Order>>(_httpClient.BaseAddress + "/Orders");
            return response;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            IEnumerable<Product> response = await _httpClient.GetFromJsonAsync<IEnumerable<Product>>(_httpClient.BaseAddress + "/products");
            return response;
        }

        public async Task UpdateCartOrderAsync(CartOrder cartOrder)
        {
            var response = await _httpClient.PutAsJsonAsync(_httpClient.BaseAddress + $"/orders/{cartOrder.RecordId}", cartOrder);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            var response = await _httpClient.PutAsJsonAsync(_httpClient.BaseAddress+$"/orders/{order.OrderID}", order);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Customer> CreateCustomerAsync(CustomerTDO customer)
        {
            var response = await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress + "/customers", customer);
            return JsonConvert.DeserializeObject<Customer>(await response.Content.ReadAsStringAsync());
        }


        public async Task<Customer>CustomerSignInAsync(CustomerSignIn customer)
        {
            var response = await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress + "/login/customers", customer);
            return JsonConvert.DeserializeObject<Customer>(await response.Content.ReadAsStringAsync());
        }

        
    }
}
