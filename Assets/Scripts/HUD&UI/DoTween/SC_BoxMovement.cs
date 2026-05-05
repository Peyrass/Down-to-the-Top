using UnityEngine;
using DG.Tweening;

public class SC_BoxMovement : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private float _cycleLength = 2f;
    void Start()
    {
        //transform.DOMoveX(6, 0).SetDelay(1f).SetEase(Ease.InOutSine);
        transform.DOMove(target.position, _cycleLength).SetDelay(1f);
    }
}
