using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SC_YokaiDialogLogic : MonoBehaviour
{
    [SerializeField] private TMP_Text characterText;
    [SerializeField] private SC_YokaiText initialDialog;
    [SerializeField] private SC_YokaiText[] skillLines;
    [SerializeField] private SC_YokaiText[] healthlines;
    [SerializeField] private SC_YokaiText[] batterylines;
    [SerializeField] private SC_YokaiText[] randomlines;
    [SerializeField] private float waitTillNextDialog;
    [SerializeField] private float waitTillNextText;
    [SerializeField] private float initialWait;
    [SerializeField] private float randomWait;

    [SerializeField] private Animator anim;

    [SerializeField] private GameObject box;
    
    private float letterWait = 0.05f;
    private bool dialogueStarted;

    private int lastText = 1;
    private int newText;
    
    void Awake()
    {
        characterText.text = string.Empty;
        StartingDialog();
        
        if (randomlines != null)
        {
            StartCoroutine(RandomCuotes());
        }
    }

    private void StartingDialog()
    {
        if (initialDialog != null)
        {
            StartCoroutine(InDialog(initialDialog));
        }
        
    }

    public void CallDialog(int cuoteTipe)
    {
        switch (cuoteTipe)
        {
            case 1:
                if (healthlines!= null)
                {
                    RandomDialog(healthlines);
                }
                break;
            case 2:
                if (batterylines!= null)
                {
                    RandomDialog(batterylines);
                }
                break;
            case 3:
                if (skillLines!= null)
                {
                    RandomDialog(skillLines);
                }
                break;
            
            default:
                break;
        }
        
        
        
    }
    private IEnumerator InDialog(SC_YokaiText texto) 
    {
        box.SetActive(true);
        dialogueStarted = true;
        int i = 0;
        
        do
        {
            characterText.text = string.Empty;
            anim.SetTrigger("StartText");
            foreach (char character in texto.dialog[i])
            {
                characterText.text += character;
                yield return new WaitForSeconds(letterWait);
            }
            anim.SetTrigger("EndText");
            i++;
            yield return new WaitForSeconds(waitTillNextText);
        } while (i < texto.dialog.Length);
        
        box.SetActive(false);
        yield return waitTillNextDialog;
        dialogueStarted = false;
    }

    private IEnumerator RandomCuotes()
    {
        do
        {
           RandomDialog(randomlines);

            yield return new WaitForSeconds(randomWait);
            
        } while (1 > 0);
    }
    

    private void RandomDialog(SC_YokaiText[] cuoteGroup)
    {
        if (cuoteGroup.Length > 1)
        {
            do
            {
                newText = Random.Range(0, cuoteGroup.Length);
            } while (lastText == newText);
        }
        
        lastText = newText;

        if (dialogueStarted == false)
        {
            StartCoroutine(InDialog(cuoteGroup[newText]));
        }
    }
    
    /*
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
    }*/
}
