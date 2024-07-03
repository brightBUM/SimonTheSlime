using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxSystem : MonoBehaviour
{
    [SerializeField] ParallaxLayer[] backGroundLayers;
    [SerializeField] ParallaxLayer foreGroundLayer;
    [SerializeField] CameraController cameraController;
    // Start is called before the first frame update
   
    private void OnEnable()
    {
        cameraController.camMovement += MoveLayers;
    }

    private void MoveLayers(float delta)
    {
        foreach(var background in backGroundLayers)
        {
            Vector3 newPos = background.layer.position;
            newPos.x -= delta * background.parallaxFactor;

            background.layer.position = newPos;
        }
    }
    private void OnDisable()
    {
        cameraController.camMovement -= MoveLayers;
    }
}

[System.Serializable]
class ParallaxLayer
{
    public Transform layer;
    public float parallaxFactor;
}
