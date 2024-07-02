using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thumper : MonoBehaviour
{
    int state;
    [SerializeField] float poundInterval = 1f;
    [SerializeField] Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ThumperActivity());
    }

    IEnumerator ThumperActivity()
    {
        while (true)
        {
            yield return new WaitForSeconds(poundInterval);

            //pound
            state = 1;
            animator.SetInteger("state",state);

            yield return new WaitForSeconds(poundInterval);
            //retract
            state = 2;
            animator.SetInteger("state", state);

            yield return new WaitForSeconds(poundInterval);
            //idle
            state = 0;
            animator.SetInteger("state", state);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            if(playerController.playerState!=State.SQUISHED)
                playerController.SetToSquishState();
        }
    }
}
