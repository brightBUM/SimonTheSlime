using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSaveLoad : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    private void Start()
    {
        SetMixervalue();
    }
    public void SetMixervalue()
    {
        var saveLoadManager = SaveLoadManager.Instance;

        audioMixer.SetFloat("MasterVolume", saveLoadManager.GetVolumeControls(0).volumeValue);
        audioMixer.SetFloat("MusicVolume", saveLoadManager.GetVolumeControls(1).volumeValue);
        audioMixer.SetFloat("SFXVolume", saveLoadManager.GetVolumeControls(2).volumeValue);

    }
}
