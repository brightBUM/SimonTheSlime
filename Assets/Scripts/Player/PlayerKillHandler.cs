using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKillHandler : MonoBehaviour
{
    [SerializeField] GameObject gorePrefab;
    [SerializeField] GameObject playerDummy;
    [SerializeField] float sawThrowForce = 20f;
    GameObject goreFx;
    // Start is called before the first frame update
    void Start()
    {
        goreFx = Instantiate(gorePrefab,transform.position,Quaternion.identity);

    }

    public void SawGore(Vector2 dir,Action complete)
    {
        var rot = Quaternion.Euler(0, 0, MathF.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        
        goreFx.transform.position = transform.position;
        goreFx.transform.rotation = rot;
        goreFx.GetComponentInChildren<ParticleSystem>().Play();

        
        var dummyBody = playerDummy.transform.GetChild(0).gameObject;
        dummyBody.transform.position = transform.position;
        dummyBody.SetActive(true);
        dummyBody.GetComponent<Rigidbody2D>().AddForce(dir*sawThrowForce, ForceMode2D.Impulse);

        DOVirtual.DelayedCall(1f, () =>
        {
            this.Reset();
            complete.Invoke();
        });
    }

    public void Reset()
    {
        playerDummy.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ExplosionGore()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
