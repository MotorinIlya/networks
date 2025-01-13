using System.Net;
using Avalonia.Input;
using Snake.Model;
using Snake.Net;
using Snake.Service;
using Snake.Service.Event;

namespace Snake.Controller;

public class TurnController(GameModel gameModel, Peer peer) : Observer
{
    private GameModel _model = gameModel;

    private Peer _peer = peer;

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
            if (_model.Role == NodeRole.Master)
            {
                UpdateDirectionSnake(MConst.KeyDirection[key], _model.MainId);
            }
            else if (_model.Role == NodeRole.Deputy || _model.Role == NodeRole.Normal)
            {
                var player = _model.GetMaster();
                _peer.AddMsg(CreatorMessages.CreateSteerMsg(MConst.KeyDirection[key]), 
                            new IPEndPoint(IPAddress.Parse(player.IpAddress), player.Port));
            }
        }
    }
}