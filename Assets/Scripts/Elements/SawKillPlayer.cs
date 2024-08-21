using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawKillPlayer : MonoBehaviour,IkillPlayer
{
    
    // Start is called before the first frame update
    void Start()
    {

    }
    public void KillPlayer(System.Action<IkillPlayer> handleKill)
    {
        
        

        handleKill(this);
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
