using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] Image[] volumeStateUI;
    [SerializeField] Slider[] volumeValueUI;
    [SerializeField] Sprite toggleOnUI;
    [SerializeField] Sprite toggleOffUI;
    // Start is called before the first frame update
    private void OnEnable()
    {
        
        for (int i = 0; i < volumeStateUI.Length; i++)
        { 
            volumeStateUI[i].sprite = SaveLoadManager.Instance.GetVolumeControls(i).volumeState?toggleOnUI:toggleOffUI; 
            volumeValueUI[i].value = SaveLoadManager.Instance.GetVolumeControls(i).volumeValue;
            switch(i)
            {
                case 0:
                    audioMixer.SetFloat("MasterVolume", Mathf.Log10(volumeValueUI[i].value) * 20f);
                    break;
                case 1:
                    audioMixer.SetFloat("MusicVolume", Mathf.Log10(volumeValueUI[i].value) * 20f);
                    break;
                case 2:
                    audioMixer.SetFloat("SFXVolume", Mathf.Log10(volumeValueUI[i].value) * 20f);
                break;
            }
        }
    }
    
    public void SetVolumeToggle(int index)
    {
        SaveLoadManager.Instance.ToggleVolumeState(index);
        volumeStateUI[index].sprite = SaveLoadManager.Instance.GetVolumeControls(index).volumeState ? toggleOnUI : toggleOffUI;
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", /*Mathf.Log10(value) * 20f*/value);
        SaveLoadManager.Instance.SetVolumeValue(0, value);
    }
    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume",/*Mathf.Log10(value) * 20f*/value);
        SaveLoadManager.Instance.SetVolumeValue(1, value);
    }
    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", /*Mathf.Log10(value) * 20f*/value);
        SaveLoadManager.Instance.SetVolumeValue(2, value);
    }
    public void SaveSettings()
    {
        SaveLoadManager.Instance.SaveGame();
    }
    private void OnDisable()
    {
        SaveSettings();
    }
}
