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
        audioMixer.SetFloat("MasterVolume", masterVolumeControl.volumeState ? masterVolumeControl.volumeValue : -80f);

        var musicVolumeControl = saveLoadManager.GetVolumeControls(1);
        audioMixer.SetFloat("MusicVolume", musicVolumeControl.volumeState ? musicVolumeControl.volumeValue : -80f);

        var sfxVolumeControl = saveLoadManager.GetVolumeControls(2);
        audioMixer.SetFloat("SFXVolume", sfxVolumeControl.volumeState ? sfxVolumeControl.volumeValue : -80f);
        
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
