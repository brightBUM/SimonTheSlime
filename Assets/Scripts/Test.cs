using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] ParticleSystem gorePrefab;
    

    private void Start()
    {
        
    }
    private void Update()
    {
        
        if(Input.GetKeyUp(KeyCode.G))
        {
            gorePrefab.Play();
        }

    }

    
}
