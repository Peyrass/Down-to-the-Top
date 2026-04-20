using UnityEngine;
using UnityEngine.EventSystems;

public class SC_ButtonGrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 scaleDimensions = new Vector3(0.2f, 0.2f, 0.2f);
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale += scaleDimensions;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ExitButton(); 
    }

    public void ExitButton()//Hay que ponerlo tambien al clickar botones, de lo contrario los botones no paran de crecer
    {
        transform.localScale -= scaleDimensions;
    }
}
