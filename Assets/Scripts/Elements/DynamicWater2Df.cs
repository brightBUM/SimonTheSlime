using UnityEngine;

public class DynamicWater2Df : MonoBehaviour
{
    [SerializeField]
    Vector2 bound = Vector2.one;
    [SerializeField, Min(10)]
    int resolution = 50;
    [SerializeField]
    Material waterMaterial;
    [SerializeField]
    float
        springConstant = .02f,
        damping = .1f,
        spread = .1f,
        collisionVelocityFactor = .04f;

    Vector3[] vertices;
    Mesh mesh;
    float[] velocities, accelerations;
    float timer;
    private float restingWaterHeight;
    private void Start()
    {
        restingWaterHeight = bound.y / 2f; // Top edge of centered mesh
        InitializePhysics();
        GenerateMesh();
        SetBosCollider2D();
    }

    void GenerateMesh()
    {
        float halfWidth = bound.x / 2f;
        float halfHeight = bound.y / 2f;
        float range = bound.x / (resolution - 1);
        float xOffset = -halfWidth;

        vertices = new Vector3[resolution * 2];

        // Top row (wave surface)
        for (int i = 0; i < resolution; i++)
        {
            float x = xOffset + i * range;
            vertices[i] = new Vector3(x, halfHeight, 0f);
        }

        // Bottom row
        for (int i = 0; i < resolution; i++)
        {
            float x = xOffset + i * range;
            vertices[i + resolution] = new Vector3(x, -halfHeight, 0f);
        }

        // Triangle construction
        int[] tris = new int[((resolution - 1) * 2) * 3];
        int t = 0;

        for (int i = 0; i < resolution - 1; i++)
        {
            int topLeft = i;
            int bottomLeft = i + resolution;
            int topRight = i + 1;
            int bottomRight = i + resolution + 1;

            // First triangle
            tris[t++] = topLeft;
            tris[t++] = bottomLeft;
            tris[t++] = bottomRight;

            // Second triangle
            tris[t++] = topLeft;
            tris[t++] = bottomRight;
            tris[t++] = topRight;
        }

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        if (waterMaterial) meshRenderer.sharedMaterial = waterMaterial;

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    void InitializePhysics()
    {
        velocities = new float[resolution];
        accelerations = new float[resolution];
    }

    private void SetBosCollider2D()
    {
        BoxCollider2D collision = gameObject.AddComponent<BoxCollider2D>();
        collision.isTrigger = true;
        collision.size = bound;
        collision.offset = Vector2.zero; // Centered
    }

    private void Update()
    {
        if (timer <= 0) return;
        timer -= Time.unscaledDeltaTime;

        for (int i = 0; i < resolution; i++)
        {
            float force = springConstant * (vertices[i].y - restingWaterHeight) + velocities[i] * damping;
            accelerations[i] = -force;
            vertices[i].y += velocities[i];
            velocities[i] += accelerations[i];
        }

        for (int i = 0; i < resolution; i++)
        {
            if (i > 0)
            {
                float l = spread * (vertices[i].y - vertices[i - 1].y);
                velocities[i - 1] += l;
            }

            if (i < resolution - 1)
            {
                float r = spread * (vertices[i].y - vertices[i + 1].y);
                velocities[i + 1] += r;
            }
        }

        mesh.vertices = vertices;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        Splash(collision, rb.velocity.y * collisionVelocityFactor);
    }

    private void Splash(Collider2D collision, float force)
    {
        timer = 3f;
        float radius = collision.bounds.max.x - collision.bounds.min.x;
        Vector2 center = new Vector2(collision.bounds.center.x, transform.position.y + restingWaterHeight);

        //GameObject splashGO = Instantiate(splash, new Vector3(center.x, center.y, 0), Quaternion.Euler(0, 0, 60));
        //Destroy(splashGO, 2f);

        for (int i = 0; i < resolution; i++)
        {
            if (PointInsideCircle(transform.TransformPoint(vertices[i]), center, radius))
            {
                velocities[i] = force;
                Debug.Log("boom");
            }
        }
    }

    bool PointInsideCircle(Vector2 point, Vector2 center, float radius)
    {
        return Vector2.Distance(point, center) < radius;
    }

    private void OnDrawGizmos()
    {
        
        //Gizmos.DrawWireCube(transform.position, new Vector3(bound.x, bound.y, 0));
    }
}
