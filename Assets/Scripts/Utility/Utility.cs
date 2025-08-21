using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class Utility
{
    public static void AnimateCounter(TextMeshProUGUI counterText, int from, int to, float time)
    {
        int currentValue = from;
        counterText.text = currentValue.ToString();

        DOTween.To(() => currentValue, x =>
        {
            currentValue = x;
            counterText.text = currentValue.ToString();
        }
        , to, time).SetEase(Ease.OutQuad);

        counterText.rectTransform.DOScale(1.2f, 0.25f).SetLoops(4, LoopType.Yoyo);
    }

    public static T RandomUniqueItemFromList<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning($"Tried to get item from empty list of {typeof(T).Name}!");
            return default;
        }

        int index = Random.Range(0, list.Count);
        T chosen = list[index];
        list.RemoveAt(index); // remove so it won’t be picked again
        return chosen;
    }

    public static T RandomItemFromList<T>(List<T> listItems)
    {
        return listItems[Random.Range(0, listItems.Count)];
    }
}