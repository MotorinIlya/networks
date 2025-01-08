using System.Collections.Generic;
using Avalonia.Media;
using Snake.Model;

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
}