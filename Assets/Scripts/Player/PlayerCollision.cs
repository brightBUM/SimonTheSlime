using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerCollision : MonoBehaviour
{
    PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerController.playerState == State.LAUNCHED)
        {
            //checking for firstbounce
           playerController.SetToFirstBounce();
        }
        else if(playerController.playerState == State.FIRSTBOUNCE)
        {
            //checking after firstbounce , bring it to rest
           playerController.SetToIdle();

        }
        else if(playerController.playerState == State.POUND)
        {
            playerController.ResetPound();

        }
    }
}
