using UnityEngine;
using UnityEngine.UI;

public class UVScroll : MonoBehaviour
{
    RawImage rawImage;
    [SerializeField] float scrollSpeed = 0.01f;
    [SerializeField] Vector2 direction = Vector2.one;
    // Start is called before the first frame update
    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        rawImage.uvRect = new Rect(rawImage.uvRect.position + direction * Time.deltaTime * scrollSpeed, rawImage.uvRect.size);
    }
}
