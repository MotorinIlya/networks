using System.Net;
using System.Threading;
using Snake.Model;
using Snake.Net;
using Snake.Service;
using Snake.Service.Event;
using Snake.View;

namespace Snake.Controller;

public class GameController : Observer
{
    private GameModel _gameModel;

    private Peer _peer;

    public GameController(string name, string gameName, Map map)
    {
        _peer = new Peer();
        _gameModel = new GameModel(name, gameName, map, _peer.IpEndPoint);
        _peer.AddObserver(this);
        _gameModel.Run();
    }

    public GameController(string playerName, string gameName, GameAnnouncement config, Peer peer, Map map)
    {
        _peer = new Peer();
        _gameModel = new GameModel(playerName, gameName, map, peer.IpEndPoint, config);
    }

    public void SearchPlayers()
    {
        var createMsgThread = new Thread(PeriodicCreateMsg);
        createMsgThread.Start();
    }

    public void Join()
    {
        _peer.SearchMulticastCopies();
    }

    private void PeriodicCreateMsg()
    {
        while (true)
        {
            var msg = CreatorMessages.CreateAnnouncementMsg(_gameModel);
            _peer.AddMsg(msg, new IPEndPoint(IPAddress.Parse(NetConst.MulticastIP), NetConst.MulticastPort));

            msg = CreatorMessages.CreateStateMsg(_gameModel);
            foreach (var player in _gameModel.Players.Players)
            {
                if (player.Id != _gameModel.MainId)
                {
                    _peer.AddMsg(msg, new IPEndPoint(IPAddress.Parse(player.IpAddress), player.Port));
                }
            }
            
            Thread.Sleep(NetConst.StartDelay);
        }
    }

    public override void Update(GameEvent gameEvent)
    {
        var msg = gameEvent.Message;

        switch(msg.TypeCase)
        {
            case GameMessage.TypeOneofCase.Announcement:
                break;
            case GameMessage.TypeOneofCase.Join:
                var coord = FinderFreePosition.FreePositionSnake(_gameModel.GameMap);
                if (coord is not null)
                {
                    var id = _gameModel.AddPlayer(msg.Join.PlayerName, gameEvent.IpEndPoint, NodeRole.Normal, coord);
                    _peer.AddMsg(CreatorMessages.CreateAckMsg(id), gameEvent.IpEndPoint);
                }
                else
                {
                    _peer.AddMsg(CreatorMessages.CreateErrorMsg(ViewConst.ErrorMsg), gameEvent.IpEndPoint);
                }
                break;
            case GameMessage.TypeOneofCase.State:
                _gameModel.UpdateMap(msg.State.State); 
                break;
        }

    }
}