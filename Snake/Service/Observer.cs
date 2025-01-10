using Snake.Service.Event;

namespace Snake.Service;


public abstract class Observer
{
    public abstract void Update(GameEvent gameEvent);
}