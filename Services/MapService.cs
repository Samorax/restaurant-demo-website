
using BingMapsRESTToolkit;

namespace restaurant_demo_website.Services
{
    public class MapService : IMapService
    {
        public MapService(IConfiguration configuration)
        {
            _configuration = configuration;
            key = _configuration.GetSection("BingMapApiKey").Value;
        }


        private string key {get;set;} 
        private IConfiguration _configuration;

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

      
    }
}