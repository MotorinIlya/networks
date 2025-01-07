using Avalonia.Controls;
using Avalonia.Input;

namespace Snake.View.Game;

public partial class GameWindow : Window
{
    private GameBoard _gameBoard;

    public GameWindow(string name, int width, int height)
    {
        CanResize = false;
        Width = width * ViewConst.BlockSize;
        Height = height * ViewConst.BlockSize;
        Title = "Snake";

        _gameBoard = new GameBoard(name, width, height);
        Content = _gameBoard;

        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
       // _gameBoard.HandleInput(e.Key);
    }
}