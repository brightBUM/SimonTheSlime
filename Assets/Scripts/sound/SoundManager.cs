using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip noBulletTImeSfx;
    [SerializeField] private AudioClip explosionSFX;
    [SerializeField] private AudioClip poundSFx;
    [SerializeField] private AudioClip ghostRespawnSFx;
    [SerializeField] private AudioClip sloMoTimerSFx;
    [SerializeField] private AudioClip flagCheckPointSFx;
    [SerializeField] private AudioClip acidSplashSFx;
    [SerializeField] private AudioClip brickBreakSFx;
    [SerializeField] private AudioClip playerOnHitSFx;
    [SerializeField] private AudioClip levelCompleteSFx;
    [SerializeField] private AudioClip grappleRopeSFx;
    [SerializeField] private AudioClip grapplePullSFx;
    [SerializeField] private AudioClip coinBangSFx;
    [SerializeField] private AudioClip timeOrbCollectSFx;
    [SerializeField] AudioSource ghostSource;
    [SerializeField] private AudioClip[] slimeStickSFX;
    [SerializeField] private AudioClip[] aimStretchSfx;
    [SerializeField] private AudioClip[] firstBounceSfx;
    [SerializeField] private AudioClip[] coinCollectSfx;
    private AudioSource activeSource;

    public static SoundManager instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        ghostSource.clip = ghostRespawnSFx;
    }
    
    private void PlayClip(AudioClip clip)
    {
        activeSource = GetIdleSource();
        activeSource.clip = clip;
        activeSource.Play();
        ObjectPoolManager.Instance.Despawn(activeSource.gameObject, activeSource.clip.length);
    }
    private AudioSource GetIdleSource()
    {
        //returns the audioSource that isnt playing
        var audioGameObject = ObjectPoolManager.Instance.Spawn(1, Vector3.zero, Quaternion.identity);
        return audioGameObject.GetComponent<AudioSource>();
    }
    public void PlayAimSFx()
    {
        PlayClip(aimStretchSfx[Random.Range(0, aimStretchSfx.Length)]);
    }
    public void PlayStickSFx()
    {
        PlayClip(slimeStickSFX[Random.Range(0, slimeStickSFX.Length)]);
    }
    public void PlayGrappleRopeSFX()
    {
        PlayClip(grappleRopeSFx);
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
            ghostSource.Play();
        }
        else
        {
            ghostSource.Stop();
        }
    }
    

}
