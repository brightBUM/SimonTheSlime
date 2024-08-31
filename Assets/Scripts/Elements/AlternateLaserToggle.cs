using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateLaserToggle : MonoBehaviour
{
    [SerializeField] Laser[] lasers;
    [SerializeField] float[] toggleIntervals;
    bool inRange;

    Coroutine coroutine;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void TriggerActivation()
    {
        inRange = true;
        StartCoroutine(ToggleLasers(lasers[0], lasers[1], toggleIntervals[0]));
        StartCoroutine(ToggleLasers(lasers[3], lasers[2], toggleIntervals[1]));
        StartCoroutine(ToggleLasers(lasers[4], lasers[5], toggleIntervals[2]));
    }
    public void DeActivate()
    {
        StopAllCoroutines();
        inRange = false;
    }

    IEnumerator ToggleLasers(Laser laser1, Laser laser2, float interval)
    {
        while(inRange)
        {
            LaserActivation(laser1, false);
            LaserActivation(laser2, true);

            yield return new WaitForSeconds(interval);

            LaserActivation(laser1, true);
            LaserActivation(laser2, false);

            yield return new WaitForSeconds(interval);

        }
    }
    

    public void LaserActivation(Laser laser,bool value)
    {
        laser.boxCollider.enabled = value;
        laser.laserVisual.SetActive(value);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            TriggerActivation();
            Debug.Log("player entered");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            DeActivate();
            Debug.Log("player exit");

        }
    }
}
[System.Serializable]
public class Laser
{
    public BoxCollider2D boxCollider;
    public GameObject laserVisual;

}