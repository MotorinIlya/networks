using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    private Dictionary<IPEndPoint, GameMessage> _games;
    private ConcurrentQueue<(GameMessage, IPEndPoint, bool)> _sendMessages;
    private ConcurrentDictionary<(long, string), ManualResetEvent> _pendingAcks;
    private ConcurrentDictionary<int, DateTime> _lastInteraction;
    private MulticastSocket _multicastSocket;
    private UdpClient _unicastSocket;
    private int _unicastPort;
    private IPAddress _unicastAddress;
    private int _stateDelayMs = NetConst.StartDelay;
    private bool _multicastIsOn = false;
    private bool _isRun = true;


    public int UnicastPort => _unicastPort;
    public IPEndPoint IpEndPoint => new IPEndPoint(_unicastAddress, _unicastPort);
    public Dictionary<IPEndPoint, GameMessage> Games => _games;


    public Peer()
    {
        _multicastSocket = new();
        _multicastSocket.Bind();
        _unicastSocket = new(NetConst.UnicastPort);
        _unicastPort = _unicastSocket.Client.LocalEndPoint is IPEndPoint endPoint 
                            ? endPoint.Port 
                            : throw new Exception(); 
        _unicastAddress = GetterIP.GetLocalIpAddress();
        _sendMessages = [];
        _pendingAcks = [];
        _games = [];
        _lastInteraction = [];

        var sendThread = new Thread(SendMsg);
        sendThread.Start();

        var receiveThread = new Thread(ReceiveMsg);
        receiveThread.Start();
    }

    public void AddDelay(int delay)
    {
        _stateDelayMs = delay;
    }

    public void CheckNodes()
    {
        var inactiveThread = new Thread(CheckInactiveNodes);
        inactiveThread.Start();
    }

    public void SearchMulticastCopies()
    {
        _multicastIsOn = true;
        var receiveThread = new Thread(SearchCopies);
        receiveThread.Start();
    }

    private void SearchCopies()
    {
        IPEndPoint? remoteEndPoint = null;

        while (_multicastIsOn)
        {
            var buffer = _multicastSocket.Receive(ref remoteEndPoint);
            var message = GameMessage.Parser.ParseFrom(buffer);
            _games[remoteEndPoint] = message;
        }
    }

    public void StopMulticastSocket()
    {
        _multicastIsOn = false;
    }

    public void Stop()
    {
        StopMulticastSocket();
        _isRun = false;
    }

    public void AddMsg(GameMessage msg, IPEndPoint remoteEndPoint) => 
                                        _sendMessages.Enqueue((msg, remoteEndPoint, false));

    public void SendMsg()
    {
        while (_isRun)
        {
            if (_sendMessages.TryDequeue(out var message))
            {
                // Send message
                var msg = message.Item1;
                var remoteEndPoint = message.Item2;
                var repeat = message.Item3;
                var buffer = msg.ToByteArray();
                _unicastSocket.Send(buffer, buffer.Length, remoteEndPoint);

                if (string.Compare(remoteEndPoint.Address.ToString(), NetConst.MulticastIP) != 0)
                {
                    Update(new PeerEvent(PeerAction.UpdateLastInteraction, remoteEndPoint));
                }
                
                if (msg.TypeCase != GameMessage.TypeOneofCase.Ack 
                    && msg.TypeCase != GameMessage.TypeOneofCase.Announcement)
                { 
                    var resetEvent = new ManualResetEvent(false);
                    _pendingAcks[(msg.MsgSeq, remoteEndPoint.ToString())] = resetEvent;

                    //wait ack message
                    var ackReceived = resetEvent.WaitOne(_stateDelayMs / 10);

                    if (!ackReceived)
                    {
                        if (!repeat)
                        {
                            _sendMessages.Enqueue((msg, remoteEndPoint, true));
                        }
                        else
                        {
                            AddMsg(CreatorMessages.CreatePingMsg(), remoteEndPoint);
                        }
                    }
                }
            }
            else
            {
                Thread.Sleep(1);
            }
        }
    }

    public void ReceiveMsg()
    {
        while (_isRun)
        {
            IPEndPoint? remoteEndPoint = null;
            byte[] buffer;
            try
            {
                buffer = _unicastSocket.Receive(ref remoteEndPoint);
            }
            catch (Exception)
            {
                if (remoteEndPoint is IPEndPoint remote)
                {
                    Update(new PeerEvent(PeerAction.DeleteInactivePlayer, remote));
                }
                continue;
            }
            
            var message = GameMessage.Parser.ParseFrom(buffer);
            Update(new GameEvent(message, remoteEndPoint));
            Update(new PeerEvent(PeerAction.UpdateLastInteraction, remoteEndPoint));
        }
    }

    public void AcceptAck(long msgSeq, string ipEndPoint)
    {
        if (_pendingAcks.TryGetValue((msgSeq, ipEndPoint), out var resetEvent))
        {
            resetEvent.Set();
        }
    }

    public void UpdateLastInteraction(int id)
    {
        if (id != 0)
        {
            _lastInteraction[id] = DateTime.Now;
        }
    }

    public void CheckInactiveNodes()
    {
        while (_isRun)
        {
            var timeout = TimeSpan.FromMilliseconds(_stateDelayMs);
            var now = DateTime.Now;
            var inactiveNodes = _lastInteraction
                .Where(pair => now - pair.Value > timeout)
                .Select(pair => pair.Key)
                .ToList();

            foreach (var nodeId in inactiveNodes)
            {
                Update(new ModelEvent(ModelAction.DeleteInactivePlayer, nodeId));
                _lastInteraction.Remove(nodeId, out _);
            }

            Thread.Sleep((int)(_stateDelayMs * 0.4));
        }
    }
}