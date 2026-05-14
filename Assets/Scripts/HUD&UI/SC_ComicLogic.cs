using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SC_ComicLogic : MonoBehaviour
{
    [SerializeField] private Image[] images;
    [SerializeField] private SC_ScriptableEvents nextScene;
    private PlayerInput input;
    private int order;
    private bool permision = true;
    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        foreach (var image in images)
        {
            image.DOFade(0, 0);
        }
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
        if (permision)
        {
            if (images.Length > order)
            {
                permision = false;
                images[order].DOFade(1, 1f).OnComplete(ReturnBool);
                order++;
            }
            else
            {
                input.actions.FindActionMap("AnyAction").Disable();
                nextScene.Raise();
            }
        }
    }

    private void ReturnBool()
    {
        permision = true;
    }

}
