using System.Collections.Generic;
using Snake.Service.Event;

namespace Snake.Service;

public class Observable()
{
    private List<Observer> _observers = [];
    public void AddObserver(Observer observer) => _observers.Add(observer);

    public void RemoveObserver(Observer observer) => _observers.Remove(observer);

    public void Update(GameEvent gameEvent)
    {
        foreach(var observer in _observers)
        {
            observer.Update(gameEvent);
        }
    }
}