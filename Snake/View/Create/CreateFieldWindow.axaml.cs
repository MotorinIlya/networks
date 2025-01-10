using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Snake.View.Game;

namespace Snake.View.Create;

public partial class CreateFieldWindow : Window
{
    public CreateFieldWindow()
    {
        InitializeComponent();
    }

    public void ClickCreate(object sender, RoutedEventArgs args)
    {
        var gameWindow = new GameWindow(Name.Text, GameName.Text, int.Parse(Width.Text), int.Parse(Height.Text));
        gameWindow.Show();
        Close();
    }

}