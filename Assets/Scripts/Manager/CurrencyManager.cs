using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nanasText;
    [SerializeField] TextMeshProUGUI cursedNanasText;
    [SerializeField] TextMeshProUGUI melonsText;
    [SerializeField] TextMeshProUGUI screwText;
    [SerializeField] TextMeshProUGUI batteriesText;
    [SerializeField] Transform showPostion;
    public static Action<bool> ToggleCurrencyPanel;
    public static Action<CurrencyType, int, int> OnCurrencyAddition;
    public static Action<CurrencyType> TriggerNoCurrencyFeedBack;
    Vector3 startPos;
    private void Start()
    {
        startPos = transform.position;
    }

    private void OnEnable()
    {
        FetchAllCurrency();
        ToggleCurrencyPanel += TweenPanel;
        OnCurrencyAddition += CounterText;
        TriggerNoCurrencyFeedBack += NoCurrencyTweenFeedBack;
    }
    public void FetchAllCurrency()
    {
        var saveLoadInstance = SaveLoadManager.Instance;
        nanasText.text = saveLoadInstance.GetCurrency(CurrencyType.Nanas).ToString();
        cursedNanasText.text = saveLoadInstance.GetCurrency(CurrencyType.cursedNanas).ToString();
        melonsText.text = saveLoadInstance.GetCurrency(CurrencyType.Melons).ToString();
        screwText.text = saveLoadInstance.GetCurrency(CurrencyType.Screws).ToString();
        batteriesText.text = saveLoadInstance.GetCurrency(CurrencyType.Batteries).ToString();
    }
    private void CounterText(CurrencyType currencyType,int from,int amount)
    {
        switch (currencyType)
        {
            case CurrencyType.Nanas:
                Utility.AnimateCounter(nanasText, from, from + amount,1f);
                break;
            case CurrencyType.cursedNanas:
                Utility.AnimateCounter(cursedNanasText, from, from + amount, 1f);
                break;
            case CurrencyType.Melons:
                Utility.AnimateCounter(melonsText, from, from + amount, 1f);
                break;
            case CurrencyType.Screws:
                Utility.AnimateCounter(screwText, from, from + amount, 1f);
                break;
            case CurrencyType.Batteries:
                Utility.AnimateCounter(batteriesText, from, from + amount, 1f);
                break;
        }
    }
    
    private void NoCurrencyTweenFeedBack(CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.Nanas:
                nanasText.rectTransform.DOShakeAnchorPos(0.3f, 20);
                break;
            case CurrencyType.cursedNanas:
                cursedNanasText.rectTransform.DOShakeAnchorPos(0.3f, 20);
                break;
            case CurrencyType.Melons:
                melonsText.rectTransform.DOShakeAnchorPos(0.3f, 20);
                break;
            case CurrencyType.Screws:
                screwText.rectTransform.DOShakeAnchorPos(0.3f, 20);
                break;
            case CurrencyType.Batteries:
                batteriesText.rectTransform.DOShakeAnchorPos(0.3f, 20);
                break;
        }
        SoundManager.Instance.PlayOutofBulletTimeSFX();

    }

    private void TweenPanel(bool value)
    {
        var targetPos = value ? showPostion.position : startPos;
        transform.DOMove(targetPos, 1f).SetEase(Ease.OutBack);
    }
   
    private void OnDisable()
    {
        ToggleCurrencyPanel -= TweenPanel;
        OnCurrencyAddition -= CounterText;
        TriggerNoCurrencyFeedBack -= NoCurrencyTweenFeedBack;

    }
}
