using CutScene;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class FillAmountTween : TweenObject
{
    [SerializeField] Image img;
    [SerializeField] float target;
    public override Tween CustomTween()
    {
        return DOTween.To(() => img.fillAmount, x => img.fillAmount = x, target, duration);
    }
}
