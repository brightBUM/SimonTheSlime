using SpriteTrailRenderer;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject ghostParticleVFX;
    [SerializeField] Material lineMaterial;
    [SerializeField] Volume volume;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] Material hitMaterial;
    [SerializeField] Gradient normalTrail;
    [SerializeField] Gradient poundTrail;
    [SerializeField] float trajectorySpeed = 5f;
    [SerializeField] float hitResetTime = 1.5f;
    [SerializeField] float blendValue = 6f;
    [SerializeField] float newBlendValue = 12f;
    LineRenderer lineRenderer;
    Animator animator;
    SpriteRenderer spriteRenderer;
    SpriteTrailRenderer.SpriteTrailRenderer spriteTrailRenderer;
    [SerializeField] Material originalMaterial;
    
    float timer = 1f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        spriteTrailRenderer = GetComponent<SpriteTrailRenderer.SpriteTrailRenderer>();
        //originalMaterial = spriteRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.playerState == State.LAUNCHED)
        {
            animator.SetFloat("velocity", playerController.Velocity);
        }
    }
    public void FlipSprite(Vector2 aimDirection)
    {
        spriteRenderer.flipX = Vector2.Dot(Vector2.right, aimDirection) < 0 ? true : false;
    }

    public void DrawTrajectory(Vector2 vel)
    {
        //draw line renderer points
        for(int i = 0;i<lineRenderer.positionCount;i++)
        {
            var pos = playerController.GetPosition(vel, i / (float)lineRenderer.positionCount);
            lineRenderer.SetPosition(i, pos);
        }

        //animate line renderer material
        timer -= Time.deltaTime;
        var result = Mathf.Lerp(0, 1, timer);
        if(result <= 0)
            timer = 1;
        lineMaterial.mainTextureOffset = Vector2.right * result*trajectorySpeed;
    }
    public void SetAim()
    {
        //animator.ResetTrigger("idle");
        animator.SetTrigger("aim");
    }
    public void SetRoll()
    {
        animator.SetTrigger("roll");
    }
    public void SetIdle()
    {
        //animator.ResetTrigger("roll");
        animator.SetTrigger("idle");
    }
    public void ResetAll()
    {
        ResetVelocity();
        animator.ResetTrigger("stick");
        animator.ResetTrigger("roll");
        animator.ResetTrigger("aim");
        animator.ResetTrigger("idle");
        animator.ResetTrigger("relaunch");
    }
    public void SetRelaunch()
    {
        animator.SetTrigger("relaunch");
    }
    public void SetStick(float sideValue)
    {
        animator.SetTrigger("stick");
        //Debug.Log("set to stick anim state : "+sideValue);
        animator.SetFloat("side",sideValue);
    }
    public void SetSquish()
    {
        animator.SetTrigger("squish");
    }
    public void ResetVelocity()
    {
        animator.SetFloat("velocity", 0f);
    }
    public void SetGrapplePose()
    {
        animator.SetTrigger("grapplePose");
    }
    public void SetGrappleGrab()
    {
        animator.SetTrigger("grappleGrab");
    }
    public void ToggleLineRenderer(bool value)
    {
        //for(int i = 0; i<lineRenderer.positionCount;i++)
        //{
        //    lineRenderer.SetPosition(i, Vector3.zero);
        //}
        lineRenderer.enabled = value;
    }
    public void ToggleTrailRenderer(bool value)
    {
        trailRenderer.enabled = value;
    }
    public void ToggleSpriteRenderer(bool value)
    {
        spriteRenderer.enabled = value;
    }
    public void ToggleSpriteTrailRenderer(bool value)
    {
        spriteTrailRenderer.enabled = value;
    }
    public void PoundTrailEffect()
    {
        trailRenderer.startWidth = 2.5f;
        trailRenderer.colorGradient = poundTrail;
    }
    public void ResetTrailEffect()
    {
        trailRenderer.startWidth = 1.5f;
        trailRenderer.colorGradient = normalTrail;
    }
    public void SpawnJumpTrail()
    {
        ObjectPoolManager.Instance.Spawn(3, transform.position + Vector3.down * 0.5f, Quaternion.identity);
    }
    public void DisableGhostParticle()
    {
        volume.blendDistance = blendValue;
        ghostParticleVFX.SetActive(false);
    }
    public void HitEffect(Action respawnPlayer)
    {
        ToggleTrailRenderer(false);
        //spriteRenderer.material = hitMaterial;
        StartCoroutine(GhostEffect(respawnPlayer));
    }
    IEnumerator GhostEffect(Action respawnPlayer)
    {
        //change to original material after some delay
        yield return new WaitForSeconds(hitResetTime);
        //spriteRenderer.material = originalMaterial;
        ToggleSpriteRenderer(true);
        animator.SetTrigger("ghost");
        volume.blendDistance = newBlendValue;

        //ghost particle orientation
        ghostParticleVFX.SetActive(true);
        Transform trans = ghostParticleVFX.transform;
        if (!spriteRenderer.flipX)
        {
            trans.localPosition = new Vector3(10, trans.localPosition.y, trans.localPosition.z);
            trans.localScale = Vector3.one;
        }
        else
        {
            trans.localPosition = new Vector3(-10, trans.localPosition.y, trans.localPosition.z);
            trans.localScale = new Vector3(-1, 1, 1);
        }

        //then respawn to last checkpoint;
        respawnPlayer();
    }
}


