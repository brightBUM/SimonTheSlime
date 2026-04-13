using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkEntryPoint : MonoBehaviour
{
    [SerializeField] Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerEntry()
    {
        animator.SetTrigger("entry");
    }
}
