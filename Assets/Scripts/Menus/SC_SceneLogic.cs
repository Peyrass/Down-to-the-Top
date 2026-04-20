using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SC_SceneLogic : MonoBehaviour
{
    [SerializeField] private float transitionWait;
    [SerializeField] private UnityEvent onCall;
    
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
        SceneManager.LoadScene(index);
    }
    private IEnumerator WaitTillQuit()
    {
        onCall.Invoke();
        yield return new WaitForSeconds(transitionWait);
        Application.Quit();
    }
}
