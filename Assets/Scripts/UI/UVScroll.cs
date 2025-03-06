using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UVScroll : MonoBehaviour
{
    RawImage rawImage;
    [SerializeField] float scrollSpeed = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        rawImage.uvRect = new Rect(rawImage.uvRect.position + Vector2.one * Time.deltaTime * scrollSpeed, rawImage.uvRect.size);
    }
}
