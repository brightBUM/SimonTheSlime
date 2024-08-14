using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoundable 
{
    public void OnPlayerPounded(System.Action<IPoundable> ContinuePound);
}
