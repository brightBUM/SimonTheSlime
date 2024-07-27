using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyDeform : MonoBehaviour
{
    private Vector3 originalScale;
    public float bounceFactor = 1.2f; // How much it should deform
    public float deformDuration = 0.2f; // Time it takes to deform and return to normal

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    private void Update()
    {
        
    }
    public void HitDeform()
    {
        StopAllCoroutines(); // Stop any ongoing deformation
        StartCoroutine(Deform());
    }
    private IEnumerator Deform()
    {
        Vector3 targetScale = new Vector3(originalScale.x * bounceFactor, originalScale.y / bounceFactor, originalScale.z);

        // Deform
        float elapsedTime = 0f;
        while (elapsedTime < deformDuration / 2)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, (elapsedTime / (deformDuration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restore to original
        elapsedTime = 0f;
        while (elapsedTime < deformDuration / 2)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, (elapsedTime / (deformDuration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale; // Ensure it's exactly the original scale at the end
    }
}

