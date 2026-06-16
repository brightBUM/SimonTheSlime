using Cinemachine;
using UnityEngine;

public class PlayerTransitionDummy : MonoBehaviour
{
    [SerializeField] float dropSpeed;
    [SerializeField] VerticalScroll scroll_1;
    [SerializeField] VerticalScroll scroll_2;
    [SerializeField] Animator animator;
    [SerializeField] TrailRenderer trailRenderer;
    public CinemachineVirtualCamera vcam;
    public TrailRenderer trail;
    public float minTrailTime = 0.2f;
    public float maxTrailTime = 0.4f;
    public float minYDamping = 0.5f;
    public float maxYDamping = 0.75f;
    public float speed = 1f; // Oscillation speed

    private CinemachineFramingTransposer framing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Vector3 direction;
    public void Init(bool down)
    {
        framing = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (down)
        {
            scroll_1.scrollSpeed = -5;
            scroll_2.scrollSpeed = -8;
            direction = Vector3.down;
            SoundManager.Instance.PlayFallingSFx();
        }
        else
        {
            scroll_1.scrollSpeed = 5;
            scroll_2.scrollSpeed = 8;
            animator.SetTrigger("sleep");
            direction = Vector3.down;
            trailRenderer.enabled = false;
        }

    }
    // Update is called once per frame
    void Update()
    {

        transform.position += direction * dropSpeed * Time.deltaTime;
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f; // 0 → 1

        // Camera Y Damping
        float yDamping = Mathf.Lerp(minYDamping, maxYDamping, t);
        framing.m_YDamping = yDamping;

        // Trail Time
        float trailTime = Mathf.Lerp(minTrailTime, maxTrailTime, t);
        trail.time = trailTime;
    }
}
