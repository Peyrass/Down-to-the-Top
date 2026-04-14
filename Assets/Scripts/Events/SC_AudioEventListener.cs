using UnityEngine;

public class SC_AudioEventListener : MonoBehaviour
{
//Toca aun hacer esto
    [SerializeField] private SC_ScriptableAudioEvents Event;
    [SerializeField] private bool VFX;
    [SerializeField] private AudioSource playAudio;

    public void OnEnable()
    {
        Event.RegisterListener(this);
    }

    public void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaise(AudioClip sound)
    {
        if (VFX)
        {
            playAudio.PlayOneShot(sound);
        }
        else
        {
            playAudio.Stop();
            playAudio.clip = sound;
            playAudio.Play();
        }
        
    }
}
