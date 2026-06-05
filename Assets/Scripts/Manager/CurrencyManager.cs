using DG.Tweening;
using Firebase.Analytics;
using System;
using System.Collections;
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
    [SerializeField] GameObject[] currencyPrefabs;
    [SerializeField] Canvas canvas;
    [SerializeField] Transform showPostion;
    public static Action<bool> ToggleCurrencyPanel;
    public static Action<CurrencyType, int, int> OnCurrencyAddition;
    public static Action<CurrencyType> TriggerNoCurrencyFeedBack;
    public static Action<int, Vector3, CurrencyType> CurrencyCollectAction;
    Vector3 startPos;

    public const int nanasRatio = 10;
    public const int cursedNanasRatio = 5;
   

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
        CurrencyCollectAction += CurrencyCollectAnimation;
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
    private TextMeshProUGUI GetCurrencyText(CurrencyType currencyType)
    {
        return currencyType switch
        {
            CurrencyType.Nanas => nanasText,
            CurrencyType.cursedNanas => cursedNanasText,
            CurrencyType.Melons => melonsText,
            CurrencyType.Screws => screwText,
            CurrencyType.Batteries => batteriesText,
            _ => null
        };
    }
    private void CounterText(CurrencyType currencyType,int from,int amount)
    {
        var text = GetCurrencyText(currencyType);

        if (text != null)
        Utility.AnimateCounter(text, from, from + amount, 1f);
    }
    private Vector3 GetCurrencyTextPosition(CurrencyType currencyType)
    {
        TMP_Text text = GetCurrencyText(currencyType);

        return text != null ? text.transform.position : Vector3.zero;
    }
    private void NoCurrencyTweenFeedBack(CurrencyType currencyType)
    {
        TMP_Text text = GetCurrencyText(currencyType);

        if (text != null)
            ShakeText(text.rectTransform);

        SoundManager.Instance.PlayOutofBulletTimeSFX();
    }
    public void CurrencyCollectAnimation(int amount, Vector3 spawnPos,CurrencyType currencyType)
    {
        StartCoroutine(CoinRewardRoutine(amount, spawnPos, currencyType));
    }
    public int[] rotationLists = { 360, 720, 1080 };
    public int GetAmountRatios(CurrencyType currencyType)
    {
        return currencyType switch
        {
            CurrencyType.Nanas => nanasRatio,
            CurrencyType.cursedNanas => cursedNanasRatio,
            CurrencyType.Melons => 1,
            CurrencyType.Screws => 1,
            CurrencyType.Batteries => 1,
            _ => 0
        };
    }
    IEnumerator CoinRewardRoutine(int amount,Vector3 spawnPos,CurrencyType currencyType)
    {

        int spawnCount = Mathf.Clamp(amount / GetAmountRatios(currencyType), 1, 10);

        List<GameObject> spawnedCoins = new();

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject coin = Instantiate(currencyPrefabs[(int)currencyType], canvas.transform);
            coin.transform.localScale = Vector3.one*0.75f;
            RectTransform rect = coin.GetComponent<RectTransform>();

            rect.position = spawnPos + (Vector3)UnityEngine.Random.insideUnitCircle * 50f;

            spawnedCoins.Add(coin);

            yield return new WaitForSeconds(0.1f);
        }

        // Optional pause before moving
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < spawnedCoins.Count; i++)
        {
            GameObject coin = spawnedCoins[i];
            RectTransform rect = coin.GetComponent<RectTransform>();

            bool isFirstCoin = i == 0;

            Sequence seq = DOTween.Sequence();

            //seq.Append(
            //    rect.DORotate(
            //        new Vector3(0, 1080, 0),
            //        UnityEngine.Random.Range(0.25f, 0.75f),
            //        RotateMode.FastBeyond360)
            //);

            seq.Append(
                rect.DOMove(
                    GetCurrencyTextPosition(currencyType),
                    UnityEngine.Random.Range(0.6f, 0.9f))
                .SetEase(Ease.InBack)
            );

            seq.OnComplete(() =>
            {
                if (isFirstCoin)
                {
                    //removing amount , bcoz it already got added to saveload by level end , just have to match up on the UI
                    var from = SaveLoadManager.Instance.GetCurrency(currencyType)-amount; 
                    CounterText(currencyType, from, amount);
                }

                Destroy(coin);
            });
        }

    }
    private void ShakeText(RectTransform rect)
    {
        if (DOTween.IsTweening(rect))
            return;

        rect.DOShakeAnchorPos(0.3f, 20f);
    }
    [ContextMenu("tween currency panel")]
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

        CurrencyCollectAction -= CurrencyCollectAnimation;
    }
}
