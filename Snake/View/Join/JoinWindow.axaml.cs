using System.Threading;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Snake.Model;
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
        Thread.Sleep(NetConst.StartDelay);
        AddGames();
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
            button.Click += (sender, e) => {
                var joinMsg = CreatorMessages.CreateJoinMsg(PlayerName.Text, gameName);
                _peer.AddMsg(joinMsg, endPoint);
            };
            games.Children.Add(button);
        }
    }
}