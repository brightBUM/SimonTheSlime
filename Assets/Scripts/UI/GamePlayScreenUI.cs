using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayScreenUI : MonoBehaviour
{
    [SerializeField] Image midAirJumpFillImage;
    [SerializeField] Image midAirJumpIcon;
    [SerializeField] Image bulletTimeIcon;
    [SerializeField] TextMeshProUGUI bulletTimeText;
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
    public void UpdateBulletTimeUI(int num)
    {
        bulletTimeText.text = num.ToString();

        float alpha = 1f;
        alpha = num > 0 ? 1f : 0.2f;
        bulletTimeIcon.color = new Color(bulletTimeIcon.color.r, bulletTimeIcon.color.g, bulletTimeIcon.color.b,alpha);
        
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
