using SpriteTrailRenderer;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject ghostParticleVFX;
    [SerializeField] GameObject deathFXPrefab;
    [SerializeField] Material lineMaterial;
    [SerializeField] Transform arrowHead;
    [SerializeField] Volume volume;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] Material hitMaterial;
    [SerializeField] Gradient normalTrail;
    [SerializeField] Gradient poundTrail;
    [SerializeField] float trajectorySpeed = 5f;
    [SerializeField] float hitResetTime = 1.5f;
    [SerializeField] float blendValue = 6f;
    [SerializeField] float newBlendValue = 12f;
    [SerializeField] float poundTrailTime = 0.15f;
    LineRenderer lineRenderer;
    Animator animator;
    SpriteRenderer spriteRenderer;
    SpriteTrailRenderer.SpriteTrailRenderer spriteTrailRenderer;
    [SerializeField] Material originalMaterial;
    float timer = 1f;
    
    public SpriteRenderer ghostDummyVisual;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        spriteTrailRenderer = GetComponent<SpriteTrailRenderer.SpriteTrailRenderer>();

        var color = GameManger.Instance.GetCharSkinColor();
        if (color == Color.black)
        {
            spriteRenderer.material = new Material(Shader.Find("Sprites/Default")); ;
        }
        else
        {
            spriteRenderer.material.SetColor("_replaceColor", GameManger.Instance.GetCharSkinColor());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.playerState == State.LAUNCHED)
        {
            animator.SetFloat("velocity", playerController.Velocity);
            //normalTrail.keys
        }
    }
    public void FlipSprite(Vector2 aimDirection)
    {
        spriteRenderer.flipX = Vector2.Dot(Vector2.right, aimDirection) < 0 ? true : false;
        spriteRenderer.material.SetFloat("_FlipX", spriteRenderer.flipX ? 1.0f : 0.0f);
    }

    public void DrawTrajectory(Vector2 vel,bool aiminLimit)
    {
        //draw line renderer points
        for(int i = 0;i<lineRenderer.positionCount;i++)
        {
            var pos = playerController.GetPosition(vel, i / (float)lineRenderer.positionCount);
            lineRenderer.SetPosition(i, pos);

            arrowHead.position = lineRenderer.GetPosition(lineRenderer.positionCount-1);
            var dir = lineRenderer.GetPosition(lineRenderer.positionCount - 1) - lineRenderer.GetPosition(lineRenderer.positionCount - 2);
            var rot = Mathf.Atan2(dir.y , dir.x) * Mathf.Rad2Deg;
            arrowHead.rotation = Quaternion.Euler(0f,0f,rot);

            //for(int _i=0, j=0;_i<10;_i++,j+=5 )
            //{
            //    //
            //    //draw white
            //    //racyast from getpos(j) to getpos (j+5).
            //    //if you get wall. // draw remaing index red
            //    // else draw breakl.

            //}
        }

        //animate line renderer material
        timer -= Time.deltaTime;
        var result = Mathf.Lerp(0, 1, timer);
        if(result <= 0)
            timer = 1;
        lineMaterial.mainTextureOffset = Vector2.right * result*trajectorySpeed;

        lineMaterial.color = !aiminLimit? new Color(0.9f, 0.3f, 0.23f) : Color.white;
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
    public bool GetSpriteFlipX()
    {
        return spriteRenderer.flipX;
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
    public void SetLaunch()
    {
        animator.SetTrigger("launch");
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
        arrowHead.gameObject.SetActive(value);
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
    public void ToggleGhostDummy(bool value)
    {
        ghostDummyVisual.enabled = value;  
    }
    public void PoundTrailEffect()
    {
        trailRenderer.time = poundTrailTime;
        trailRenderer.startWidth = 2.5f;
        trailRenderer.colorGradient = poundTrail;
    }
    public void ResetTrailEffect()
    {
        trailRenderer.time = 0.3f;
        trailRenderer.startWidth = 1.5f;
        trailRenderer.colorGradient = normalTrail;
    }
    public void SpawnJumpTrail()
    {
        ObjectPoolManager.Instance.Spawn(3, transform.position + Vector3.down * 0.5f, Quaternion.identity);
    }
    public void DisableGhostParticle()
    {
        //volume.blendDistance = blendValue;
        ghostParticleVFX.SetActive(false);
    }
    public void DeathEffect()
    {
        ToggleTrailRenderer(false);
        ToggleSpriteRenderer(false);
        var deathFX = Instantiate(deathFXPrefab,transform.position,Quaternion.identity);
        Destroy(deathFX, 1.5f);
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
        ToggleSpriteRenderer(false);
        animator.SetTrigger("ghost");
        ToggleGhostDummy(true);
        //volume.blendDistance = newBlendValue;

        //ghost particle orientation
        ghostParticleVFX.SetActive(true);
        Transform trans = ghostParticleVFX.transform;
        if (spriteRenderer.flipX)
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


