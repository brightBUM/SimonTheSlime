using System.Collections.Generic;
using UnityEngine;

//class that populates pods 

public class RecoveryPodHandler : MonoBehaviour
{
    [SerializeField]List<RecoveryPod> recoveryPods;
    private void Awake()
    {
        //get data from saveload
        var recoveryPodData = SaveLoadManager.Instance.playerProfile.recoveryPodData;
        for (int i = 0; i < recoveryPods.Count; i++)
        {
            if (i < recoveryPodData.Count)
            {
                //pod bought , get pod state
                recoveryPods[i].Init(recoveryPodData[i].GetPodState());
            }
            else
            {
                //all other in buy state
                recoveryPods[i].Init(PodState.Buy);
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
