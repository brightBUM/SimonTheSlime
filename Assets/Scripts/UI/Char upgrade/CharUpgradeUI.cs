using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharUpgradeUI : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] GameObject mileStoneAnchorPrefab;
    [SerializeField] List<UpgradeStateUI> upgradeCards;
    const float sliderOffset = 150f;
    const float cardWidth = 350;

    public static Action UpdateCreatureCount;
    private void OnEnable()
    {
        UpdateCreatureCount += RefreshUpgradeProgress;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var totalCreatureCount = SaveLoadManager.Instance.playerProfile.creatureUnlockStates.Count;
        int unlockedCreatureCount = SaveLoadManager.Instance.GetUnlockedCreaturesCount();
        Debug.Log($"unlocked / total : {unlockedCreatureCount}/{totalCreatureCount} ");

        RectTransform sliderRT = slider.GetComponent<RectTransform>();

        sliderRT.anchorMin = new Vector2(0, 0);
        sliderRT.anchorMax = new Vector2(1, 0);

        sliderRT.offsetMin = new Vector2(200f, sliderRT.offsetMin.y);
        sliderRT.offsetMax = new Vector2(-150f, sliderRT.offsetMax.y);

        for (int i = 0; i < upgradeCards.Count; i++)
        {
            float centerX = sliderOffset + i * cardWidth;
            var cardRectTransform = upgradeCards[i].GetComponent<RectTransform>();
            cardRectTransform.anchoredPosition = new Vector2(centerX+50,cardRectTransform.anchoredPosition.y);


            int reqMilestoneCreatures = Mathf.RoundToInt((float)i * totalCreatureCount / (upgradeCards.Count - 1));
            slider.value = unlockedCreatureCount / (float)totalCreatureCount;
            float normalizedPosition = reqMilestoneCreatures / (float)totalCreatureCount;
            float x = normalizedPosition * sliderRT.rect.width;

            var mileStoneObject = Instantiate(mileStoneAnchorPrefab,slider.transform);
            RectTransform rt = mileStoneObject.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(x, rt.anchoredPosition.y);

            mileStoneObject.GetComponentInChildren<TextMeshProUGUI>().text = reqMilestoneCreatures.ToString();

            //to do unlock card
            if(unlockedCreatureCount >= reqMilestoneCreatures)
                upgradeCards[i].UnlockCard();
        }
        
    }

    public void RefreshUpgradeProgress()
    {
        var totalCreatureCount = SaveLoadManager.Instance.playerProfile.creatureUnlockStates.Count;
        int unlockedCreatureCount = SaveLoadManager.Instance.GetUnlockedCreaturesCount();

        for (int i = 0; i < upgradeCards.Count; i++)
        {
            int reqMilestoneCreatures = Mathf.RoundToInt((float)i * totalCreatureCount / (upgradeCards.Count - 1));
            slider.value = unlockedCreatureCount / (float)totalCreatureCount;

            if (unlockedCreatureCount >= reqMilestoneCreatures)
                upgradeCards[i].UnlockCard();
        }
    }
    private void OnDisable()
    {
        UpdateCreatureCount -= RefreshUpgradeProgress;

    }

}
