using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class VerticalScroll : MonoBehaviour
{
    public float scrollSpeed = 0.5f; // Speed of vertical movement

    private Material mat;
    private Vector2 offset;

    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
        offset = mat.mainTextureOffset;
    }

    void Update()
    {
        offset.y += scrollSpeed * Time.deltaTime;
        mat.mainTextureOffset = offset;
    }
}