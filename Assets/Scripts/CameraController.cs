using DG.Tweening;

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] float followSpeed = 2f;
    [Header("pound effect")]
    [SerializeField] float poundShakeStrength;
    [SerializeField] float poundShakeDuration;
    [Header("hit effect")]
    [SerializeField] float hitShakeStrength = 2f;
    [SerializeField] float hitShakeDuration = 0.2f;
    public Transform lastCheckPoint;
    Transform target;
    Vector3 offset;
    Vector3 followPosition;
    // Start is called before the first frame update
    void Start()
    {
        target = playerController.transform;
        offset = target.position-this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if(playerController.playerState != State.GHOST)
        {
            FollowPlayer();
        }
    }
    private void LateUpdate()
    {
        if (playerController.playerState == State.GHOST)
        {
            FollowPlayer();
        }
    }
    private void FollowPlayer()
    {
        followPosition = target.position + (-offset);
        transform.position = Vector3.Lerp(transform.position, followPosition, Time.deltaTime * followSpeed);
    }
    
    public void CameraPoundEffect()
    {
        //camera shake while pound
        transform.DOShakePosition(poundShakeDuration, poundShakeStrength, 10, Random.Range(10,90), false);
    }
    public void CameraHitEffect()
    {
        //camera shake while pound
        transform.DOShakePosition(poundShakeDuration, poundShakeStrength, 10, Random.Range(10, 90), false);
    }
}
