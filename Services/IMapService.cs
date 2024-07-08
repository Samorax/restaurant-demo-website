using BingMapsRESTToolkit;

namespace restaurant_demo_website.Services
{
    public interface IMapService    
    {
        Task<double> GetDistance(string originPostcode, string destPostcode);
        Task GetAddressImage(string addressline, string locality, string postcode, string country);
    }
}