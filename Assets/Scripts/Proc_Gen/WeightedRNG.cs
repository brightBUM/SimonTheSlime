using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeightedRNG : MonoBehaviour
{
    // Weights for each rarity
    [SerializeField] private int commonWeight = 70;
    [SerializeField] private int rareWeight = 25;
    [SerializeField] private int epicWeight = 5;
    [SerializeField] CagePod cagePodPrefab;

    List<CagePod> spawnedItems;
    Vector3 pos;
    
    // Start is called before the first frame update
    void Start()
    {
        spawnedItems = new List<CagePod>();
        pos = new Vector3(-25f, 0f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //clear old 
            ClearItems();
            var spawnSize = Random.Range(2, 6);
            for (int i = 0; i < spawnSize; i++)
            {
                //spawn weighted items;
                GetRandomRarity();
                pos += Vector3.right * 10f;
            }
        }
    }

    public void ClearItems()
    {
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            Destroy(spawnedItems[i].gameObject);
        }
        spawnedItems.Clear();
        pos = new Vector3(-25f, 0f, 0);
    }

    public void SpawnPods(List<Transform> cagePodTransforms,int chunkSize)
    {
        int podSpawnCount = 0;
        switch(chunkSize)
        {
            case 5:
                podSpawnCount = 2; break;
            case 6:
            case 7:
                podSpawnCount = 3; break;
            case 8:
                podSpawnCount = 5; break;
            default:
                podSpawnCount = 2; break;
                
        }

        for (int i = 0; i < podSpawnCount; i++)
        {
            //spawn weighted items;
            pos = Utility.RandomUniqueItemFromList(cagePodTransforms).position;
            GetRandomRarity();
        }
    }
    public void GetRandomRarity()
    {
        int totalWeight = commonWeight + rareWeight + epicWeight;
        int roll = Random.Range(1, totalWeight + 1); // inclusive

        if (roll <= commonWeight)
            Common();
        else if (roll <= commonWeight + rareWeight)
            Rare();
        else
            Epic();
    }
    private void SpawnPod(CreatureType creatureType,string itemName)
    {
        var item = Instantiate(cagePodPrefab, pos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(item.gameObject, gameObject.scene);
        item.Init(creatureType);
        item.name += itemName;
        spawnedItems.Add(item);
    }
    private void Common()
    {
        SpawnPod(CreatureType.Common, " Common");
    }
    private void Rare()
    {
        SpawnPod(CreatureType.Rare, " Rare");
    }
    private void Epic()
    {
        SpawnPod(CreatureType.Epic, " Epic");

    }
    //// Example test

}
