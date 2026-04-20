using UnityEngine;

public class SC_FloatEventListener : MonoBehaviour
{
    [SerializeField] private SC_ScriptableFloatEvent Event;
    public void OnEnable()
    {
        Event.RegisterListener(this);
    }

    public void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public virtual void OnEventRaise(float value)
    {
        throw new System.NotImplementedException();
    }
}
