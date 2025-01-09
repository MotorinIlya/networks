using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Google.Protobuf;
using Snake.Model;

namespace Snake.Net;


public class Peer
{
    private Dictionary<IPEndPoint, DateTime> _activeCopies;

    private Dictionary<IPEndPoint, GameMessage> _games;

    private Queue<GameMessage> _mulMessages;

    private Queue<GameMessage> _messages;

    private MulticastSocket _multicastSocket;

    private UdpClient _unicastSocket;

    private int _unicastPort;

    private IPAddress _unicastAddress;

    private NodeRole _nodeRole;

    public Peer()
    {
        _activeCopies = [];
        _multicastSocket = new();
        _multicastSocket.Bind();
        _unicastSocket = new(NetConst.UnicastPort);
        _unicastPort = ((IPEndPoint)_unicastSocket.Client.LocalEndPoint).Port;
        _unicastAddress = ((IPEndPoint)_unicastSocket.Client.LocalEndPoint).Address;
        _mulMessages = new();
        _messages = new();
        _games = [];
    }


    public void SearchMulticastCopies()
    {
        Thread receiveThread = new Thread(SearchCopies);
        receiveThread.Start();

        Thread deleteThread = new Thread(DeleteDeactiveCopies);
        deleteThread.Start();
    }

    public void SendMsg(GameModel model)
    {
        Thread announceMsgThread = new Thread(() => SendAnnounceMsg(model));
        announceMsgThread.Start();
    }

    private void SearchCopies()
    {
        IPEndPoint? remoteEndPoint = null;

        while (true)
        {
            var buffer = _multicastSocket.Receive(ref remoteEndPoint);
            var message = GameMessage.Parser.ParseFrom(buffer);

            lock (_activeCopies)
            {
                _activeCopies[remoteEndPoint] = DateTime.Now;
            }
            lock (_games)
            {
                _games[remoteEndPoint] = message;
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

    public void SendAnnounceMsg(GameModel model)
    {
        var endPoint = new IPEndPoint(IPAddress.Parse(NetConst.MulticastIP), NetConst.MulticastPort);
        while (true)
        {
            var message = CreatorMessages.createAnnouncementMsg(model);
            var buffer = message.ToByteArray();
            _unicastSocket.Send(buffer, buffer.Length, endPoint);
        }
    }

    public int UnicastPort => _unicastPort;

    public IPEndPoint IpEndPoint => new IPEndPoint(_unicastAddress, _unicastPort);

    public Dictionary<IPEndPoint, GameMessage> Games => _games;
}