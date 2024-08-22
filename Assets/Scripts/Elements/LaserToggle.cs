using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserToggle : MonoBehaviour
{
    [SerializeField] float toggleInterval;
    [SerializeField] GameObject laserVisual;
    bool poweredOn = true;
    BoxCollider2D boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        StartCoroutine(ToggleLaser());
    }

    IEnumerator ToggleLaser()
    {
        while (true)
        {
            poweredOn = !poweredOn;
            boxCollider.enabled = poweredOn;
            laserVisual.SetActive(poweredOn);
            yield return new WaitForSeconds(toggleInterval);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
