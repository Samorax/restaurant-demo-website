namespace restaurant_demo_website.Services
{
    public interface IMapService    
    {
        Task<double> GetDistance(string originPostcode, string destPostcode);
    }
}