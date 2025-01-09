using Avalonia.Controls;
using Snake.Net;


namespace Snake.View.Join;

public partial class JoinWindow : Window
{

    private Peer _peer;
    public JoinWindow()
    {
        InitializeComponent();
        _peer = new Peer();
        _peer.SearchMulticastCopies();
    }

    public void AddGames()
    {
        var Games = _peer.Games;
        foreach(var Game in Games)
        {
            var endPoint = Game.Key;
            var gameMsg = Game.Value;
            var gameName = gameMsg.Announcement.Games[0].GameName;
            var gameConfig = gameMsg.Announcement.Games[0].Config;
            var button = new Button
            {
                Content = gameName
            };
            //button.Click += JoinGame();
            games.Children.Add(button);
        }
    }
}