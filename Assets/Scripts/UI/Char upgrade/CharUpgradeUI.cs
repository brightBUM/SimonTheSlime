using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharUpgradeUI : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] GameObject mileStoneAnchorPrefab;
    [SerializeField] GameObject upgradeStateUIPrefab;
    [SerializeField] RectTransform cardsLayout;
    [SerializeField] RectTransform content;
    [SerializeField] RectTransform mileStoneContainer;
    [SerializeField] List<UpgradeCard> upgradeCardsOrder;
     List<UpgradeStateUI> upgradeStateUIs = new List<UpgradeStateUI>();
    const float sliderOffset = 150f;
    const float cardWidth = 350;

    public static Action UpdateCreatureCount;
    private void OnEnable()
    {
        UpdateCreatureCount += RefreshUpgradeProgress;
    }
    private IEnumerator Start()
    {
        foreach(UpgradeCard upgradeCard in upgradeCardsOrder)
        {
            var upgradeStateUIObject = Instantiate(upgradeStateUIPrefab, cardsLayout);
            var upgradeStateUI = upgradeStateUIObject.GetComponent<UpgradeStateUI>();
            upgradeStateUI.Init(upgradeCard.upgradeStatId, upgradeCard.upgradeIndex);
            upgradeStateUIs.Add(upgradeStateUI);
        }

        yield return null;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(cardsLayout);

        float width = cardsLayout.rect.width;
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        AlignCardsWithMileStones();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void AlignCardsWithMileStones()
    {
        var totalCreatureCount = SaveLoadManager.Instance.playerProfile.creatureUnlockStates.Count;
        int unlockedCreatureCount = SaveLoadManager.Instance.GetUnlockedCreaturesCount();
        Debug.Log($"unlocked / total : {unlockedCreatureCount}/{totalCreatureCount} ");

        RectTransform first = upgradeStateUIs[0].GetComponent<RectTransform>();

        RectTransform last = upgradeStateUIs[^1].GetComponent<RectTransform>();

        RectTransform sliderRT = slider.GetComponent<RectTransform>();

        float left = first.anchoredPosition.x;
        float right = last.anchoredPosition.x;

        float cardsWidth = cardsLayout.rect.width;

        sliderRT.offsetMin = new Vector2(left, sliderRT.offsetMin.y);
        sliderRT.offsetMax = new Vector2(-(cardsWidth - right), sliderRT.offsetMax.y);

        for (int i = 0; i < upgradeStateUIs.Count; i++)
        {

            int reqMilestoneCreatures = Mathf.RoundToInt((float)i * totalCreatureCount / (upgradeStateUIs.Count - 1));
            slider.value = unlockedCreatureCount / (float)totalCreatureCount;

            RectTransform card = upgradeStateUIs[i].GetComponent<RectTransform>();

            GameObject marker =
                Instantiate(mileStoneAnchorPrefab, mileStoneContainer);

            RectTransform markerRT =
                marker.GetComponent<RectTransform>();

            markerRT.anchorMin =
            markerRT.anchorMax =
                new Vector2(0, .5f);

            markerRT.anchoredPosition =
                new Vector2(card.anchoredPosition.x, 0);

            marker.GetComponentInChildren<TextMeshProUGUI>().text = reqMilestoneCreatures.ToString();

            //to do unlock card
            if(unlockedCreatureCount >= reqMilestoneCreatures)
                upgradeStateUIs[i].UnlockCard();
        }
        
    }

    public void RefreshUpgradeProgress()
    {
        Debug.Log("char upgrade refresh called");
        var totalCreatureCount = SaveLoadManager.Instance.playerProfile.creatureUnlockStates.Count;
        int unlockedCreatureCount = SaveLoadManager.Instance.GetUnlockedCreaturesCount();

        for (int i = 0; i < upgradeStateUIs.Count; i++)
        {
            int reqMilestoneCreatures = Mathf.RoundToInt((float)i * totalCreatureCount / (upgradeStateUIs.Count - 1));
            slider.value = unlockedCreatureCount / (float)totalCreatureCount;

            if (unlockedCreatureCount >= reqMilestoneCreatures)
                upgradeStateUIs[i].UnlockCard();
        }
    }
    private void OnDisable()
    {
        UpdateCreatureCount -= RefreshUpgradeProgress;

    }

}
[System.Serializable]
public class UpgradeCard
{
    public UpgradeStatId upgradeStatId;
    public int upgradeIndex;
}
