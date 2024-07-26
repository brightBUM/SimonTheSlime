using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
   
    [SerializeField] Shake[] shakeValues;
    private CinemachineBasicMultiChannelPerlin noiseChannel;
    // Start is called before the first frame update
    void Start()
    {
        noiseChannel = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShakeCamera(Shake shakeValue)
    {
        noiseChannel.m_AmplitudeGain = shakeValue.amplitude;
        noiseChannel.m_FrequencyGain = shakeValue.frequency;
        DOTween.To(() => noiseChannel.m_AmplitudeGain, x => noiseChannel.m_AmplitudeGain = x, 0, shakeValue.duration);
    }
    public void OnPound()
    {
        ShakeCamera(shakeValues[0]);
    }
    public void OnDash()
    {
        ShakeCamera(shakeValues[1]);
    }
    public void OnGrapple()
    {
        ShakeCamera(shakeValues[2]);
    }
    public void OnExplosion()
    {
        ShakeCamera(shakeValues[3]);
    }
    public void OnHit()
    {
        ShakeCamera(shakeValues[4]);
    }
    
}
[System.Serializable]
class Shake
{
    public string name;
    public float amplitude = 2f;
    public float frequency = 6f;
    public float duration = 0.5f;
    
}
