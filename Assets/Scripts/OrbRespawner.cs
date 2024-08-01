using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class OrbRespawner : MonoBehaviour
{
    [SerializeField] GameObject timeOrbPrefab;
    [SerializeField] Transform spawnerParent;
    [SerializeField] float timebwSpawns;
    bool readyToSpawn = true;
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (spawnerParent.childCount == 0 && readyToSpawn)
        {
            readyToSpawn = false;
            DOVirtual.DelayedCall(timebwSpawns, () =>
            {
                var orb =Instantiate(timeOrbPrefab, spawnerParent);
                orb.transform.localScale = Vector3.zero;
                orb.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBounce);
                readyToSpawn = true;
            });
        }
    }
}
