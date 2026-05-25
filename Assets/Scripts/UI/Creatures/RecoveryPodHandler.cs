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
                recoveryPods[i].Init(i,recoveryPodData[i]);
            }
            else
            {
                //all other in buy state
                recoveryPods[i].Init(i);
            }
        }
    }
    
}
