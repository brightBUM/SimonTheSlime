using UnityEngine;

public class DropPT : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float deathtime = 1f;
    [SerializeField] Transform parent;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;

        transform.parent = null;
        Destroy(parent.gameObject, deathtime);
        Destroy(this.gameObject, deathtime);
    }
    
}