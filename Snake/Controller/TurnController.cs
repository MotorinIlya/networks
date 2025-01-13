using Avalonia.Input;
using Snake.Model;
using Snake.Net;
using Snake.Service;
using Snake.Service.Event;

namespace Snake.Controller;

public class TurnController(GameModel gameModel) : Observer
{
    private GameModel _model = gameModel;

    public void UpdateDirectionSnake(Direction newDirection, int playerId)
    {
        _model?.ChangeDirection(newDirection, playerId);
    }

    public override void Update(GameEvent gameEvent)
    {
        var msg = gameEvent.Message;
        switch (msg.TypeCase)
        {
            case GameMessage.TypeOneofCase.Steer:
                _model.ChangeDirection(msg.Steer.Direction, gameEvent.IpEndPoint);
                break;
        }
    }

    public void Update(Key key)
    {
        if (MConst.KeyDirection.ContainsKey(key))
        {
            UpdateDirectionSnake(MConst.KeyDirection[key], _model.MainId);
        }
    }
}