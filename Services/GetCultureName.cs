using FoodloyaleApi.Models;

using System.Configuration;
using System.Globalization;
using System.Net.Http;

namespace restaurant_demo_website.Services
{
    public class GetCultureName
    {
        private HttpClient _httpCLient;
        private IConfiguration _configuration;

        public GetCultureName(IConfiguration configuration, HttpClient client) 
        {
            _httpCLient = client;
            _configuration = configuration;
        }
        public async Task<string> GetNameAsync()
        {
            try
            {
                _httpCLient.BaseAddress = new Uri(_configuration.GetSection("BaseUrl").Value);
                _httpCLient.DefaultRequestHeaders.Add("x-api-key", _configuration.GetSection("ApiKey").Value);
                var response = await _httpCLient.GetFromJsonAsync<ApplicationUser>("/api/restaurant");
                var y = CultureInfo.GetCultures(CultureTypes.SpecificCultures).FirstOrDefault(x => x.NativeName.Contains(response.Country));

                return y.Name;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
            
        }
    }
}