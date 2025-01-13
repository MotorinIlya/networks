using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Snake.Controller;
using Snake.Model;
using Snake.Net;

namespace Snake.View.Game;

public class GameBoard : UserControl
{
    private Map _map;
    private Timer _timer;
    private TurnController _controller;
    public GameBoard(string name, string gameName, int width, int height)
    {
        Focusable = true;
        _timer = new Timer(OnTimerTick, null, 0, 1000);
        _map = new Map(width, height);
        var game = new Model.Game(name, gameName, _map);
        _controller = game.GetTurnController();
    }

    public GameBoard(string playerName, string gameName, GameAnnouncement config, Peer peer)
    {
        Focusable = true;
        _timer = new Timer(OnTimerTick, null, 0, 1000);
        _map = new Map(config.Config.Width, config.Config.Height);
        var game = new Model.Game(playerName, gameName, config, peer, _map);
        _controller = game.GetTurnController();
    }

    private void OnTimerTick(object? state)
    {
        Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
    }

    public void HandleInput(Key e)
    {
        _controller.Update(e);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        for (int y = 0; y < _map.Height; y++)
        {
            for (int x = 0; x < _map.Width; x++)
            {
                var rect = new Rect(x * ViewConst.BlockSize, 
                                    y * ViewConst.BlockSize, 
                                    ViewConst.BlockSize, 
                                    ViewConst.BlockSize);
                context.FillRectangle(ViewConst.GameBrushes[_map.GetGameObject(x, y)], rect);
                context.DrawRectangle(new Pen(ViewConst.GameBrushes[_map.GetGameObject(x, y)], 1), rect);
            }
        }
    }
}