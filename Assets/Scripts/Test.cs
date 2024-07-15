using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Test : MonoBehaviour
{
    bool leftHit; 
    bool rightHit;
    [SerializeField] LayerMask platformMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("collided with : " + collision.gameObject.name);
        var leftHitInfo = Physics2D.Raycast(transform.position, -transform.up, 2f, platformMask);
        if (leftHitInfo.collider != null)
        {
            Debug.Log("up hit");
            leftHit = true;
        }
        else
        {
            leftHit = false;
        }
        var rightHitInfo = Physics2D.Raycast(transform.position, transform.right, 2f, platformMask);
        if (rightHitInfo.collider != null)
        {
            Debug.Log("right hit");

        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = leftHit?Color.green:Color.red;
        Gizmos.DrawRay(transform.position, -2f * transform.up);
        //Gizmos.DrawRay(transform.position, 5f * transform.right);
    }

}
