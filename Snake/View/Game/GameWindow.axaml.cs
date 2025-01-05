using Avalonia.Controls;
using Avalonia.Input;

namespace Snake.View.Game;

public partial class GameWindow : Window
{
    //private GameBoard _gameBoard;

    public GameWindow()
    {
        Width = 800;
        Height = 600;
        Title = "Snake";

        // _gameBoard = new GameBoard();
        // Content = _gameBoard;

        // KeyDown += OnKeyDown;
    }

    // private void OnKeyDown(object? sender, KeyEventArgs e)
    // {
    //     _gameBoard.HandleInput(e.Key);
    // }
}