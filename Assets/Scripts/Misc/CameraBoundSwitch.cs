using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundSwitch : MonoBehaviour
{
    [SerializeField] CinemachineConfiner2D confiner2D;
    [SerializeField] PolygonCollider2D polygonCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerController>())
        {
            confiner2D.m_BoundingShape2D = polygonCollider;
            confiner2D.InvalidateCache();
        }
    }
}
