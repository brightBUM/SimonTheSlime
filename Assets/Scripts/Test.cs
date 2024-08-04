using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform start; 
    [SerializeField] Transform drag;
    [SerializeField] Transform result;
    [SerializeField] TextMeshProUGUI dotValText;
    [SerializeField] float maxForce = 5f;
    bool firstclick;
    // Start is called before the first frame update
    LineRenderer lineRenderer;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if(!firstclick)
            {
                start.position = mousePos;
                lineRenderer.SetPosition(0,transform.position);
                firstclick = true;
                return;
            }

            drag.position = mousePos;
            var diff = drag.position - start.position;
            var result = transform.position+(-diff);

            var aimDir = result - transform.position;
            result = transform.position + Vector3.ClampMagnitude(aimDir, maxForce);

           
            //dotValText.text = Vector2.Dot(transform.position.normalized, result.position.normalized).ToString();
            var dotvalue = Vector2.Dot(((-transform.right)-transform.position).normalized, (result-transform.position).normalized);
            dotValText.text = dotvalue.ToString();
            if(dotvalue>0.1f)
            {
                this.result.position = result;
                lineRenderer.SetPosition(1, result);

            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            firstclick = false;
        }
       
       
    }

}
