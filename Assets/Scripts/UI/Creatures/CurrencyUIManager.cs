using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyUIManager : MonoBehaviour
{
    [SerializeField] int coinAmount; 
    public static CurrencyUIManager Instance;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] TextMeshProUGUI starsText;
    [SerializeField] TextMeshProUGUI gemsText;

    [Header("Targets")]
    [SerializeField] RectTransform coinsTarget;
    [SerializeField] RectTransform starsTarget;
    [SerializeField] RectTransform gemsTarget;

    [Header("FX")]
    [SerializeField] GameObject flyingCoinPrefab;
    [SerializeField] GameObject flyingGemPrefab;

    [SerializeField] Canvas canvas;

    int currentCoins;
    int currentStars;
    int currentGems;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        currentCoins = 100;
        currentStars = 100;
        currentGems = 100;
        //currentCoins =
        //    PlayerPrefs.GetInt("COINS", 0);

        //currentStars =
        //    SaveManager.Instance.GetTotalStars();

        //currentGems =
        //    PlayerPrefs.GetInt("GEMS", 0);

        coinsText.text = currentCoins.ToString();

        starsText.text = currentStars.ToString();

        gemsText.text = currentGems.ToString();
    }

    #region STARS

    public void AnimateStars(int previous, int current)
    {
        StartCoroutine(
            AnimateCurrencyText(
                starsText,
                previous,
                current
            )
        );
    }

    #endregion

    #region COINS
    [ContextMenu("triggerCoinSpawn")]
    private void SpawnCoins()
    {
        //RewardCoins(coinAmount, transform.position);
    }
    

    #endregion

    #region GEMS

    public void RewardGems(
        int amount,
        Vector3 spawnPos)
    {
        StartCoroutine(
            GemRewardRoutine(amount, spawnPos)
        );
    }

    IEnumerator GemRewardRoutine(
        int amount,
        Vector3 spawnPos)
    {
        int previousGems = currentGems;

        currentGems += amount;

        PlayerPrefs.SetInt(
            "GEMS",
            currentGems
        );

        ShowFloatingText(
            "+" + amount,
            spawnPos
        );

        int spawnCount =
            Mathf.Clamp(amount, 3, 10);

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject gem =
                Instantiate(
                    flyingGemPrefab,
                    canvas.transform
                );

            RectTransform rect =
                gem.GetComponent<RectTransform>();

            rect.position =
                spawnPos +
                (Vector3)Random.insideUnitCircle * 100f;

            rect.DOMove(
                gemsTarget.position,
                Random.Range(0.6f, 0.9f)
            )
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                Destroy(gem);
            });

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.4f);

        StartCoroutine(
            AnimateCurrencyText(
                gemsText,
                previousGems,
                currentGems
            )
        );
    }

    #endregion

    #region COMMON

    IEnumerator AnimateCurrencyText(
        TextMeshProUGUI text,
        int from,
        int to)
    {
        text.transform.DOKill();

        text.transform
            .DOScale(1.25f, 0.2f)
            .SetLoops(2, LoopType.Yoyo);

        float duration = 0.5f;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            int value =
                Mathf.RoundToInt(
                    Mathf.Lerp(
                        from,
                        to,
                        timer / duration
                    )
                );

            text.text = value.ToString();

            yield return null;
        }

        text.text = to.ToString();
    }

    void ShowFloatingText(
        string value,
        Vector3 pos)
    {
        // Optional later
    }

    #endregion
}