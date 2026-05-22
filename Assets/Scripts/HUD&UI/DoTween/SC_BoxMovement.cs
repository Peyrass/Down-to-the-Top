using System.Collections;
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

    [Header("Audio")] 
    [SerializeField] private SC_ScriptableAudioEvents aEvents;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private float soundWait;
    
    void Start()
    {
        actualScale = transform.localScale.x;
        //transform.DOMoveX(6, 0).SetDelay(1f).SetEase(Ease.InOutSine);
        StartCoroutine(MoveSound());
        transform.DOMove(target.position, _cycleLength).SetDelay(1f).OnComplete(WiggleIn);
    }

    private void WiggleIn()
    {
        transform.DOScale(newScale, wiggleTime).OnComplete(WiggleOut);
        
    }

    private void WiggleOut()
    {
        aEvents.Raise(clips[1]);
        transform.DOScale(actualScale, wiggleTime).OnComplete(nextScene.Raise);
    }

    private IEnumerator MoveSound()
    {
        yield return new WaitForSeconds(soundWait);
        aEvents.Raise(clips[0]);
    }
    
    
}
