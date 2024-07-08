
using BingMapsRESTToolkit;
using System.Drawing;
using System.Drawing.Imaging;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;





namespace restaurant_demo_website.Services
{
    public class MapService : IMapService
    {
        public MapService(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            key = _configuration.GetSection("BingMapApiKey").Value;
        }


        private string key {get;set;} 
        private IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;

        public async Task<double> GetDistance(string originPostcode, string destPostcode)
        {
            var OriginRequest = new GeocodeRequest
            {
                BingMapsKey = key,
                Address = new SimpleAddress
                {
                    PostalCode = originPostcode
                }

            };

            var DestinationRequest = new GeocodeRequest
            {
                BingMapsKey = key,
                Address = new SimpleAddress
                {

                    PostalCode = destPostcode
                }
            };

            var result = await ServiceManager.GetResponseAsync(OriginRequest);
            var result2 = await ServiceManager.GetResponseAsync(DestinationRequest);

            var r = result.ResourceSets[0].Resources[0] as BingMapsRESTToolkit.Location;
            var r2 = result2.ResourceSets[0].Resources[0] as BingMapsRESTToolkit.Location;

            var Originlongitude = r.GeocodePoints.First().Coordinates[0];
            var Originlatitude = r.GeocodePoints.First().Coordinates[1];

            var Destlongitude = r2.GeocodePoints[0].Coordinates[0];
            var Destlatitude = r2.GeocodePoints[0].Coordinates[1];

            var distance = new DistanceMatrixRequest
            {
                BingMapsKey = key,
                TravelMode = TravelModeType.Driving,
                DistanceUnits = DistanceUnitType.Miles,
                Origins = new List<SimpleWaypoint> { new SimpleWaypoint(Originlongitude, Originlatitude) },
                Destinations = new List<SimpleWaypoint> { new SimpleWaypoint(Destlongitude, Destlatitude) }
            };



            var d = await distance.Execute();
            var k = d.ResourceSets[0].Resources[0] as BingMapsRESTToolkit.DistanceMatrix;
            return k.Results.First().TravelDistance;
        }

        public async Task GetAddressImage(string addressline, string locality, string postcode, string country)
        {
            var imgSource = string.Empty;
            var o = new GeocodeRequest
            {
                BingMapsKey = key,
                Address = new SimpleAddress
                {
                    AddressLine = addressline,
                    Locality = locality,
                    PostalCode = postcode,
                    CountryRegion = country
                }
            };
            var d = await o.Execute();
            var location = d.ResourceSets[0].Resources[0] as BingMapsRESTToolkit.Location;
            var longitude = location.GeocodePoints.First().Coordinates[0];
            var latitude = location.GeocodePoints.First().Coordinates[1];
            await GetMapImage(longitude, latitude);

        }

        private async Task GetMapImage(double longitude, double latitude)
        {
            var request = new ImageryRequest
            {
                BingMapsKey = key,
                MapHeight = 500,
                MapWidth = 800,
                CenterPoint = new Coordinate(longitude, latitude),
                ZoomLevel = 16,
                ImagerySet = ImageryType.CanvasLight,
                Format = ImageFormatType.PNG,
                Pushpins = new List<ImageryPushpin>(){
                new ImageryPushpin(){
                    Location = new Coordinate(longitude, latitude),
                    Label = "hi"
                } },
                HighlightEntity = true


            };

            using (var imageStream = await ServiceManager.GetImageAsync(request))
            {
                var img = Image.FromStream(imageStream);
                var basepath = _hostingEnvironment.WebRootPath;
                var imgSource = Path.Combine(basepath, "Images\\AddressImage.png");
                img.Save(imgSource, ImageFormat.Png);

            }
        }
    }
}