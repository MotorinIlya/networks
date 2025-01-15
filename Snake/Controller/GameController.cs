using System.Net;
using System.Threading;
using Avalonia.Controls;
using Snake.Model;
using Snake.Net;
using Snake.Service;
using Snake.Service.Event;
using Snake.View;
using Snake.View.Game;

namespace Snake.Controller;

public class GameController : Observer
{
    private GameModel _gameModel;

    private Peer _peer;
    
    private GameWindow _gameWindow;

    //create master
    public GameController(GameWindow gameWindow, string name, string gameName, Map map)
    {
        _gameWindow = gameWindow;
        _peer = new Peer();
        _gameModel = new GameModel(name, gameName, map, _peer.IpEndPoint);
        _peer.AddDelay(_gameModel.StateDelayMs);
    }

    //create joiner
    public GameController(GameWindow gameWindow, 
                            string playerName, 
                            string gameName, 
                            GameAnnouncement config, 
                            Peer peer, 
                            Map map)
    {
        _gameWindow = gameWindow;
        _peer = peer;
        _gameModel = new GameModel(playerName, gameName, map, peer.IpEndPoint, config);
        _peer.AddDelay(_gameModel.StateDelayMs);
    }

    public void Run()
    {
        _gameModel.Run();
    }

    public void AddPeerObservers(TurnController turnController)
    {
        _peer.AddObserver(this);
        _peer.AddObserver(turnController);
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
        var endPoint = gameEvent.IpEndPoint;

        switch(msg.TypeCase)
        {
            case GameMessage.TypeOneofCase.Announcement:
                break;
            case GameMessage.TypeOneofCase.Join:
                if (msg.Join.RequestedRole != NodeRole.Viewer)
                {
                    var coord = FinderFreePosition.FreePositionSnake(_gameModel.GameMap);
                    if (coord is not null)
                    {
                        var id = _gameModel.AddPlayer(msg.Join.PlayerName, endPoint, NodeRole.Normal, coord);
                        _peer.AddMsg(CreatorMessages.CreateAckMsg(_gameModel.MainId, id, msg.MsgSeq), endPoint);
                    }
                    else
                    {
                        _peer.AddMsg(CreatorMessages.CreateErrorMsg(ViewConst.ErrorMsg), endPoint);
                    }
                }
                else
                {
                    var id = _gameModel.AddViewer(msg.Join.PlayerName, endPoint);
                    _peer.AddMsg(CreatorMessages.CreateAckMsg(_gameModel.MainId, id, msg.MsgSeq), endPoint);
                }
                break;
            case GameMessage.TypeOneofCase.State:
                _gameModel.UpdateMap(msg.State.State); 
                _peer.AddMsg(CreatorMessages.CreateAckMsg(_gameModel.MainId, _gameModel.EndPointToId(endPoint), msg.MsgSeq), 
                                gameEvent.IpEndPoint);
                break;
            case GameMessage.TypeOneofCase.Ack:
                _peer.AcceptAck(msg.MsgSeq, endPoint.ToString());
                if (msg.HasReceiverId)
                {
                    _gameModel.SetId(msg.ReceiverId);
                }
                break;
            case GameMessage.TypeOneofCase.Ping:
                var pingReceiverId = _gameModel.EndPointToId(endPoint);
                _peer.AddMsg(CreatorMessages.CreateAckMsg(_gameModel.MainId, pingReceiverId, msg.MsgSeq), endPoint);
                break;
            case GameMessage.TypeOneofCase.Error:
                var errorReceiverId = _gameModel.EndPointToId(endPoint);
                _peer.AddMsg(CreatorMessages.CreateAckMsg(_gameModel.MainId, errorReceiverId, msg.MsgSeq), endPoint);
                _gameWindow.Close();
                _gameWindow.ShowError();
                break;
            case GameMessage.TypeOneofCase.Discover:
                _peer.AddMsg(CreatorMessages.CreateAnnouncementMsg(_gameModel), endPoint);
                break;
            case GameMessage.TypeOneofCase.RoleChange:
                break;
        }
    }

    public GameModel Model => _gameModel;
    public Peer GamePeer => _peer;
}