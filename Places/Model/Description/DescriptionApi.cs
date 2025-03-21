using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;

namespace HttpPlaces;
public class DescriptionApi : HttpApi
{
    public override async Task<InformationJson> Get(string id, HttpClient httpClient)
    {
        var message = await httpClient.GetAsync($"http://api.opentripmap.com/0.1/ru/places/xid/{id}?apikey=5ae2e3f221c38a28845f05b6525262d2e9501766df7dc543333148a9");
        string response = await message.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<DescriptionJson>(response);
    }
}
