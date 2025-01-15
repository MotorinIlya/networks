using System.Threading;
using Avalonia.Controls;
using Snake.Model;
using Snake.Net;
using Snake.View.Game;

namespace Snake.View.Join;

public partial class JoinWindow : Window
{

    private Peer _peer;
    public JoinWindow()
    {
        CanResize = false;
        InitializeComponent();
        _peer = new Peer();
        _peer.SearchMulticastCopies();
        Thread.Sleep(NetConst.StartDelay);
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
                var role = GetRole(PlayerRole);
                string playerName = PlayerName.Text is string text ? text : "Player";
                var joinMsg = CreatorMessages.CreateJoinMsg(playerName, gameName, role);
                _peer.AddMsg(joinMsg, endPoint);
                var gameWindow = new GameWindow(playerName, gameName, gameMsg.Announcement.Games[0], _peer);
                gameWindow.Show();
                Close();
            };
            games.Children.Add(button);
        }
    }

    private NodeRole GetRole(ComboBox box)
    {
        var selectedItem = PlayerRole.SelectedItem as ComboBoxItem;
        if (selectedItem is not null && selectedItem.Content is string content)
        {
            return ViewConst.StringToRole[content.ToString()];
        }
        else
        {
            return ViewConst.StringToRole["Viewer"];
        }
    }
}