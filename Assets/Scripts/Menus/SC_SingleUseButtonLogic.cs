using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_SingleUseButtonLogic : MonoBehaviour
{
    [SerializeField] private float transitionWait;
    
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
        yield return new WaitForSeconds(transitionWait);
        SceneManager.LoadScene(index);
    }
}
