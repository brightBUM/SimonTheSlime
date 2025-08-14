using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CagePod : MonoBehaviour
{
    [SerializeField] CreatureType creatureType;
    [SerializeField] GameObject cagedVisual;
    [SerializeField] GameObject brokenPod;
    [SerializeField] GameObject breakVFX;
    [SerializeField] CreatureDrop creaturePrefab;
    [SerializeField] SpriteRenderer creatureVisual;
    [SerializeField] float bobOffset;

    Tween bobTween;
    // Start is called before the first frame update
    void Start()
    {
        //get random creature assignment

        //for now manual assignment
        creatureVisual.sprite = GameManger.Instance.GetCreatureSprite(creatureType);

        //bobbing Visual
        bobTween = creatureVisual.transform.DOMoveY(creatureVisual.transform.position.y + bobOffset, 1f)
            .SetLoops(-1,LoopType.Yoyo).SetEase(Ease.OutSine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            //check if player state is slam/dash
            if (playerController.playerState == State.POUND || playerController.playerState == State.DASH)
            {
                //break the pod 
                cagedVisual.SetActive(false);
                brokenPod.SetActive(true);
                //breakVFX.SetActive(true);
                bobTween.Kill();

                //spawn the creature prefab
                var creatureDrop = Instantiate(creaturePrefab,transform.position,Quaternion.identity);
                creatureDrop.SetCreatureType(creatureVisual.sprite);
                creatureDrop.Init();

                //disable the collider 
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }
            
    }
}
