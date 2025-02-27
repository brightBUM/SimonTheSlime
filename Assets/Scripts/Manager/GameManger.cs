using CutScene;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class GameManger : MonoBehaviour
{
    [HideInInspector] public int selectedIndex;
    public readonly string LOADINGSCENE = "SceneTrans";
    public static GameManger Instance;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource menuAudioSource;
    
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
        ToggleMenuMusic(false);

#if UNITY_ANDROID
        Application.targetFrameRate = 60;
        
#endif
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
            menuAudioSource?.UnPause();
        }
        else
        {
            menuAudioSource?.Pause();
        }
    }

    public void ReloadIntroDelayed()
    {
        DOVirtual.DelayedCall(1f, () =>
        {
            FindAnyObjectByType<IntroCutScene>()?.CheckForSaveLoad(false);
            ToggleMenuMusic(false);
        });
    }
}
