using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] PlayerInput input;
    // Start is called before the first frame update
    private void OnEnable()
    {
        //input.mouseClicked += LeftClicked;
        input.mouseReleased += LeftReleased;
        input.mouseDragging += LeftDragging;
    }

    private void LeftDragging(Vector2 vector)
    {
        Debug.Log("dragging");

    }

    private void LeftReleased()
    {
        Debug.Log("end");

    }

    private void LeftClicked(Vector2 vector)
    {
        Debug.Log("click");

    }
    private void OnDisable()
    {
        //input.mouseClicked  -= LeftClicked;
        input.mouseReleased -= LeftReleased;
        input.mouseDragging -= LeftDragging;
    }

}
