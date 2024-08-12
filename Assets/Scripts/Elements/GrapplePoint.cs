using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Splines;
using static UnityEngine.Rendering.DebugUI;

public class GrapplePoint : MonoBehaviour
{
    [SerializeField] GameObject grappleUIKey;
    [SerializeField] Transform rangeVisual;
    [SerializeField] float dropTimerMax = 5f;
    [SerializeField] float rangeShrinkScale = 0.15f;
    [SerializeField] float rangeNormalScale = 2.2f;
    [SerializeField] GameObject playerDummy;
    [SerializeField] ParticleSystem smokeFX;
    float timer = 0;
    ParticleSystem.EmissionModule emissionModule;
    private TweenerCore<float, float, FloatOptions> Tween;

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
            playerController.SetGrapplePoint(this.transform.position);
            playerController.GrappleRangeShrink += GrapplePointShrink;
            playerController.GrappleRelaunch += GrappleRelaunched;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            grappleUIKey.SetActive(true);
            playerController.FreeGrapplePoint();
            playerController.GrappleRangeShrink -= GrapplePointShrink;
            playerController.GrappleRelaunch -= GrappleRelaunched;
            rangeVisual.DOScale(rangeNormalScale, 0.2f).SetEase(Ease.OutBounce);
        }
    }

    private void GrapplePointShrink()
    {
        rangeVisual.DOScale(rangeShrinkScale, 0.2f).SetEase(Ease.OutBounce);
        playerDummy.SetActive(true);

        DOVirtual.DelayedCall(2f, () =>
        {
            if (!playerDummy.activeSelf)
                return;
            float emissionrate = 5f;
            emissionModule.rateOverTime = emissionrate;
            Tween =  DOTween.To(() => emissionrate, x => emissionrate = x, 20f, 3f).OnUpdate(() =>
            {
                if (!playerDummy.activeSelf)
                {
                    DOTween.Kill(Tween);
                    emissionrate = 0f;
                }
                emissionModule.rateOverTime = emissionrate;
                
            });
        });
        
        
    }
    private void GrappleRelaunched()
    {
        playerDummy.SetActive(false);
        emissionModule = smokeFX.emission;
        emissionModule.rateOverTime = 0f;
    }

}
