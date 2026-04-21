using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SC_CallPauseMessager : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private UnityEvent onPause;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

    }

    private void OnEnable()
    {
        playerInput.actions["pauseInput"].started += CallPause;
    }

    private void OnDisable()
    {
        playerInput.actions["pauseInput"].started += CallPause;

    }

    private void CallPause(InputAction.CallbackContext context)
    {
        onPause.Invoke();
        
    }
}
