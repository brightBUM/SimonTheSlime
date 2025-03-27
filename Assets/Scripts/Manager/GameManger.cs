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
    [SerializeField] CharSkinSO charSkinSO;
    public GameConfig gameConfig;

    public bool IsPaused { get; set; }

    private const string privacyPolicyURL = "https://kyodaigameworks.com/privacy-policy/";
    private const string termsAndConditionsURL = "https://kyodaigameworks.com/terms-of-service/";

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

#if UNITY_ANDROID && !UNITY_EDITOR
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
    public Color GetCharSkinColor()
    {
        return charSkinSO.skinList[SaveLoadManager.Instance.playerProfile.equippedSkin].skinColor;
    }
    public void ReloadIntroDelayed()
    {
        DOVirtual.DelayedCall(1f, () =>
        {
            FindAnyObjectByType<IntroCutScene>()?.PrivacyAgreeTrigger();
            ToggleMenuMusic(false);
        });
    }

    public void TogglePauseGame()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
    }

    public void TogglePauseGame(bool value)
    {
        IsPaused = value;
        Time.timeScale = IsPaused ? 0f : 1f;
    }

    public void PrivacyPolicy()
    {
        Application.OpenURL(privacyPolicyURL);
    }
    public void TermsAndConditions()
    {
        Application.OpenURL(termsAndConditionsURL);
    }

}
