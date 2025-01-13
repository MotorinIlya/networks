using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Google.Protobuf;
using Snake.Model;
using Snake.Service;
using Snake.Service.Event;

namespace Snake.Net;


public class Peer : Observable
{
    //private Dictionary<IPEndPoint, DateTime> _activeCopies;

    private Dictionary<IPEndPoint, GameMessage> _games;

    private ConcurrentQueue<(GameMessage, IPEndPoint)> _sendMessages;

    private MulticastSocket _multicastSocket;

    private UdpClient _unicastSocket;

    private int _unicastPort;

    private IPAddress _unicastAddress;

    public Peer()
    {
        //_activeCopies = [];
        _multicastSocket = new();
        _multicastSocket.Bind();
        _unicastSocket = new(NetConst.UnicastPort);
        _unicastPort = ((IPEndPoint)_unicastSocket.Client.LocalEndPoint).Port;
        _unicastAddress = ((IPEndPoint)_unicastSocket.Client.LocalEndPoint).Address;
        _sendMessages = new();
        _games = [];

        var sendThread = new Thread(SendMsg);
        sendThread.Start();

        var receiveThread = new Thread(ReceiveMsg);
        receiveThread.Start();
    }


    public void SearchMulticastCopies()
    {
        Thread receiveThread = new Thread(SearchCopies);
        receiveThread.Start();

        // Thread deleteThread = new Thread(DeleteDeactiveCopies);
        // deleteThread.Start();
    }

    private void SearchCopies()
    {
        IPEndPoint? remoteEndPoint = null;

        while (true)
        {
            var buffer = _multicastSocket.Receive(ref remoteEndPoint);
            var message = GameMessage.Parser.ParseFrom(buffer);

            // lock (_activeCopies)
            // {
            //     _activeCopies[remoteEndPoint] = DateTime.Now;
            // }
            lock (_games)
            {
                _games[remoteEndPoint] = message;
            }
        }
    }

    // private void DeleteDeactiveCopies()
    // {
    //     while (true)
    //     {
    //         var dateTime = DateTime.Now;
    //         lock (_activeCopies)
    //         {
    //             foreach (var copy in _activeCopies)
    //             {
    //                 if (dateTime - copy.Value > NetConst.ExpirationTime)
    //                 {
    //                     _activeCopies.Remove(copy.Key);
    //                 }
    //             }
    //         }
    //     }
    // }

    public void AddMsg(GameMessage msg, IPEndPoint remoteEndPoint) => _sendMessages.Enqueue((msg, remoteEndPoint));

    public void SendMsg()
    {
        while (true)
        {
            if (_sendMessages.TryDequeue(out var message))
            {
                var msg = message.Item1;
                var remoteEndPoint = message.Item2;
                var buffer = msg.ToByteArray();
                _unicastSocket.Send(buffer, buffer.Length, remoteEndPoint);
            }
            else
            {
                Thread.Sleep(50);
            }
        }
    }

    public void ReceiveMsg()
    {
        IPEndPoint? remoteEndPoint = null;
        while (true)
        {
            var buffer = _unicastSocket.Receive(ref remoteEndPoint);
            var message = GameMessage.Parser.ParseFrom(buffer);
            Update(new GameEvent(message, remoteEndPoint));
        }
    }

    public int UnicastPort => _unicastPort;

    public IPEndPoint IpEndPoint => new IPEndPoint(_unicastAddress, _unicastPort);

    public Dictionary<IPEndPoint, GameMessage> Games => _games;
}