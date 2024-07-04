using System;
using Unity.Mathematics;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Material material;  // Reference to the background transform
        public float parallaxFactor;  // The parallax effect multiplier for this layer
    }

    public ParallaxLayer[] layers;  // Array to store the background layers
    private Vector3 previousCameraPosition;  // Store the camera's position in the previous frame
    private Transform cameraTransform;  // Reference to the camera's transform

    void Start()
    {
        // Get the camera's transform
        cameraTransform = Camera.main.transform;

        // Store the initial camera position
        previousCameraPosition = cameraTransform.position;
    }
    private void FixedUpdate()
    {
        // Calculate the camera movement since the last frame
        Vector3 cameraMovement = cameraTransform.position - previousCameraPosition;

        // Loop through each layer
        foreach (ParallaxLayer layer in layers)
        {
            // Calculate the parallax movement for this layer
            Vector3 parallaxMovement = cameraMovement * layer.parallaxFactor * Time.fixedDeltaTime;

           
            // Move the background by the calculated amount
            layer.material.mainTextureOffset += new Vector2(parallaxMovement.x, 0);
        }

        // Store the current camera position for the next frame
        previousCameraPosition = cameraTransform.position;
    }
    void LateUpdate()
    {
        
    }
}
