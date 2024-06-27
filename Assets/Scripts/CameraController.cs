using DG.Tweening;

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float followSpeed = 2f;
    //[Header("pound effect")]
    [SerializeField] float poundShakeStrength;
    [SerializeField] float poundShakeDuration;
    public Transform lastCheckPoint;
    Vector3 offset;
    Vector3 followPosition;

    // Start is called before the first frame update
    void Start()
    {
        offset = target.position-this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void FixedUpdate()
    {
        followPosition = target.position + (-offset);
        transform.position = Vector3.Lerp(transform.position, followPosition, Time.deltaTime * followSpeed);
        
    }
    private void LateUpdate()
    {
    }

    public void CameraPoundEffect()
    {
        //camera shake while pound
        transform.DOShakePosition(poundShakeDuration, poundShakeStrength, 10, Random.Range(10,90), false);
    }
}
