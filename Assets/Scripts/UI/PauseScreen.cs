using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] Image[] volumeStateUI;
    [SerializeField] Slider[] volumeValueUI;
    [SerializeField] Slider holdTimeSlider;
    [SerializeField] Slider doubleTapTimeSlider;
    [SerializeField] TextMeshProUGUI holdtimeText;
    [SerializeField] TextMeshProUGUI doubleTapText;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Sprite toggleOnUI;
    [SerializeField] Sprite toggleOffUI;
    // Start is called before the first frame update
    private void OnEnable()
    {
        LoadSettings();
        Debug.Log("loading audio prefs");
        //holdTimeSlider.onValueChanged.AddListener(delegate { SetHoldTime(); });
        //doubleTapTimeSlider.onValueChanged.AddListener(delegate { SetDoubleTapTime(); });
    }
    
    public void LoadSettings()
    {
        for (int i = 0; i < volumeStateUI.Length; i++)
        {
            volumeStateUI[i].sprite = SaveLoadManager.Instance.GetVolumeControls(i).volumeState ? toggleOnUI : toggleOffUI;
            volumeValueUI[i].value = SaveLoadManager.Instance.GetVolumeControls(i).volumeValue;
            //audioMixer.SetFloat("MasterVolume", Mathf.Log10(volumeValueUI[0].value) * 20f);
            //audioMixer.SetFloat("MusicVolume", Mathf.Log10(volumeValueUI[1].value) * 20f);
            //audioMixer.SetFloat("SFXVolume", Mathf.Log10(volumeValueUI[2].value) * 20f);

        }
    }

    public void SetVolumeToggle(int index)
    {
        var volumeControl = SaveLoadManager.Instance.GetVolumeControls(index);
        volumeControl.volumeState = !volumeControl.volumeState;
        volumeStateUI[index].sprite = volumeControl.volumeState ? toggleOnUI : toggleOffUI;

        var convertedValue = volumeControl.volumeState ? Mathf.Log10(volumeControl.volumeValue) * 20f : -80f;
        switch (index)
        {
            case 0:
                audioMixer.SetFloat("MasterVolume", convertedValue);
                volumeValueUI[0].value = volumeControl.volumeValue;
                break;
            case 1:
                audioMixer.SetFloat("MusicVolume", convertedValue);
                volumeValueUI[1].value = volumeControl.volumeValue;
                break;
            case 2:
                audioMixer.SetFloat("SFXVolume", convertedValue);
                volumeValueUI[2].value = volumeControl.volumeValue;
                break;
        }

        SaveLoadManager.Instance.SetVolumeState(index,volumeControl.volumeState);
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20f); //since mixer ranges from 0 to -80
        SaveLoadManager.Instance.SetVolumeValue(0, value);
    }
    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20f);
        SaveLoadManager.Instance.SetVolumeValue(1, value);
    }
    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);
        SaveLoadManager.Instance.SetVolumeValue(2, value);
    }

    public void SetHoldTime()
    { 
        playerInput.minHoldingTime = holdTimeSlider.value;
        holdtimeText.text = holdTimeSlider.value.ToString("N2");
    }
    public void SetDoubleTapTime()
    {
        playerInput.doubleTapMaxTime = doubleTapTimeSlider.value;
        doubleTapText.text = doubleTapTimeSlider.value.ToString("N2");
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
