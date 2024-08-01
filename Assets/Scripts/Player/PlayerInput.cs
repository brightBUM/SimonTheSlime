using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Action PoundAbility;  
    //public Action BulletTimeAbility;  
    public Action DashAbility;  
    public Action GrappleAbility;  
    public Action mouseReleased;
    public Action RespawnToCheckPoint;
    //public Action<Vector2> mouseClicked;
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
        //if (Input.GetMouseButtonDown(0))
        //{
        //    mousePos = camRef.ScreenToWorldPoint(Input.mousePosition);
        //    mouseClicked.Invoke(mousePos);
        //}
        if (Input.GetMouseButtonUp(0))
        {
            mouseReleased.Invoke();
        }
        if (Input.GetMouseButtonDown(1))
        {
            PoundAbility.Invoke();
        }
        //if(Input.GetKeyDown(KeyCode.Z))
        //{
        //    BulletTimeAbility.Invoke();
        //}
        if (Input.GetKeyDown(KeyCode.X))
        {
            DashAbility.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GrappleAbility.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            RespawnToCheckPoint.Invoke();
        }
    }
}
