using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using UnityEngine;


public class GrapplePoint : MonoBehaviour
{
    [SerializeField] GameObject grappleUIKey;
    [SerializeField] Transform rangeVisual;
    [SerializeField] float dropTimerMax = 5f;
    [SerializeField] float rangeShrinkScale = 0.15f;
    [SerializeField] float rangeNormalScale = 2.2f;
    [SerializeField] GameObject playerDummy;
    [SerializeField] ParticleSystem smokeFX;
    [SerializeField] ParticleSystem electricSparkFX;
    float timer = 0;
    ParticleSystem.EmissionModule emissionModule;
    private TweenerCore<float, float, FloatOptions> Tween;
    public Action PlayerDropped; 

    // Start is called before the first frame update
    void Start()
    {
        emissionModule = smokeFX.emission;
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            grappleUIKey.SetActive(false);
            playerController.SetGrapplePoint(this.transform.position,ref this.PlayerDropped);
            playerController.GrappleRangeShrink += GrapplePointShrink;
            playerController.GrappleRelaunch += GrappleRelaunched;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            grappleUIKey.SetActive(true);
            playerController.FreeGrapplePoint(ref this.PlayerDropped);
            playerController.GrappleRangeShrink -= GrapplePointShrink;
            playerController.GrappleRelaunch -= GrappleRelaunched;
            rangeVisual.DOScale(rangeNormalScale, 0.2f).SetEase(Ease.OutBounce);
        }
    }

    private void GrapplePointShrink()
    {
        rangeVisual.DOScale(rangeShrinkScale, 0.2f).SetEase(Ease.OutBounce);
        playerDummy.SetActive(true);

        StartCoroutine(DelayedCall());
    }
    IEnumerator DelayedCall()
    {
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(DroneSmokeFx());
    }

    private IEnumerator DroneSmokeFx()
    {
        //do not start smoke effect , if player leaves drone within 2 sec
        if (!playerDummy.activeSelf)
            yield break ;


        float emissionrate = 5f;
        emissionModule.rateOverTime = emissionrate;
        float rate = 0f;
        
        while(rate <= 3.0f)
        {
            if(!playerDummy.activeSelf)
            {
                emissionModule.rateOverTime = 0f;
                yield break;
            }
            rate += Time.deltaTime;
            emissionModule.rateOverTime = Mathf.Lerp(emissionrate,20f,rate);
            yield return null;
        }
        
        emissionModule.rateOverTime = 0f;
        electricSparkFX.Play();
        PlayerDropped?.Invoke();
    }

    private void GrappleRelaunched()
    {
        playerDummy.SetActive(false);
        
    }

}
