using System.Net.Http;
using System.Text.Json;

namespace HttpPlaces
{
    public class RequestWorker
    {
        private HttpClient httpClient = new HttpClient();

        public RequestWorker() {}
        public async Task<Places>? GetLocations(string nameLocation)
        {
            var message = await httpClient.GetAsync($"https://graphhopper.com/api/1/geocode?q={nameLocation}&locale=ru&limit=10&reverse=false&debug=false&provider=default&key=bd0739b1-e29d-444d-9859-c5e02f013656");
            string response = await message.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Places>(response);
        }

        public async Task<WeatherJson> GetWeather(double lat, double lon)
        {
            var message = await httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid=8dfba83e7b60159cda4f0733976edb12");
            string response = await message.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<WeatherJson>(response);
        }

        public async Task<InterestingPlacesJson> GetInterestingPlaces(double lat, double lon)
        {
            Console.WriteLine(lat.ToString() + ", " + lon.ToString());
            var message = await httpClient.GetAsync($"https://api.opentripmap.com/0.1/ru/places/radius?radius=1000&lon={lon.ToString().Replace(',', '.')}&lat={lat.ToString().Replace(',', '.')}&apikey=5ae2e3f221c38a28845f05b6525262d2e9501766df7dc543333148a9");
            string response = await message.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<InterestingPlacesJson>(response);
        }

        public async Task<DescriptionJson> GetDescription(string id)
        {
            var message = await httpClient.GetAsync($"http://api.opentripmap.com/0.1/ru/places/xid/{id}?apikey=5ae2e3f221c38a28845f05b6525262d2e9501766df7dc543333148a9");
            string response = await message.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DescriptionJson>(response);
        }
    }
}