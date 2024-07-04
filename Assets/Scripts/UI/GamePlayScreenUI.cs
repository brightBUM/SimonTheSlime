using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayScreenUI : MonoBehaviour
{
    [SerializeField] Image midAirJumpFillImage;
    [SerializeField] Image midAirJumpIcon;
    [SerializeField] TextMeshProUGUI slomoText;
    [SerializeField] TextMeshProUGUI bananaText;
    public static GamePlayScreenUI instance;
    public Action<float> UpdateMidAirJumpUI;
    private void OnEnable()
    {
        UpdateMidAirJumpUI += UpdateMidAirUIAbility;
    }
    private void Awake()
    {
        instance = this;
    }

    private void UpdateMidAirUIAbility(float value)
    {
        midAirJumpFillImage.fillAmount = value;
    }
    void Start()
    {
        UpdateMidAirUIAbility(0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDisable()
    {
        UpdateMidAirJumpUI -= UpdateMidAirUIAbility;

    }
}
