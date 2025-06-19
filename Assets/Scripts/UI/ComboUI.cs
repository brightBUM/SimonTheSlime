using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI comboNum;
    [SerializeField] Ease easeType = Ease.OutElastic;
    Tween temp;
    public void TweenCombo(Vector3 pos, int comboValue)
    {
        temp?.Kill();
        transform.localScale = Vector3.zero;

        comboNum.text = comboValue + "X";

        transform.position = pos + Vector3.up * 3f;
        transform.DOScale(0.75f, 0.5f).SetEase(easeType);
        transform.DOMoveY(transform.position.y + 0.5f, 0.5f).SetEase(easeType);

        temp = DOVirtual.DelayedCall(1.5f, () =>
        {
            transform.DOMoveX(transform.position.x-50f, 0.5f).SetEase(Ease.OutFlash).OnComplete(() =>
            {
                transform.localScale = Vector3.zero;
            });
        });
    }
}
