using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureType
{
    Common,
    Rare,
    Epic
}

public class CreatureDrop : LootDrop
{
    public CreatureType creatureType;
    public void SetCreatureType(CreatureType creatureType,Sprite sprite)
    {
        this.creatureType = creatureType;
        spriteRenderer.sprite = sprite;
    }
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //attach to last chain position for player controller
        if(collision.TryGetComponent<CreatureChain>(out CreatureChain creatureChain))
        {
            creatureChain.AddToChain(creatureType,spriteRenderer.sprite);
            Destroy(this.gameObject);
        }
    }
}
