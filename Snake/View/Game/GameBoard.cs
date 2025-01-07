using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Snake.Model;

namespace Snake.View.Game;

public class GameBoard : UserControl
{
    private Map _map;
    public GameBoard(string name, int width, int height)
    {
        Focusable = true;
        _map = new Map(width, height);
        new Model.Game(name, _map);
    }

    public void HandleInput(KeyEventArgs e){}

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        // for (int y = 0; y < _map.HeightMap; y++)
        // {
        //     for (int x = 0; x < _mapChanger.WidthMap; x++)
        //     {
        //         var rect = new Rect(x * TileSize, y * TileSize, TileSize, TileSize);
        //         context.FillRectangle(_tileBrushes[_mapChanger.GetPosition(y, x)], rect);
        //         context.DrawRectangle(new Pen(Brushes.Black, 1), rect);
        //     }
        // }
    }
}