using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Snake.Net;

namespace Snake.View.Game;

public partial class GameWindow : Window
{
    private GameBoard _gameBoard;

    //create master
    public GameWindow(string name, string gameName, int width, int height)
    {
        InitializeComponent();
        CanResize = false;
        Width = width * ViewConst.BlockSize + ViewConst.StatisticsWidth;
        Height = height * ViewConst.BlockSize;
        Title = "Snake";

        _gameBoard = new GameBoard(this, name, gameName, width, height);
        Grid.SetColumn(_gameBoard, 0);
        MainGrid.Children.Add(_gameBoard);

        KeyDown += OnKeyDown;
    }

    //create joiner
    public GameWindow(string playerName, string gameName, GameAnnouncement config, Peer peer)
    {
        InitializeComponent();
        CanResize = false;
        Width = config.Config.Width * ViewConst.BlockSize + ViewConst.StatisticsWidth;
        Height = config.Config.Height * ViewConst.BlockSize;
        Title = "Snake";

        _gameBoard = new GameBoard(this, playerName, gameName, config, peer);
        Grid.SetColumn(_gameBoard, 0);
        MainGrid.Children.Add(_gameBoard);

        KeyDown += OnKeyDown;
    }

    public void ShowError()
    {
        
    }

    public void UpdateStatistics(GameState state)
    {
        Dispatcher.UIThread.Post(() => {
            Update(state);
        });
    }

    private void Update(GameState state)
    {
        PlayerBoard.Children.Clear();
        foreach (var player in state.Players.Players)
        {
            PlayerBoard.Children.Add(new TextBlock
            {
                Text = player.Name 
                        + "(" + player.Role.ToString() + ":" + player.Id.ToString() + ") : " 
                        + player.Score
            });
        }
        PlayerBoard.Children.Add(new TextBlock
        {
            Text = $"Snake count: {state.Snakes.Count}"
        });
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
       _gameBoard.HandleInput(e.Key);
    }
}