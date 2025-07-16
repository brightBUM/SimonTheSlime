using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneAudio : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip swooshClip;
    [SerializeField] AudioClip alarmClip;
    [SerializeField] AudioClip glassBangClip;
    [SerializeField] AudioClip[] popSounds;
    // Start is called before the first frame update
    public void PlaySwooshSFX()
    {
        audioSource.clip = swooshClip;
        audioSource.Play();
    }
    public void PlayTweenObjectSounds(AudioClip clip)
    {
        //if(clip == null)
        //{
        //    audioSource.clip = popSounds[Random.Range(0, popSounds.Length)];
        //}
        //else
        //{
        //    audioSource.clip = clip;

        //}
        audioSource.clip = clip;
        audioSource.Play();
        
    }

    public void PlayClip(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
