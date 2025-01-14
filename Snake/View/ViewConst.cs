using System.Collections.Generic;
using Avalonia.Media;
using Snake.Model;
using Snake.Net;

namespace Snake.View;

public static class ViewConst
{
    public const int BlockSize = 20;

    public static readonly Dictionary<GameObjects, IBrush> GameBrushes = new()
    {
        {GameObjects.Floor, Brushes.Black},
        {GameObjects.SnakeBody, Brushes.Blue},
        {GameObjects.Apple, Brushes.Red}
    };

    public const string ErrorMsg = "The game is full";

    public static readonly Dictionary<string, NodeRole> StringToRole = new()
    {
        {"Normal", NodeRole.Normal},
        {"Viewer", NodeRole.Viewer}
    };
}