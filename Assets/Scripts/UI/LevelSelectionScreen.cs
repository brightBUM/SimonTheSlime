using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionScreen : MonoBehaviour
{
    [SerializeField] List<Transform> starparent;
    [SerializeField] Transform starPrefab;
    // Start is called before the first frame update
    void Start()
    {
        //get level progress data from saveload and show level progress
        for(int i=0;i<starparent.Count;i++)
        {
            var count = SaveLoadManager.Instance.GetLevelStarData(i);
            while(count > 0)
            {
                Instantiate(starPrefab, starparent[i]);
                count--;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
