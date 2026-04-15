using UnityEngine;
using System.Collections.Generic;

public class SC_AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] List<SC_AnimationEvent> animationEvents = new();

    public void OnAnimationEventTriggered(string eventName) 
    {
        SC_AnimationEvent matchingEvent = animationEvents.Find(se => se.eventName == eventName);
        matchingEvent?.OnAnimationEvent?.Invoke();
    }
}
