using System;

namespace Snake.Service.Event;

public class ModelEvent(ModelAction action) : ObserverEvent
{
    private ModelAction _action = action;
    public ModelAction Action => _action;
}