using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip noBulletTImeSfx;
    [SerializeField] private AudioClip explosionSFX;
    [SerializeField] private AudioClip poundSFx;
    [SerializeField] private AudioClip dashSFx;
    [SerializeField] private AudioClip ghostRespawnSFx;
    [SerializeField] private AudioClip gateUnlockLoopSFx;
    [SerializeField] private AudioClip sloMoTimerSFx;
    [SerializeField] private AudioClip flagCheckPointSFx;
    [SerializeField] private AudioClip acidSplashSFx;
    [SerializeField] private AudioClip slimeSplashSFx;
    [SerializeField] private AudioClip brickBreakSFx;
    [SerializeField] private AudioClip playerOnHitSFx;
    [SerializeField] private AudioClip levelCompleteSFx;
    [SerializeField] private AudioClip grappleRopeSFx;
    [SerializeField] private AudioClip grapplePullSFx;
    [SerializeField] private AudioClip coinBangSFx;
    [SerializeField] private AudioClip erectPlatformSFx;
    [SerializeField] private AudioClip switchPlatformSFx;
    [SerializeField] private AudioClip timeOrbCollectSFx;
    [SerializeField] private AudioClip resetPuzzleSFx;
    [SerializeField] private AudioClip lightBlinkSFx;
    [SerializeField] AudioSource ghostSource;
    [SerializeField] AudioSource gateUnlockSource;
    [SerializeField] private AudioClip[] slimeStickSFX;
    [SerializeField] private AudioClip[] coinCollectSfx;

    [Header("Creatures/UI")]
    [SerializeField] private AudioClip creaturePickup;
    [SerializeField] private AudioClip creaturePickCancel;
    [SerializeField] private AudioClip creatureDrop;
    [SerializeField] private AudioClip floorScrollUI;
    [SerializeField] private AudioClip floorScrollSnapUI;
    [SerializeField] private AudioClip newCreatureRevealUI;
    [SerializeField] private AudioClip existingCreatureRevealUI;

    [Header("Creatures/Dungeon")]
    [SerializeField] private AudioClip cagedPodBreak;
    [SerializeField] private AudioClip cursedCurrencyCollect;
    [SerializeField] private AudioClip tileBreak;
    [SerializeField] private AudioClip tileRegroup;
    [SerializeField] private AudioClip fallingSound;
    [SerializeField] private AudioClip pipeSound;
    [SerializeField] private AudioClip vaccumPullSound;
    [SerializeField] private AudioClip creatureCollect;
    [SerializeField] private AudioClip[] slamSFX;
    [SerializeField] private AudioClip[] creaturesAdded;
    [SerializeField] private AudioClip[] creaturesInventory;
    [SerializeField] private AudioClip[] starsCollect;
    private AudioSource activeSource;

    public static SoundManager Instance;

    private List<string> levelAudioPath = new List<string>
    {
        "lv1 Funky Chunk",
        "lv2 Funky Boxstep",
        "lv3 Funin and Sunin",
        "lv4 Funk Game Loop"
    };
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
    }
    // Start is called before the first frame update
    void Start()
    {
        ghostSource.clip = ghostRespawnSFx;
        //gateUnlockSource.clip = gateUnlockLoopSFx;
    }
    public AudioClip GetLevelMusic(int index)
    {
        AudioClip audioClip = (AudioClip)Resources.Load("Audio/" + levelAudioPath[index]);
        return audioClip;
    }
    private void PlayClip(AudioClip clip)
    {
        activeSource = GetIdleSource();
        activeSource.clip = clip;
        activeSource.pitch = 1;
        activeSource.Play();
        
        ObjectPoolManager.Instance.Despawn(activeSource.gameObject, activeSource.clip.length);
    }
    private void PlayClipPitchRandomized(AudioClip clip)
    {
        activeSource = GetIdleSource();
        activeSource.clip = clip;
        activeSource.Play();
        activeSource.pitch = Random.Range(1.0f, 3.0f);
        ObjectPoolManager.Instance.Despawn(activeSource.gameObject, activeSource.clip.length);
    }
    private AudioSource GetIdleSource()
    {
        //returns the audioSource that isnt playing
        var audioGameObject = ObjectPoolManager.Instance.Spawn(1, Vector3.zero, Quaternion.identity);
        return audioGameObject.GetComponent<AudioSource>();
    }
    
    public void PlayStickSFx()
    {
        PlayClip(slimeStickSFX[Random.Range(0, slimeStickSFX.Length)]);
    }
    public void PlayGrappleRopeSFX()
    {
        PlayClip(grappleRopeSFx);
    }
    public void PlaylightBlinkSFX()
    {
        PlayClip(lightBlinkSFx);
    }
    public void PlaySlimeSplashSFX()
    {
        PlayClip(slimeSplashSFx);
    }
    public void PlayResetPuzzleSFX()
    {
        PlayClip(resetPuzzleSFx);
    }
    public void PlayDashSFX()
    {
        PlayClip(dashSFx);
    }
    public void PlayTimeOrbCollectSFX()
    {
        PlayClip(timeOrbCollectSFx);
    }
    public void PlayOutofBulletTimeSFX()
    {
        PlayClip(noBulletTImeSfx);
    }
    public void PlayGrapplePullSFX()
    {
        PlayClip(grapplePullSFx);
    }
    public void PlayCollectibleSFx()
    {
        PlayClip(coinCollectSfx[Random.Range(0,coinCollectSfx.Length)]);
    }
    public void PlaySwitchPlatformSFX()
    {
        PlayClip(switchPlatformSFx);
    }
    public void PlayGearUnlockLoopSFX()
    {

    }
    public void PlayErectPlatformSFx()
    {
        PlayClip(erectPlatformSFx);
    }
    public void PlaySloMoTimer()
    {
        PlayClip(sloMoTimerSFx);
    }
    public void PlayExplosionSFX()
    {
        PlayClip(explosionSFX);
    }
    public void PlayCoinBangSFX()
    {
        PlayClip(coinBangSFx);
    }
    public void PlayBounceSFx()
    {
        //audioSource.clip = firstBounceSfx[Random.Range(0,firstBounceSfx.Length)];
        //audioSource.Play();
    }
    public void PlayPoundSFx()
    {
        PlayClip(poundSFx);

    }
    public void PlayFlagCheckPointSFx()
    {
        PlayClip(flagCheckPointSFx);
    }
    public void PlayBrickBreakSFx()
    {
        PlayClip(brickBreakSFx);
    }
    public void PlayOnHitSFx()
    {
        PlayClip(playerOnHitSFx);
    }
    public void PlayLevelCompleteSFx()
    {
        PlayClip(levelCompleteSFx);
    }
    public void PlayAcidSplashSFx()
    {
        PlayClip(acidSplashSFx);
    }
    public void PlayGhostRespawnSFx(bool value)
    {
        if (value)
        {
            ghostSource?.Play();
        }
        else
        {
            ghostSource?.Stop();
        }
    }
    public void PlayGateUnlockSFx(bool value)
    {
        if (value)
        {
            gateUnlockSource.Play();
        }
        else
        {
            gateUnlockSource.Stop();
        }
    }
    #region Creatures

    public void PlayNewCreatureReveal()
    {
        PlayClip(newCreatureRevealUI);
    }
    public void PlayExistingCreatureReveal()
    {
        PlayClip(existingCreatureRevealUI);
    }
    public void PlayCreaturePickupSFx()
    {
        PlayClip(creaturePickup);
    }
    public void PlayCreaturePickCancelSFx()
    {
        PlayClip(creaturePickCancel);
    }
    public void PlayCreatureDropSFx()
    {
        PlayClip(creatureDrop);
    }
    public void PlayFloorScrollSFx()
    {
        PlayClip(floorScrollUI);
    }
    public void PlayFloorScrollSnapSFx()
    {
        PlayClip(floorScrollSnapUI);
    }
    public void PlayCagedPodSFx()
    {
        PlayClip(cagedPodBreak);
    }
    public void PlayCursedCurrencyCollectlSFx()
    {
        PlayClip(cursedCurrencyCollect);
    }
    public void PlayPipeTileBreakSFx()
    {
        Debug.Log("tile break sfx");
        PlayClip(tileBreak);
    }
    public void PlayTileRegroupSFx()
    {
        PlayClip(tileRegroup);
    }
    public void PlayFallingSFx()
    {
        PlayClip(fallingSound);
    }
    public void PlayEntryPipeSFx()
    {
        PlayClip(pipeSound);
    }
    public void PlayExitPipeSFx()
    {
        PlayClip(vaccumPullSound);
    }
  
    public void PlayCreatureCollectSFx()
    {
        PlayClip(creatureCollect);
    }
    public void PlaySlamSFx()
    {
        PlayClip(slamSFX[Random.Range(0, slamSFX.Length)]);
    }
    public void PlayCreatureCollectToLevelSFx(int index)
    {
        PlayClip(creaturesAdded[index]);
    }
    public void PlayCreatureToInventorySFx(int index)
    {
        PlayClip(creaturesInventory[index]);
    }
    public void PlayStarsCollectSFx(int index)
    {
        PlayClip(starsCollect[index]);
    }
    #endregion
}
