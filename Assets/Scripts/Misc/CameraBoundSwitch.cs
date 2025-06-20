using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundSwitch : MonoBehaviour
{
    [SerializeField] CinemachineConfiner2D confiner2D;
    [SerializeField] CompositeCollider2D compositeCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerController>())
        {
            confiner2D.m_BoundingShape2D = compositeCollider;
            confiner2D.InvalidateCache();
        }
    }
}
