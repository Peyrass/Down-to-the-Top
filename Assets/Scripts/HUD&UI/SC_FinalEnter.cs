using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_FinalEnter : MonoBehaviour
{
    private Animator animator;
    private PlayerInput input;
    [SerializeField] private SC_ScriptableAudioEvents aEvent;
    [SerializeField] private AudioClip clip;
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
        Cursor.lockState =CursorLockMode.None;
        Cursor.visible = true;
        aEvent.Raise(clip);
        input.actions.FindActionMap("AnyAction").Disable();
        animator.SetTrigger("Enter");
    }

    public void MouseChangeState()
    {
        Cursor.lockState =CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
