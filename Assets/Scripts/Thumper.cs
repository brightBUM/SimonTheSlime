using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;
public enum HitDirection
{
    Up, Down, Left, Right
}
public class Thumper : MonoBehaviour
{
    int state;
    [SerializeField] Animator animator;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] HitDirection hitDirection;
    [SerializeField] float idleInterval = 1.5f;
    [SerializeField] float poundInterval = 0.4f;
    [SerializeField] float retractInterval = 0.6f;
    [SerializeField] Transform boxRef;
    [SerializeField] float boxSize = 3f;
    bool hit;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ThumperActivity());
    }

    IEnumerator ThumperActivity()
    {
        while (true)
        {
            //pound
            state = 1;
            animator.SetInteger("state",state);

            yield return new WaitForSeconds(poundInterval);
            //retract
            state = 2;
            animator.SetInteger("state", state);

            yield return new WaitForSeconds(retractInterval);
            //idle
            state = 0;
            animator.SetInteger("state", state);

            yield return new WaitForSeconds(idleInterval);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.SetToSquishState(this.transform.position,this.hitDirection);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //box cast and check if player is below the thumper
        //RaycastHit2D hitinfo = Physics2D.BoxCast(boxRef.position, Vector2.one * boxSize, 0, Vector2.down, 0f, playerLayer);
        //if (hitinfo.collider!=null)
        //{
        //    hit = true;
        //    var playerController = hitinfo.collider.GetComponent<PlayerController>();
        //    Debug.Log("hit point : " + hitinfo.point);
        //    if (playerController.playerState != State.SQUISHED && playerController.playerState != State.GHOST)
        //    {
        //        //check the thumper orientation and spawn squish dummy according to the direction

        //        playerController.SetToSquishState();
        //        //Debug.Log("squish called");
        //    }
        //}
        //else
        //{
        //    hit = false;
        //}
    }


    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = hit ? Color.green : Color.red;
    //    Gizmos.DrawCube(boxRef.position, Vector3.one * boxSize);

    //}
}
