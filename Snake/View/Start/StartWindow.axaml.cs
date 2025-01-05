using Avalonia.Controls;
using Avalonia.Interactivity;

using Snake.View.Create;

namespace Snake.View.Start;

public partial class StartWindow : Window
{
    public StartWindow()
    {
        InitializeComponent();
    }

    public void ClickCreateGame(object sender, RoutedEventArgs args)
    {
        box.Text = "Hello i'm clicked";
        
        var createFieldWindow = new CreateFieldWindow();
        createFieldWindow.Show();
        Close();
    }

    public void ClickJoinGame(object sender, RoutedEventArgs args)
    {
        box.Text = "Click Join";
    }

    public void ClickExit(object sender, RoutedEventArgs args)
    {
        Close();
    }
}