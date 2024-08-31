using UnityEngine;

public class patrol : MonoBehaviour
{
    [SerializeField] private float _PlatSpeed;
    [SerializeField] private bool flip;

    [SerializeField] Transform ptA;
    [SerializeField] Transform ptB;
    [SerializeField] Transform firstTarget;
    SpriteRenderer spr;
    Vector3 target;
    Vector3 direction;
   
    void Start()
    {
        if (_PlatSpeed <= 0)
            return;
        spr = GetComponent<SpriteRenderer>();
        direction = Vector2.up;
        target = firstTarget.position;
    }

    void Update()
    {

        if (_PlatSpeed <= 0)
            return;

        if (Vector2.Distance(transform.position, target) > 0.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, _PlatSpeed * Time.deltaTime);
        }
        else
        {
            target = GetTarget();

        }

    }
   
    private Vector3 GetTarget()
    {
        var dis1 = Vector2.Distance(transform.position, ptA.position);
        var dis2 = Vector2.Distance(transform.position, ptB.position);
        if (dis1 > 1f)
        {
            return ptA.position;
        }
        else if (dis2 > 1f)
        {
            return ptB.position;
        }
        return Vector3.zero;

    }
    
}