using UnityEngine;

//[ExecuteInEditMode]
public class ParallaxLooper : MonoBehaviour
{
    public Transform[] backgrounds;   // Assign 3 background transforms
    public float parallaxSpeed = 0.5f;
    public float parallaxSpeedY = 1f;
    public Transform player;

    private float backgroundWidth;
    private Vector3 lastPlayerPosition;

    void Start()
    {
        if(player==null)
        {
            player = Camera.main.transform;
        }

        backgroundWidth = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;
        lastPlayerPosition = player.position;
    }
    
    private void Update()
    {
        var delta = player.position - lastPlayerPosition;

        // Move each background according to player delta and parallax speed
        foreach (Transform bg in backgrounds)
        {
            bg.position += new Vector3(delta.x * parallaxSpeed, delta.y * parallaxSpeedY, 0f);
            //bg.position += new Vector3(delta.x * parallaxSpeed, 0f, 0f);
        }

        // Loop backgrounds
        foreach (Transform bg in backgrounds)
        {
            float distanceFromPlayer = bg.position.x - player.position.x;

            if (distanceFromPlayer > backgroundWidth * 1.5f)
            {
                // Too far to the right, move it to the left end
                Transform leftmost = GetLeftmostBackground();
                bg.position = new Vector3(leftmost.position.x - backgroundWidth, bg.position.y, bg.position.z);
            }
            else if (distanceFromPlayer < -backgroundWidth * 1.5f)
            {
                // Too far to the left, move it to the right end
                Transform rightmost = GetRightmostBackground();
                bg.position = new Vector3(rightmost.position.x + backgroundWidth, bg.position.y, bg.position.z);
            }
        }

        lastPlayerPosition = player.position;
    }
    
    Transform GetRightmostBackground()
    {
        Transform rightmost = backgrounds[0];
        foreach (Transform bg in backgrounds)
        {
            if (bg.position.x > rightmost.position.x)
                rightmost = bg;
        }
        return rightmost;
    }

    Transform GetLeftmostBackground()
    {
        Transform leftmost = backgrounds[0];
        foreach (Transform bg in backgrounds)
        {
            if (bg.position.x < leftmost.position.x)
                leftmost = bg;
        }
        return leftmost;
    }
}
