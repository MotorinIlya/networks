using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Snake.Net;


public class Peer
{
    private NodeRole nodeRole;
    
    private Dictionary<IPEndPoint, DateTime> activeCopies;

    private MulticastSocket multicastSocket;

    private UdpClient socket;

    private string? name;

    public Peer()
    {
        activeCopies = [];
        multicastSocket = new();
        socket = new UdpClient(0);
    }

    public void SearchMulticastCopies (Dictionary<IPEndPoint, DateTime> activeCopies)
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
            multicastSocket.Receive(ref remoteEndPoint);

            lock (activeCopies)
            {
                activeCopies[remoteEndPoint] = DateTime.Now;
            }
        }
    }

    private void DeleteDeactiveCopies ()
    {
        while (true)
        {
            var dateTime = DateTime.Now;
            lock (activeCopies)
            {
                foreach (var copy in activeCopies)
                {
                    if (dateTime - copy.Value > Const.ExpirationTime)
                    {
                        activeCopies.Remove(copy.Key);
                    }
                }
            }
        }
    }

    
}