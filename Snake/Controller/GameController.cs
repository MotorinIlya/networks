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
    }

    public void Start()
    {
        _peer.SendMsg(_gameModel);
    }

    public void Join()
    {
        _peer.SearchMulticastCopies();
    }
}