using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class GrapplePoint : MonoBehaviour
{
    [SerializeField] GameObject grappleUIKey;
    [SerializeField] Transform rangeVisual;
    [SerializeField] float rangeShrinkScale = 0.15f;
    [SerializeField] float rangeNormalScale = 2.2f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            grappleUIKey.SetActive(true);
            playerController.SetGrapplePoint(this.transform.position);
            playerController.GrappleRangeShrink += GrapplePointShrink;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            grappleUIKey.SetActive(false);
            playerController.FreeGrapplePoint();
            playerController.GrappleRangeShrink -= GrapplePointShrink;
            rangeVisual.DOScale(rangeNormalScale, 0.2f).SetEase(Ease.OutBounce);
        }
    }

    private void GrapplePointShrink()
    {
        rangeVisual.DOScale(rangeShrinkScale, 0.2f).SetEase(Ease.OutBounce);
    }
    
    
}
