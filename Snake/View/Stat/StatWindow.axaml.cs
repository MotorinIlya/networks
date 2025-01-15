using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Snake.Net;

namespace Snake.View.Stat;


public partial class StatWindow : Window
{
    public StatWindow(GamePlayers players, int stateDelay)
    {
        InitializeComponent();
        
        // async () => 
        // {
        //     AddPlayers(players, stateDelay);
        // };
    }
    
    public async void AddPlayers(GamePlayers players, int stateDelay)
    {
        while (true)
        {
            Players.Children.Clear();
            foreach (var player in players.Players)
            {
                var infoPlayer = player.Name + " " + player.Role.ToString() + " " + player.Score.ToString();
                Players.Children.Add(new TextBlock
                {
                    Text = infoPlayer
                });
            }
        }
    }
}