using System;
using UnityEngine;

public struct Empty { }

public abstract class GenericEventChannel<T> : ScriptableObject
{
    public event Action<T> EventRaised;

    public void RaiseEvent(T payload = default)
    {
        if(EventRaised == null)
        {
            return;
        }

        EventRaised.Invoke(payload);
    }
}
