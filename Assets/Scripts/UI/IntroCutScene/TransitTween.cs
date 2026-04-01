using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CutScene
{
    public class TransitTween :TweenObject
    {
        [SerializeField] Image image1;
        [SerializeField] Image image2;
        public override Tween CustomTween()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(0.25f);

            // Step 1

            sequence.Append(image1.DOFade(0, 0.5f));
            sequence.Join(image2.DOFade(1, 0.5f));

            return sequence;
        }
    }
}
