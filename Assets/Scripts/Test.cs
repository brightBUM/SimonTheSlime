using DG.Tweening;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject gorePrefab;
    [SerializeField] GameObject impactVFX;
    [SerializeField] Transform pauseScreen;
    [SerializeField] Ease easeType;
    [SerializeField] float duration;

    ParticleSystem FXprefab;
    ParticleSystem ImpPrefab;
    private void Start()
    {
        FXprefab = Instantiate(gorePrefab).GetComponent<ParticleSystem>();
        ImpPrefab = Instantiate (impactVFX).GetComponent<ParticleSystem>();
    }
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            FXprefab.transform.position = new Vector3(pos.x, pos.y, 0);
            ImpPrefab.transform.position = new Vector3(pos.x, pos.y, 0);
            FXprefab.Play();
            ImpPrefab.Play();
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    pauseScreen.DOScale(Vector3.one, duration).SetEase(easeType);
        //}
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    pauseScreen.localScale = Vector3.zero;
        //}
    }

    
}
