using System.Collections;
using UnityEngine;

public class SC_AutoLoad : MonoBehaviour
{
    [SerializeField] private SC_ScriptableEvents events;
    void Start()
    {
        StartCoroutine(restart());
    }

    private IEnumerator restart()
    {

        yield return new WaitForSeconds(1f);
        events.Raise();
    }
}
