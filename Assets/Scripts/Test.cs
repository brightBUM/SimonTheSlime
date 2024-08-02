using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    // Start is called before the first frame update


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ObjectPoolManager.Instance.Spawn(0, Vector3.zero, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ObjectPoolManager.Instance.Despawn(this.gameObject,0f);
        }
    }

}
