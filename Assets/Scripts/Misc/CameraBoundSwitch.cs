using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundSwitch : MonoBehaviour
{
    [SerializeField] CinemachineConfiner2D confiner2D;
    [SerializeField] List<CompositeCollider2D> compositeCollider;
    int index = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            if(playerController.transform.position.x<transform.position.x)
            {
                //entered left side
                Debug.Log($"entered left");
                index++;
                if(index<compositeCollider.Count)
                {
                    confiner2D.m_BoundingShape2D = compositeCollider[index];
                    confiner2D.InvalidateCache();
                }
            }
            else
            {
                //entered right side
                Debug.Log($"entered right");
                index--;
                if(index>=0)
                {
                    confiner2D.m_BoundingShape2D = compositeCollider[index];
                    confiner2D.InvalidateCache();
                }
            }

            
        }
    }
}
