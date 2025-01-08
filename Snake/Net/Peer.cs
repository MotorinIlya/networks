using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Snake.Net;


public class Peer
{
    private Dictionary<IPEndPoint, DateTime> _activeCopies;

    private Queue<GameMessage> _mulMessages;

    private Queue<GameMessage> _messages;

    private MulticastSocket _multicastSocket;

    private UdpClient _unicastSocket;

    private int _unicastPort;

    private NodeRole _nodeRole;

    public Peer()
    {
        _activeCopies = [];
        _multicastSocket = new();
        _unicastSocket = new(NetConst.UnicastPort);
        _unicastPort = ((IPEndPoint)_unicastSocket.Client.LocalEndPoint).Port;
        _mulMessages = new();
        _messages = new();
    }

    public void Start()
    {
        _nodeRole = NodeRole.Master;
        _multicastSocket.Bind();
    }

    public void SearchMulticastCopies(Dictionary<IPEndPoint, DateTime> activeCopies)
    {
        Thread receiveThread = new Thread(SearchCopies);
        receiveThread.Start();

        Thread deleteThread = new Thread(DeleteDeactiveCopies);
        deleteThread.Start();
    }

    private void SearchCopies()
    {
        IPEndPoint? remoteEndPoint = null;

        while (true)
        {
            _multicastSocket.Receive(ref remoteEndPoint);

            lock (_activeCopies)
            {
                _activeCopies[remoteEndPoint] = DateTime.Now;
            }
        }
    }

    private void DeleteDeactiveCopies()
    {
        while (true)
        {
            var dateTime = DateTime.Now;
            lock (_activeCopies)
            {
                foreach (var copy in _activeCopies)
                {
                    if (dateTime - copy.Value > NetConst.ExpirationTime)
                    {
                        _activeCopies.Remove(copy.Key);
                    }
                }
            }
        }
    }

    private void SendAnnounceMsg()
    {
        var endPoint = new IPEndPoint(IPAddress.Parse(NetConst.MulticastIP), NetConst.MulticastPort);
        while (true)
        {

        }
    }

    public int UnicastPort => _unicastPort;

    public IPEndPoint IpEndPoint => new IPEndPoint(IPAddress.Parse(NetConst.MyIp), _unicastPort);
}