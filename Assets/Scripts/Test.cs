using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject gorePrefab;

    ParticleSystem FXprefab;
    private void Start()
    {
        FXprefab = Instantiate(gorePrefab).GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        
        if(Input.GetMouseButtonDown(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            FXprefab.transform.position = new Vector3(pos.x, pos.y, 0);
            FXprefab.Play();
        }

    }

    
}
