using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidDrip : MonoBehaviour
{
    [SerializeField] float interval;
    [SerializeField] Transform spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AcidDripSpawner());
    }
    IEnumerator AcidDripSpawner()
    {
        while (true)
        {
            var acidDrop = ObjectPoolManager.Instance.Spawn(3,spawnPoint.position,Quaternion.identity);
            ObjectPoolManager.Instance.Despawn(acidDrop, 2f);
            yield return new WaitForSeconds(interval);
        }
       
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
