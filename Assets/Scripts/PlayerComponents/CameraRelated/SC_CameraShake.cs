using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
public class SC_CameraShake : MonoBehaviour
{
    private CinemachineCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin perlin;

    private void Awake()
    {
        virtualCam = GetComponent<CinemachineCamera>();
        perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
        ResetIntensity();
    }

    public void ShakeCam(float shakeIntensity, float shakeTime)
    {
        perlin.AmplitudeGain = shakeIntensity;
        StartCoroutine(WaitTime(shakeTime));
    }

    IEnumerator WaitTime(float shakeTime)
    {
        yield return new WaitForSeconds(shakeTime);
        ResetIntensity();
    }

    private void ResetIntensity()
    {
        perlin.AmplitudeGain = 0f;
    }
}
