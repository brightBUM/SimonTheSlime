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
        SetMixervalue();

    }

    public void SetMixervalue()
    {
        var saveLoadManager = SaveLoadManager.Instance;

        audioMixer.SetFloat("MasterVolume", saveLoadManager.GetVolumeControls(0).volumeValue);
        audioMixer.SetFloat("MusicVolume", saveLoadManager.GetVolumeControls(1).volumeValue);
        audioMixer.SetFloat("SFXVolume", saveLoadManager.GetVolumeControls(2).volumeValue);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
