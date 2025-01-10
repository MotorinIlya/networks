using System.Net;
using System.Threading;
using Snake.Model;
using Snake.Net;
using Snake.Service;
using Snake.Service.Event;

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
    }

    public GameController(string playerName, string gameName, GameAnnouncement config, Peer peer, Map map)
    {
        _peer = new Peer();
        _gameModel = new GameModel(playerName, gameName, map, peer.IpEndPoint, config);
    }

    public void Start()
    {
        var createMsgThread = new Thread(periodicCreateAnnMsg);
        createMsgThread.Start();
    }

    public void Join()
    {
        _peer.SearchMulticastCopies();
    }

    private void periodicCreateAnnMsg()
    {
        while (true)
        {
            var msg = CreatorMessages.CreateAnnouncementMsg(_gameModel);
            _peer.AddMsg(msg, new IPEndPoint(IPAddress.Parse(NetConst.MulticastIP), NetConst.MulticastPort));
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
                _peer.AddMsg(CreatorMessages.CreateAckMsg(), gameEvent.IpEndPoint);
                break;
            
        }

    }
}