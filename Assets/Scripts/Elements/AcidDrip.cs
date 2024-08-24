using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidDrip : MonoBehaviour
{
    [SerializeField] float minTime = 3f;
    [SerializeField] float maxTime = 6f;
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
            var randNum = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(randNum);

            var acidDrop = ObjectPoolManager.Instance.Spawn(3,spawnPoint.position,Quaternion.identity);
            ObjectPoolManager.Instance.Despawn(acidDrop, 2f);
        }
       
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
