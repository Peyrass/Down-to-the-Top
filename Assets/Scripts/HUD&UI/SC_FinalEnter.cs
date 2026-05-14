using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_FinalEnter : MonoBehaviour
{
    private Animator animator;
    private PlayerInput input;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
    }
    
    private void OnEnable()
    {
        input.actions["Anything"].started += NextPanel;
    }
    private void OnDisable()
    {
        input.actions["Anything"].started -= NextPanel;
    }
    
    private void NextPanel(InputAction.CallbackContext obj)
    {
        input.actions.FindActionMap("AnyAction").Disable();
        animator.SetTrigger("Enter");
    }
}
