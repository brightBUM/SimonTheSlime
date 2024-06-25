using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Action rightClicked;  
    public Action mouseClicked;
    public Action mouseReleased;
    public Action<Vector2> mouseDragging;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Vector2 mousePos = Input.mousePosition;
            mouseDragging.Invoke(mousePos);
        }
        if (Input.GetMouseButtonDown(0))
        {
            mouseClicked.Invoke();
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseReleased.Invoke();
        }
        if (Input.GetMouseButtonDown(1))
        {
            rightClicked.Invoke();
        }
    }
}
