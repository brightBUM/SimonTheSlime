using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxSystem : MonoBehaviour
{
    [SerializeField] ParallaxLayer[] backGroundLayers;
    [SerializeField] ParallaxLayer foreGroundLayer;
    [SerializeField] CameraController cameraController;
    // Start is called before the first frame update
    void Start()
    {
        cameraController.camMovement += MoveLayers;
    }

    private void MoveLayers(float delta)
    {
        foreach(var item in backGroundLayers)
        {
            Vector3 newPos = item.layer.localPosition;
            newPos.x -= delta * item.parallaxFactor;

            item.layer.localPosition = newPos;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
class ParallaxLayer
{
    public Transform layer;
    public float parallaxFactor;
}
