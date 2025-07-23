using DG.Tweening;
using TMPro;

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
}