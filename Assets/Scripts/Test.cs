using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] Transform trans;
    [SerializeField] float size = 3f;
    [SerializeField] LayerMask playerLayer;
    bool hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hitinfo = Physics2D.BoxCast(trans.position, Vector2.one * size, 0, Vector2.down, 1f, playerLayer);
        if (hitinfo.collider != null)
        {
            hit = true;
            Debug.Log("hit info name : " + hitinfo.collider.name);
        }
        else { hit = false; }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = hit?Color.green:Color.red;
        Gizmos.DrawCube(trans.position, Vector3.one*size);
    }
}
