using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour,IkillPlayer
{
    public void KillPlayer(System.Action<IkillPlayer> HandleKill)
    {
        HandleKill(this);
    }

}
