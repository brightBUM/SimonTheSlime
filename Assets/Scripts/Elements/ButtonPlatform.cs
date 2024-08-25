using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPlatform : MonoBehaviour,IPoundable 
{
    [SerializeField] LevelEndGate levelEndGate;
    int buttonValue = 0;
    public void OnPlayerPounded(Action<IPoundable> ContinuePound)
    {
        ContinuePound(this);


    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
