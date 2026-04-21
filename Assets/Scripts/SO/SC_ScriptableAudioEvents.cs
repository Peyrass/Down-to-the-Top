using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/AudioEvents",fileName = "CallEvent")]
public class SC_ScriptableAudioEvents : ScriptableObject
{
    private readonly List<SC_AudioEventListener> eventListeners = 
        new List<SC_AudioEventListener>();

    public void Raise(AudioClip sound)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--) eventListeners[i].OnEventRaise(sound);
    }
    public void RegisterListener(SC_AudioEventListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(SC_AudioEventListener listener)
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
