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

    private UdpClient _multicastSocket;

    private UdpClient _socket;

    public Peer()
    {
        _activeCopies = [];
        _multicastSocket = new();
        _socket = new UdpClient(0);
        _mulMessages = new();
        _messages = new();
    }

    public void SearchMulticastCopies (Dictionary<IPEndPoint, DateTime> _activeCopies)
    {
        Thread receiveThread = new Thread(SearchCopies);
        receiveThread.Start();

        Thread deleteThread = new Thread(DeleteDeactiveCopies);
        deleteThread.Start();
    }

    private void SearchCopies ()
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

    private void DeleteDeactiveCopies ()
    {
        while (true)
        {
            var dateTime = DateTime.Now;
            lock (_activeCopies)
            {
                foreach (var copy in _activeCopies)
                {
                    if (dateTime - copy.Value > Const.ExpirationTime)
                    {
                        _activeCopies.Remove(copy.Key);
                    }
                }
            }
        }
    }

    
}