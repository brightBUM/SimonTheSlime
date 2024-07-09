using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stickable : MonoBehaviour
{
    [SerializeField] float yValue = 0f;
    [SerializeField] float delayTime = 1f;
    Transform playerTransform;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTransform!=null)
        {
            pos = playerTransform.position;
            pos.y += yValue * Time.deltaTime;
            //Debug.Log("pos y : "+pos.y);

            playerTransform.position = pos;
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
            playerTransform = playerController.transform;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            //playerTransform = null;
            Debug.Log("leave conveyor");

        }
    }

    
}
