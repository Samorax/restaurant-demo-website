using System.Globalization;

namespace restaurant_demo_website.Services
{
    public class GetCultureName:IGetCultureName
    {
        public string GetName(string Country)
        {
            var y = CultureInfo.GetCultures(CultureTypes.SpecificCultures).FirstOrDefault(x => x.NativeName.Contains(Country));

            return y.Name;
        }
    }
}