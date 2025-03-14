using System.Net;
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
    private readonly object _stateLock = new();


    public GameModel Model => _gameModel;
    public Peer GamePeer => _peer;
    public GameState GameState => _gameModel.State;

    //create master
    public GameController(GameWindow gameWindow, 
                            string name, 
                            string gameName, 
                            Map map)
    {
        _gameWindow = gameWindow;
        _peer = new Peer();
        _gameModel = new GameModel(name, gameName, map, _peer.IpEndPoint, _stateLock);
        _peer.AddDelay(_gameModel.Config.StateDelayMs);
        _peer.CheckNodes();
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
        _gameModel = new GameModel(playerName, gameName, map, peer.IpEndPoint, config, _stateLock);
        _peer.AddDelay(_gameModel.Config.StateDelayMs);
        _peer.CheckNodes();
    }

    public void Run()
    {
        _gameModel.Run();
    }

    public void Stop()
    {
        _peer.StopMulticastSocket();
    }

    public void AddPeerObservers(TurnController turnController)
    {
        _peer.AddObserver(this);
        _peer.AddObserver(turnController);
        _gameModel.AddObserver(this);
    }

    public override void Update(ObserverEvent newEvent)
    {
        lock(_stateLock)
        {
            if (newEvent is GameEvent gameEvent)
            {
                UpdateWithMsg(gameEvent);
            }
            else if (newEvent is ModelEvent modelEvent)
            {
                UpdateWithModel(modelEvent);
            }
            else if (newEvent is PeerEvent peerEvent)
            {
                UpdateWithPeer(peerEvent);
            }
        }
    }

    private void UpdateWithMsg(GameEvent gameEvent)
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
                        _peer.AddMsg(CreatorMessages.CreateAckMsg(_gameModel.MainId, msg.MsgSeq), endPoint);
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
                _gameWindow.UpdateStatistics(msg.State.State);
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
                if (msg.RoleChange.HasSenderRole)
                {
                    _gameModel.SetOtherRole(msg.SenderId, msg.RoleChange.SenderRole);
                }
                
                if (msg.RoleChange.HasReceiverRole)
                {
                    _gameModel.SetRole(msg.RoleChange.ReceiverRole);
                    if (msg.RoleChange.ReceiverRole == NodeRole.Master)
                    {
                        var id = _gameModel.SetDeputy();
                        _gameModel.SetActualPlayerId();
                        CreatorMessages.CreateForAllRoleChangeMsg(_peer, _gameModel, _gameModel.MainId, id);
                        Run();
                    }
                }
                _peer.AddMsg(CreatorMessages.CreateAckMsg(_gameModel.MainId, msg.SenderId, msg.MsgSeq), endPoint);
                _gameWindow.UpdateStatistics(_gameModel.State);
                break;
        }
    }

    private void UpdateWithModel(ModelEvent modelEvent)
    {
        GameMessage msg;
        switch (modelEvent.Action)
        {
            // model stop
            case ModelAction.Stop:
                Stop();
                break;
            
            //Update stats for window
            case ModelAction.UpdateStatistics:
                _gameWindow.UpdateStatistics(_gameModel.State);
                PeriodicCreateMsg();
                break;

            //Delete inactive player if he dont send msg long time
            case ModelAction.DeleteInactivePlayer:
                _gameModel.InactivePlayer(modelEvent.RecvId);
                break;
            
            //It is sent for a deputy from the master, that he is now the master and master now is a viewer.
            case ModelAction.SendRoleMsgViewerMaster:
                msg = CreatorMessages.CreateRoleChangeMsg(NodeRole.Viewer, 
                                                                NodeRole.Master, 
                                                                _gameModel.MainId, 
                                                                modelEvent.RecvId);
                _peer.AddMsg(msg, _gameModel.GetEndPoint(modelEvent.RecvId));
                break;

            //It is sent for a normal from master, that he is now the deputy
            case ModelAction.SendRoleMsgRecvDeputy:
                msg = CreatorMessages.CreateRoleChangeMsg(MConst.RoleChange.Receiver, 
                                                                NodeRole.Deputy, 
                                                                _gameModel.MainId, 
                                                                modelEvent.RecvId);
                _peer.AddMsg(msg, _gameModel.GetEndPoint(modelEvent.RecvId));
                break;

            // It is sent for a all from deputy, that he is now new master
            case ModelAction.SendRoleMsgSendMaster:
                CreatorMessages.CreateForAllRoleChangeMsg(_peer, 
                                                            _gameModel, 
                                                            _gameModel.MainId, 
                                                            modelEvent.RecvId);
                break;
        }
    }

    private void UpdateWithPeer(PeerEvent peerEvent)
    {
        switch (peerEvent.PeerAction)
        {
            case PeerAction.UpdateLastInteraction:
                _peer.UpdateLastInteraction(_gameModel.EndPointToId(peerEvent.IPEndPoint));
                break;
            case PeerAction.DeleteInactivePlayer:
                _gameModel.InactivePlayer(_gameModel.EndPointToId(peerEvent.IPEndPoint));
                break;
        }
        
    }

    private void PeriodicCreateMsg()
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
    }
}