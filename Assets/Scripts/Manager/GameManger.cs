using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    [HideInInspector] public int selectedIndex;
    public readonly string LOADINGSCENE = "SceneTrans";
    public static GameManger Instance;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource menuAudioSource;
    public Texture2D hoverCursor;
    CursorMode cursorMode = CursorMode.Auto;
    Vector2 hotSpot = Vector2.zero;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        //Debug.Log("game manager , start");
    }
    void Start()
    {
        SaveLoadManager.Instance.InitFileSystem();
        SetMixervalueFromFile();

    }

    public void SetMixervalueFromFile()
    {
        var saveLoadManager = SaveLoadManager.Instance;

        var masterVolumeControl = saveLoadManager.GetVolumeControls(0);
        audioMixer.SetFloat("MasterVolume", masterVolumeControl.volumeState ? Mathf.Log10(masterVolumeControl.volumeValue) * 20f : -80f);

        var musicVolumeControl = saveLoadManager.GetVolumeControls(1);
        audioMixer.SetFloat("MusicVolume", musicVolumeControl.volumeState ? Mathf.Log10(musicVolumeControl.volumeValue) * 20f : -80f);

        var sfxVolumeControl = saveLoadManager.GetVolumeControls(2);
        audioMixer.SetFloat("SFXVolume", sfxVolumeControl.volumeState ? Mathf.Log10(sfxVolumeControl.volumeValue) * 20f : -80f);
        
    }
    public void ToggleMenuMusic(bool value)
    {
        if(value)
        {
            menuAudioSource.UnPause();
        }
        else
        {
            menuAudioSource.Pause();
        }
    }

    public void SwapCursor(bool value)
    {
        if (value)
        {
            Cursor.SetCursor(hoverCursor, hotSpot, cursorMode);
        }
        else
        {
            Cursor.SetCursor(null, hotSpot, cursorMode);
        }
    }

    
}
