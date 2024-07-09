using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            //change state to stick 
            //if its a stickable platform make it slide after delay 
            //if its conveyor stick and move players position with you
            Debug.Log("hit with conveyor");
        }
    }
}
