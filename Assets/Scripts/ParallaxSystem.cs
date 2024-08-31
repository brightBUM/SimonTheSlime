using System;
using Unity.Mathematics;
using UnityEngine;

public class ParallaxSystem : MonoBehaviour
{

    private float length;
    public GameObject cam;
    public float xFactor;
    public float yFactor;
    private Vector2 startPos;
    private float lastYpos;

    void Start()
    {
        startPos = (Vector2)transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        lastYpos = cam.transform.position.y;

        
    }
    private void FixedUpdate()
    {
        float temp = (cam.transform.position.x * (1 - xFactor));
        float dist = (cam.transform.position.x * xFactor);

        float yShift = cam.transform.position.y - lastYpos;
        lastYpos = cam.transform.position.y;

        transform.position = new Vector3(startPos.x + dist, transform.position.y+(yShift*yFactor), transform.position.z);

        if (temp > startPos.x + length) startPos.x += length;
        else if (temp < startPos.x - length) startPos.x -= length;


    }
    void LateUpdate()
    {

    }
}
