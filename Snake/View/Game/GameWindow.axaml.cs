using Avalonia.Controls;
using Avalonia.Input;
using Snake.Net;

namespace Snake.View.Game;

public partial class GameWindow : Window
{
    private GameBoard _gameBoard;

    //create master
    public GameWindow(string name, string gameName, int width, int height)
    {
        CanResize = false;
        Width = width * ViewConst.BlockSize;
        Height = height * ViewConst.BlockSize;
        Title = "Snake";

        _gameBoard = new GameBoard(this, name, gameName, width, height);
        Content = _gameBoard;

        KeyDown += OnKeyDown;
    }

    //create joiner
    public GameWindow(string playerName, string gameName, GameAnnouncement config, Peer peer)
    {
        CanResize = false;
        Width = config.Config.Width * ViewConst.BlockSize;
        Height = config.Config.Height * ViewConst.BlockSize;
        Title = "Snake";

        _gameBoard = new GameBoard(this, playerName, gameName, config, peer);
        Content = _gameBoard;

        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
       _gameBoard.HandleInput(e.Key);
    }

    public void ShowError()
    {
        
    }
}