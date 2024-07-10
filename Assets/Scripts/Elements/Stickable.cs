using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stickable : MonoBehaviour
{
    [SerializeField] float yValue = 0f;
    [SerializeField] float delayTime = 1f;
    [SerializeField] Transform fallOffPoint;
    PlayerController playerController;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController != null)
        {
            pos = playerController.transform.position;
            var distance = Vector2.Distance(pos, fallOffPoint.position);
            
            if (distance>1.5f)
            {
                //Debug.Log("distance : " + distance);
                pos.y += yValue * Time.deltaTime;
                playerController.transform.position = pos;

            }
            else
            {
                Debug.Log("player made null,reached distance : " + distance);
                //change player state from stick to idle
                playerController.SetToIdle();
                playerController.ResetGravity();
                playerController = null;
            }
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            //if its a stickable platform make it slide after delay 
            //if its conveyor stick and move players position with you
            //Debug.Log("enter conveyor");
            playerController.SetToStickState();
            this.playerController = playerController;
        }
    }
    
}
