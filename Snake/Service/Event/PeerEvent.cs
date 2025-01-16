using System.Net;

namespace Snake.Service.Event;


public class PeerEvent(PeerAction peerAction, IPEndPoint iPEndPoint) : ObserverEvent
{
    private IPEndPoint _iPEndPoint = iPEndPoint;
    public IPEndPoint IPEndPoint => _iPEndPoint;
    private PeerAction _peerAction = peerAction;
    public PeerAction PeerAction => _peerAction;
}