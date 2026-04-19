using System;
using UnityEngine;
using UnityEngine.Events;

public class SC_GameEventListeners : MonoBehaviour
{
  [SerializeField] private SC_ScriptableEvents Event;
  [SerializeField] private UnityEvent Response;

  public void OnEnable()
  {
      Event.RegisterListener(this);
  }

  public void OnDisable()
  {
      Event.UnregisterListener(this);
  }

  public void OnEventRaise()
    {
      Response.Invoke();
    }
}
