using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboGroup : MonoBehaviour
{
    private int target;
    private int current;
    private float timer;
    private bool comboActive;
    private const float maxLimit = 0.45f;
    
    public Action Collected;
    private void OnEnable()
    {
        Collected += ResetTimer;
    }

    private void ResetTimer()
    {
        comboActive = true;
        current++;
        if(current >= target)
        {
            //combo achived
            //Debug.Log("combo group achieved");
            LevelManager.Instance.ComboAchieved();
            this.enabled = false;
        }
        timer = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        target = transform.childCount;
    }

    private void Update()
    {
        if (!comboActive)
            return;

        if(timer<maxLimit)
        {
            timer += Time.deltaTime;
        }
        else
        {
            //cancel combo
            //Debug.Log("combo group failed");
            comboActive = false;
            this.enabled = false;
        }
    }

    private void OnDisable()
    {
        Collected -= ResetTimer;

    }
}
