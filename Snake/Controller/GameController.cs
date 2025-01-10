using System.Net;
using System.Threading;
using Snake.Model;
using Snake.Net;

namespace Snake.Controller;

public class GameController
{
    private GameModel _gameModel;

    private Peer _peer;

    public GameController(string name, Map map)
    {
        _peer = new Peer();
        _gameModel = new GameModel(name, map, _peer.IpEndPoint);
        _peer.AddObserver(_gameModel);
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
}