using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI comboNum;
    public void TweenCombo(Vector3 pos, int comboValue)
    {
        transform.localScale = Vector3.zero;

        comboNum.text = comboValue + "x";

        transform.position = pos + Vector3.up * 3f;
        transform.DOScale(1, 0.5f).SetEase(Ease.InOutFlash);
        transform.DOMoveY(transform.position.y + 0.5f, 0.5f).SetEase(Ease.InOutFlash);
    }
}
