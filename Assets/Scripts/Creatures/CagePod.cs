using DG.Tweening;
using UnityEngine;

public class CagePod : MonoBehaviour
{
    [SerializeField] CreatureType creatureType;
    [SerializeField] GameObject cagedVisual;
    [SerializeField] GameObject brokenPod;
    [SerializeField] GameObject breakVFX;
    [SerializeField] CreatureDrop creaturePrefab;
    [SerializeField] SpriteRenderer creatureVisual;
    [SerializeField] SpriteRenderer glassVisual;
    [SerializeField] float bobOffset;

    Tween bobTween;
    
    public void Init(CreatureType creatureType)
    {
        //get random creature assignment
        this.creatureType = creatureType;

        //for now manual assignment
        creatureVisual.sprite = GameManger.Instance.GetCreatureSprite(creatureType);
        glassVisual.color = GameManger.Instance.GetCreatureColor(creatureType);
        //bobbing Visual
        bobTween = creatureVisual.transform.DOMoveY(creatureVisual.transform.position.y + bobOffset, 1f)
            .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutSine);
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

    private void OnDestroy()
    {
        bobTween.Kill();

    }
}
