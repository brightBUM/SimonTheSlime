using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDrop : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Transform radialVisual;
    [SerializeField] float upForce = 5f;
    public static Action<Sprite, Vector3> OnCollection;

    Ease easetype = Ease.InOutFlash;
    Rigidbody2D rb;
    Vector2[] dir = {new Vector2(1,1),
                     new Vector2(-1,1)};
    public void Init()
    {
        radialVisual.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);

        var randDir = dir[UnityEngine.Random.Range(0, dir.Length)];

        transform.DOJump(new Vector3(transform.position.x + randDir.x*upForce, 
                                     transform.position.y, 0), upForce, 1, 0.5f)
                                     .SetEase(easetype)
                                     .OnComplete(() =>
                                     {
                                         GetComponent<BoxCollider2D>().enabled = true;
                                     });
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Init();
        //}

        var zRotation = radialVisual.eulerAngles.z + 10f * Time.deltaTime;
        radialVisual.rotation = Quaternion.Euler(0, 0, zRotation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var sprite = spriteRenderer.sprite;

            if(transform.CompareTag("Screw Part"))
            {
                LevelManager.Instance.collectedScrews++;
            }
            else if(transform.CompareTag("Battery Part"))
            {
                LevelManager.Instance.collectedBatteries++;
            }
            else if(transform.CompareTag("Melon"))
            {
                LevelManager.Instance.collectedMelons++;
            }
            //tween the UI icon
            OnCollection.Invoke(sprite, transform.position);
            //disable self
            gameObject.SetActive(false);
        }
    }
}
