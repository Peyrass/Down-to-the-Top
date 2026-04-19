using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Events",fileName = "CallEvent")]
public class SC_ScriptableEvents : ScriptableObject
{
    private readonly List<SC_GameEventListeners> eventListeners = 
        new List<SC_GameEventListeners>();

    public void Raise()
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--) eventListeners[i].OnEventRaise();
    }
    public void RegisterListener(SC_GameEventListeners listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(SC_GameEventListeners listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }

}
