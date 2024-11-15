using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Windows;
using System.Collections.Generic;

using HttpPlaces;

namespace lab3;

public partial class MainWindow : Window
{
    PlacesApi placeApi;
    public MainWindow()
    {
        InitializeComponent();
        placeApi = new PlacesApi();
    }

    private async void ButtonOnClick(object? sender, RoutedEventArgs e)
    {
        var places = await placeApi.GetPlaces(Search.Text);
        CreateButtons(places);
        Search.Clear();
    }

    private async void CreateButtons(List<PlaceInfo> places)
    {
        ButtonsContainer.Children.Clear();
        CreateStatus("Search");

        foreach (var place in places)
        {
            var button = new Button
            {
                Content = $"Place: {place.Name} City: {place.City}"
            };
            await placeApi.GetWeather(place);
            await placeApi.GetInterestingPlaces(place);
            await placeApi.GetDescription(place);
            
            button.Click += async (sender, e) => {
                
                ReadContainer.Children.Clear();
                ReadContainer.Children.Add(new TextBlock
                {
                    Text = "Place: " + place.Name,
                    FontSize = 20,
                    FontWeight= Avalonia.Media.FontWeight.Bold
                });
                ReadContainer.Children.Add(new TextBlock
                {
                    Text = "Kind: " + place.Kind,
                });
                ReadContainer.Children.Add(new TextBlock
                {
                    Text = "City: " + place.City
                });
                ReadContainer.Children.Add(new TextBlock
                {
                    Text = "Country: " + place.Country
                });
                ReadContainer.Children.Add(new TextBlock
                {
                    Text = "Temperature: " + place.Temp
                });
                ReadContainer.Children.Add(new TextBlock
                {
                    Text = "Pressure: " + place.Pressure
                });
                ReadContainer.Children.Add(new TextBlock
                {
                    Text = "Humidity: " + place.Humidity
                });
                ReadContainer.Children.Add(new TextBlock
                {
                    Text = "Interesting places:"
                });
                foreach (var interestingPlace in place.InterestingPlaces)
                {
                    if (place.Description.ContainsKey(interestingPlace))
                    {
                        ReadContainer.Children.Add(new TextBlock
                        {
                            Text = interestingPlace + ": " + place.Description[interestingPlace]
                        });
                    }
                    else
                    {
                        ReadContainer.Children.Add(new TextBlock
                        {
                            Text = interestingPlace
                        });
                    }
                }
            };
            ButtonsContainer.Children.Add(button);
        }

        CreateStatus("Completed");
    }

    private void CreateStatus(string status)
    {
        StatusPanel.Children.Clear();
        StatusPanel.Children.Add(new TextBlock
        {
            Text = status
        });
    }
}