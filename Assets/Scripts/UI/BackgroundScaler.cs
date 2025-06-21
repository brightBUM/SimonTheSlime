using UnityEngine;
[ExecuteInEditMode]
public class BackgroundScaler : MonoBehaviour
{
    public bool reSizeBG;
    public bool reSizeHeight;
    private void Start()
    {
        ResizeBackground();
    }
    private void Update()
    {
        if(reSizeBG)
            ResizeBackground();
        if(reSizeHeight)
            ReSizeHeight();
    }
    void ResizeBackground()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null) return;

        // Get the world height and width based on the camera's view
        float worldHeight = Camera.main.orthographicSize * 2f;
        float worldWidth = worldHeight * Screen.width / Screen.height;

        // Get the sprite’s size in world units
        Vector2 spriteSize = sr.sprite.bounds.size;

        // Calculate the new scale to fit the screen
        transform.localScale = new Vector3(worldWidth / spriteSize.x, worldHeight / spriteSize.y, 1f);
    }
    void ReSizeHeight()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null) return;

        // Get the world height and width based on the camera's view
        float worldHeight = Camera.main.orthographicSize * 2f;
        //float worldWidth = worldHeight * Screen.width / Screen.height;

        // Get the sprite’s size in world units
        Vector2 spriteSize = sr.sprite.bounds.size;

        // Calculate the new scale to fit the screen
        transform.localScale = new Vector3(transform.localScale.x, worldHeight / spriteSize.y, 1f);
    }
}
