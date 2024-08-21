using System.Collections;
using UnityEngine;

public class ExplosionKillPlayer : MonoBehaviour,IkillPlayer
{
    public void KillPlayer(System.Action<IkillPlayer> handleKill)
    {
        handleKill(this);
    }

    
}