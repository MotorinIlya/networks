using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Snake.View.Game;

namespace Snake.View.Create;

public partial class CreateFieldWindow : Window
{
    public CreateFieldWindow()
    {
        CanResize = false;
        InitializeComponent();
    }

    public void ClickCreate(object sender, RoutedEventArgs args)
    {
        var name = Name.Text is string text ? text : "Master";
        var gameName = GameName.Text is string game ? game : "Game";
        var width = Width.Text is string widthText ? widthText : "40";
        var height = Height.Text is string heightText ? heightText : "30";
        var gameWindow = new GameWindow(name, gameName, int.Parse(width), int.Parse(height));
        gameWindow.Show();
        Close();
    }
}