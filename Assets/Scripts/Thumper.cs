using System.Collections;
using UnityEngine;

public class Thumper : MonoBehaviour
{
    int state;
    [SerializeField] Animator animator;
    [SerializeField] float poundInterval = 0.3f;
    [SerializeField] float retractInterval = 0.5f;
    [SerializeField] Transform boxRef;
    [SerializeField] float boxSize = 3f;
    [SerializeField] LayerMask playerLayer;
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
            yield return new WaitForSeconds(retractInterval);

            //pound
            state = 1;
            animator.SetInteger("state",state);

            yield return new WaitForSeconds(retractInterval);
            //retract
            state = 2;
            animator.SetInteger("state", state);

            yield return new WaitForSeconds(retractInterval);
            //idle
            state = 0;
            animator.SetInteger("state", state);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //box cast and check if player is below the thumper
        RaycastHit2D hitinfo = Physics2D.BoxCast(boxRef.position, Vector2.one * boxSize, 0, Vector2.down, 0f, playerLayer);
        if (hitinfo.collider!=null)
        {
            //hit = true;
            var playerController = hitinfo.collider.GetComponent<PlayerController>();
            if (playerController.playerState != State.SQUISHED && playerController.playerState != State.GHOST)
            {
                playerController.SetToSquishState();
                //Debug.Log("squish called");
            }
        }
        else
        {
            //hit = false;
        }
    }


    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = hit ? Color.green : Color.red;
    //    Gizmos.DrawCube(boxRef.position, Vector3.one * boxSize);

    //}
}
