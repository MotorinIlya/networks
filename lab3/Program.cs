using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;


namespace http
{

    
    public class Program
    {
      public static async Task Main()
      {
        Console.WriteLine("Start program");
        var requestWorker = new RequestWorker();

        string nameLocation = Console.ReadLine();

        Places? places = await requestWorker.GetLocations(nameLocation);

        int index = 0;
        foreach (var location in places.hits)
        {
          Console.WriteLine($"[{index}] - place: {location.name}; city: {location.city}");
          index++;
        }

        Console.WriteLine("Choose location index");
        index = int.Parse(Console.ReadLine());

        var place = places.hits[index];
        Console.WriteLine($"Place {place.name} {place.city}");
        WeatherJson? weather = await requestWorker.GetWeather(place.point.lat, place.point.lng);
        Console.WriteLine($"Weather temp {weather.main.temp - 273.15}");
        Console.WriteLine($"Pressure is {weather.main.pressure * 0.75}");

        InterestingPlacesJson? interestingPlaces = await requestWorker.GetInterestingPlaces(place.point.lat, place.point.lng);
        
        foreach (var interestingPlace in interestingPlaces.features)
        {
          if (interestingPlace.properties is Properties properties && properties.name is string name && (string.Compare(name, "") != 0))
          {
            Console.WriteLine("- " + interestingPlace.properties.name);
          }
          else { continue; }
          var message = await requestWorker.GetDescription(interestingPlace.properties.xid);
          if (message.wikipedia_extracts is Wiki wiki && wiki.text is string text)
          {
            Console.WriteLine(text);
          }
        }
      }
    }
}