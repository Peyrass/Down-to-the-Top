using System.Collections;
using TMPro;
using UnityEngine;

public class SC_YokaiDialogLogic : MonoBehaviour
{
    [SerializeField] public TMP_Text characterText;
    [SerializeField] private SC_YokaiText[] dialogLines;
    [SerializeField] private float waitTillNextDialog;
    [SerializeField] private float initialWait;
    private float letterWait = 0.05f;
    private bool dialogueStarted;

    private int lastText = 1;
    private int newText;
    
    void Start()
    {
        characterText.text = string.Empty;
        EnDialogo(); 
    }
    private void EnDialogo() 
    {
        dialogueStarted = true;
        
        
        StartCoroutine(Showline());
    }
    
    private IEnumerator Showline()
    {
        yield return new WaitForSeconds(initialWait);
        
        do
        {
            do
            {
                newText = Random.Range(0, dialogLines.Length);
            } while (lastText == newText);

            lastText = newText;
            
            characterText.text = string.Empty;

            foreach (char ch in dialogLines[newText].dialog)
            {
                characterText.text += ch;
                yield return new WaitForSeconds(letterWait);
            }

            yield return new WaitForSeconds(waitTillNextDialog);

        } while (dialogueStarted);
    }
}
