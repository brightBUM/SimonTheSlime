using CutScene;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


namespace CutScene
{
    public class FillAmountTween : TweenObject
    {
        [SerializeField] Image img;
        [SerializeField] float target;
        public override Tween CustomTween()
        {
            return DOTween.To(() => img.fillAmount, x => img.fillAmount = x, target, duration);
        }
    }
}

