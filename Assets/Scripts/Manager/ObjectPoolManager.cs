using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [SerializeField] List<GameObject> objectsToPool;
    [SerializeField] Dictionary<string,List<GameObject>> mainPool = new Dictionary<string,List<GameObject>>();
    private List<Transform> poolParents;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        poolParents = new List<Transform>();
        foreach (var item in objectsToPool)
        {
            mainPool.Add(item.name + "_Pool", new List<GameObject>());
            var poolParent = new GameObject();
            poolParent.name = item.name + "_Pool";
            poolParent.transform.parent = this.transform;
            poolParents.Add(poolParent.transform);
        }
    }

    public GameObject Spawn(int index,Vector3 position,Quaternion rotation)
    {
        //find inactive item in pool;
        foreach(var poolObject in mainPool[objectsToPool[index].name+"_Pool"])
        {
            if(!poolObject.activeInHierarchy)
            {
                poolObject.SetActive(true);
                poolObject.transform.localPosition = position;
                poolObject.transform.localRotation = rotation;
                return poolObject;
            }
        }

        var poolItem = Instantiate(objectsToPool[index], position, rotation, poolParents[index]);
        mainPool[objectsToPool[index].name + "_Pool"].Add(poolItem);
        return poolItem;
    }
    public void Despawn(GameObject gameObject,float delay)
    {
        if(gameObject!=null)
        {
            if(delay > 0)
            {
                DOVirtual.DelayedCall(delay, () =>
                {
                    gameObject.SetActive(false);
                });
            }
            else
            {
                gameObject.SetActive(false);
            }
           
        }
    }

    public void ResetPool(int index)
    {
        foreach (var poolObject in mainPool[objectsToPool[index].name + "_Pool"])
        {
            if (poolObject.activeInHierarchy)
            {
                poolObject.SetActive(false);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
