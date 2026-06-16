using UnityEngine;

public class ExitChunkSFXTrigger : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerController>())
        {
            audioSource.Play();
        }
    }
}
