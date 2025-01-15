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
        var endPoint = gameEvent.IpEndPoint;
        switch (msg.TypeCase)
        {
            case GameMessage.TypeOneofCase.Steer:
                _model.ChangeDirection(msg.Steer.Direction, endPoint);
                _peer.AddMsg(CreatorMessages.CreateAckMsg(_model.MainId, _model.EndPointToId(endPoint), msg.MsgSeq), endPoint);
                break;
        }
    }

    public void Update(Key key)
    {
        if (MConst.KeyDirection.ContainsKey(key))
        {
            if (_model.Role == NodeRole.Master)
            {
                if (_model.GetSnake(_model.MainId).HeadDirection != MConst.OpKeyDirection[key])
                {
                    UpdateDirectionSnake(MConst.KeyDirection[key], _model.MainId);
                }
            }
            else if (_model.Role == NodeRole.Deputy || _model.Role == NodeRole.Normal)
            {
                if (_model.GetSnake(_model.MainId).HeadDirection != MConst.OpKeyDirection[key])
                {
                    var player = _model.GetMaster();
                    _peer.AddMsg(CreatorMessages.CreateSteerMsg(MConst.KeyDirection[key]), 
                                new IPEndPoint(IPAddress.Parse(player.IpAddress), player.Port));
                }
            }
        }
    }
}