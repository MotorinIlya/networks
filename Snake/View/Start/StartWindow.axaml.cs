using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

using Snake.View.Create;
using Snake.View.Join;

namespace Snake.View.Start;

public partial class StartWindow : Window
{
    public StartWindow()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
        CanResize = false;
        InitializeComponent();
    }

    public void ClickCreateGame(object sender, RoutedEventArgs args)
    {
        var createFieldWindow = new CreateFieldWindow();
        createFieldWindow.Show();
        Close();
    }

    public void ClickJoinGame(object sender, RoutedEventArgs args)
    {
        var joinWindow = new JoinWindow();
        joinWindow.Show();
        Close();
        joinWindow.AddGames();
    }

    public void ClickExit(object sender, RoutedEventArgs args)
    {
        Close();
    }
}