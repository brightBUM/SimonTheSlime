using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Action rightClicked;  
    public Action QkeyPressed;  
    public Action<Vector2> mouseClicked;
    public Action mouseReleased;
    public Action<Vector2> mouseDragging;

    Camera camRef;
    Vector2 mousePos;
    // Start is called before the first frame update
    void Start()
    {
        camRef = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            mousePos = camRef.ScreenToWorldPoint(Input.mousePosition);
            mouseDragging.Invoke(mousePos);
        }
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = camRef.ScreenToWorldPoint(Input.mousePosition);
            mouseClicked.Invoke(mousePos);
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseReleased.Invoke();
        }
        if (Input.GetMouseButtonDown(1))
        {
            rightClicked.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            QkeyPressed.Invoke();
        }
    }
}
