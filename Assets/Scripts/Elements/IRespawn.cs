using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRespawn : MonoBehaviour
{
    [Header("BaseRespawn")]
    [SerializeField] BoxCollider2D groundCollider;
    [SerializeField] GameObject respawnEffect;

    public void ToggleRespawnEffect(bool value)
    {
        respawnEffect.SetActive(value);
    }

    public void RespawnEffect()
    {
        StartCoroutine(DelayedRespawn());
    }

    IEnumerator DelayedRespawn()
    {
        ToggleRespawnEffect(true);
        groundCollider.enabled = true;

        yield return new WaitForSeconds(1.5f);

        ToggleRespawnEffect(false);
    }

    public Vector3 CheckPointPosition()
    {
        return transform.position;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerController>())
        {
            groundCollider.enabled = false;
        }
    }
}
