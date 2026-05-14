using UnityEngine;
using DG.Tweening;

public class SC_BoxMovement : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private float _cycleLength = 2f;
    [SerializeField] private SC_ScriptableEvents nextScene;
    [SerializeField] private float newScale;
    [SerializeField] private float wiggleTime;
    private float actualScale;
    
    void Start()
    {
        actualScale = transform.localScale.x;
        //transform.DOMoveX(6, 0).SetDelay(1f).SetEase(Ease.InOutSine);
        transform.DOMove(target.position, _cycleLength).SetDelay(1f).OnComplete(WiggleIn);
    }

    private void WiggleIn()
    {
        transform.DOScale(newScale, wiggleTime).OnComplete(WiggleOut);
    }

    private void WiggleOut()
    {
        transform.DOScale(actualScale, wiggleTime).OnComplete(nextScene.Raise);
    }
    
    
}
