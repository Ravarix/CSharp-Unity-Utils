
using System;
using System.Collections.Generic;

public class SmartEvent<T>
{
    private readonly SmartLinkedList<Action<T>> _actions = new SmartLinkedList<Action<T>>();

    public int Count => _actions.Count;

    public void Invoke(T t)
    {
        foreach (var action in _actions)
            action(t);
    }

    public void Add(Action<T> action) => _actions.AddLast(action);

    public void Remove(Action<T> action) => _actions.Remove(action);

    public void Clear() => _actions.Clear();

    public static SmartEvent<T> operator +(SmartEvent<T> smartEvent, Action<T> action)
    {
        smartEvent._actions.AddLast(action);
        return smartEvent;
    }

    public static SmartEvent<T> operator -(SmartEvent<T> smartEvent, Action<T> action)
    {
        smartEvent._actions.Remove(action);
        return smartEvent;
    }
}