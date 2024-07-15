using DG.Tweening;
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] Transform lookAhead;
    [SerializeField] float followSpeed = 2f;
    [Header("pound effect")]
    [SerializeField] float poundShakeStrength;
    [SerializeField] float poundShakeDuration;
    [Header("hit effect")]
    [SerializeField] float hitShakeStrength = 2f;
    [SerializeField] float hitShakeDuration = 0.2f;
    Transform target;
    Vector3 offset;
    Vector3 followPosition;
    public Action<float> camMovement;
    float oldPosition;
    // Start is called before the first frame update
    void Start()
    {
        target = lookAhead.transform;
        offset = target.position-this.transform.position;
        //oldPosition = transform.position.x;
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
        //var delta = oldPosition - transform.position.x;
        //camMovement(delta);

        followPosition = target.position + (-offset);
        transform.position = Vector3.Lerp(transform.position, followPosition, Time.deltaTime * followSpeed);

        //oldPosition = transform.position.x;
    }
    
    public void CameraPoundEffect()
    {
        //camera shake while pound
        transform.DOShakePosition(poundShakeDuration, poundShakeStrength, 10, UnityEngine.Random.Range(10,90), false);
    }
    public void CameraHitEffect()
    {
        //camera shake while player Hit
        transform.DOShakePosition(hitShakeDuration, hitShakeStrength, 10, UnityEngine.Random.Range(10, 90), false);
    }
}
