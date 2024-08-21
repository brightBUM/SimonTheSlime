using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDummy : MonoBehaviour
{
    Rigidbody2D rb;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        rb.velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 7)
        {
            rb.velocity = Vector3.zero;
        }
    }
}
