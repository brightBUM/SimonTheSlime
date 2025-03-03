using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRespawn : MonoBehaviour
{
    [Header("BaseRespawn")]
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

        yield return new WaitForSeconds(1.5f);

        ToggleRespawnEffect(false);
    }

    public Vector3 CheckPointPosition()
    {
        return transform.position;
    }
}
