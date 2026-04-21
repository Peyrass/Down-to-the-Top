using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "floatEvent", menuName = "SO/FloatEvent")]
public class SC_ScriptableFloatEvent : ScriptableObject
{
    private readonly List<SC_FloatEventListener> eventListeners = 
        new List<SC_FloatEventListener>();

    public void Raise(float value)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--) eventListeners[i].OnEventRaise(value);
    }
    public void RegisterListener(SC_FloatEventListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(SC_FloatEventListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
    
    public void UnregisterAll()
    {
        if(eventListeners.Count!=0)
        {
            for (int i = eventListeners.Count; i > 0; i--)
            {
                eventListeners.RemoveAt(i-1);
            }
        }
    }
}
