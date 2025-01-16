using System;

namespace Snake.Service.Event;

public class ModelEvent(ModelAction action, int recvId) : ObserverEvent
{
    private ModelAction _action = action;
    public ModelAction Action => _action;
    private int _recvId = recvId;
    public int RecvId => _recvId;
}