using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CutScene
{
    public class AnimateTween : TweenObject
    {
        [SerializeField] List<Image> images;
        [SerializeField] AudioClip alertSound;
        [SerializeField] AudioClip VFXSound;
        [SerializeField] AudioClip fallSound;
        [SerializeField] Transform alertTransform;
        [SerializeField] CutSceneAudio cutSceneAudio;
        [SerializeField] ParticleSystem vfx1;
        [SerializeField] ParticleSystem vfx2;
        public override Tween CustomTween()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(0.25f);

            // Step 1
            
            sequence.Append(images[0].DOFade(0, 0.5f));
            sequence.Join(images[1].DOFade(1, 0.5f));
            sequence.Join(alertTransform.DOScale(1, 0.5f).SetEase(Ease.OutBounce));
            sequence.Join(DOVirtual.DelayedCall(0.05f, () =>
            {
                vfx1.Play();
                cutSceneAudio.PlayClip(alertSound);
            }));
            sequence.AppendInterval(1f); // total time = 0.75s after this step

            // Step 2
            sequence.Append(images[1].DOFade(0, 0.5f));
            sequence.Join(images[2].DOFade(1, 0.5f));
            sequence.Join(DOVirtual.DelayedCall(0.05f, () =>
            {
                alertTransform.gameObject.SetActive(false);
                vfx2.Play();
                cutSceneAudio.PlayClip(VFXSound);
            }));
            sequence.AppendInterval(1f); // wait 1 second

            // Step 3
            sequence.Append(images[2].DOFade(0, 0.5f));
            sequence.Join(images[3].DOFade(1, 0.5f));
            sequence.Join(DOVirtual.DelayedCall(0.05f, () =>
            {
                cutSceneAudio.PlayClip(fallSound);
            }));

            return sequence;
        }
    }
}
