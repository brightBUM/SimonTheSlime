using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IkillPlayer 
{
    public void KillPlayer(System.Action<IkillPlayer> handleKill);
}
