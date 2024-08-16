using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSetup : MonoBehaviour
{
    [SerializeField] DynamicWater dynamicWater;
    [SerializeField] DynamicWater2Df dynamicWater2Df;
    BoxCollider2D boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        AssignInitialBounds();
    }

    private void AssignInitialBounds()
    {
        DynamicWater.Bound bound = new DynamicWater.Bound();
        bound.left = transform.position.x-boxCollider.size.x/2;
        bound.right = transform.position.x+boxCollider.size.x/2;
        bound.bottom = transform.position.y - boxCollider.size.y / 2;
        bound.top = (transform.position.y + boxCollider.size.y / 2);

        //set dynamic water bound
        dynamicWater.SetBounds(bound);
        //dynamicWater.transform.position = boxCollider.bounds.center;

        //set dynamic water 2df bounds , have to offset it by bounds size beforehand
        Vector2 offsetPos = new Vector2(bound.left - boxCollider.size.x, bound.bottom);

        //dynamicWater.transform.position = new Vector3(offsetPos.x, offsetPos.y, 0f);
        
        dynamicWater2Df.transform.position = new Vector3(offsetPos.x, offsetPos.y, 0f);
        
        dynamicWater2Df.SetBounds(boxCollider.size);
        
        boxCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
