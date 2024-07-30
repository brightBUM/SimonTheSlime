using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSound : MonoBehaviour
{
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource= GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerAnimation>())
        {
            audioSource.Play();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerAnimation>())
        {
            audioSource.Stop();
        }
    }
}
