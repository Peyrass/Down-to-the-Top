using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SC_SceneLogic : MonoBehaviour
{
    [SerializeField] private float transitionWait;
    [SerializeField] private UnityEvent onCall;

    [SerializeField] private UnityEvent onRemoveEvent;
    [SerializeField] private UnityEvent onRemoveAudioEvent;
    [SerializeField] private UnityEvent onRemoveFloatEvent;
    
    public void LoadGameScene(int index)
    {
        StartCoroutine(WaitTillLoadScene(index));
    }
    
    public void ExitButton()
    {
        Application.Quit();
    }

    private IEnumerator WaitTillLoadScene(int index)
    {
        onCall.Invoke();
        yield return new WaitForSeconds(transitionWait);
        onRemoveEvent.Invoke();
        onRemoveAudioEvent.Invoke();
        onRemoveFloatEvent.Invoke();
        SceneManager.LoadScene(index);
    }
    private IEnumerator WaitTillQuit()
    {
        onCall.Invoke();
        yield return new WaitForSeconds(transitionWait);
        onRemoveEvent.Invoke();
        onRemoveAudioEvent.Invoke();
        onRemoveFloatEvent.Invoke();
        Application.Quit();
    }
}
