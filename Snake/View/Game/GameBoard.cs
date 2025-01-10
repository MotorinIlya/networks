using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Snake.Model;
using Snake.Net;

namespace Snake.View.Game;

public class GameBoard : UserControl
{
    private Map _map;
    public GameBoard(string name, string gameName, int width, int height)
    {
        Focusable = true;
        _map = new Map(width, height);
        _ = new Model.Game(name, gameName, _map);
    }

    public GameBoard(string playerName, string gameName, GameAnnouncement config, Peer peer)
    {
        Focusable = true;
        _map = new Map(config.Config.Width, config.Config.Height);
        _ = new Model.Game(playerName, gameName, config, peer, _map);
    }

    public void HandleInput(KeyEventArgs e){}

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        for (int y = 0; y < _map.Height; y++)
        {
            for (int x = 0; x < _map.Width; x++)
            {
                var rect = new Rect(x * ViewConst.BlockSize, y * ViewConst.BlockSize, ViewConst.BlockSize, ViewConst.BlockSize);
                context.FillRectangle(ViewConst.GameBrushes[_map.GetGameObject(x, y)], rect);
                context.DrawRectangle(new Pen(Brushes.Black, 1), rect);
            }
        }
    }
}