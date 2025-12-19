using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureReveal : MonoBehaviour
{
    [SerializeField] Image panelImage;
    [SerializeField] RectTransform titleText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image image;
    [SerializeField] Transform glowEffect;
    [SerializeField] Transform closeButton;
    Vector3 titlePos;
    Vector3 namePos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        titlePos = titleText.anchoredPosition;
        namePos = nameText.rectTransform.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [ContextMenu("Trigger Reveal")]
    public void TriggerNewReveal(CreatureData creatureData)
    {
        //get creature from drawn pool
        image.sprite = creatureData.sprite;
        nameText.text = creatureData.name;

        float alpha = 0.0f;

        DOTween.To(() => alpha, x => alpha = x, 1, 1f).OnUpdate(() =>
        {
            panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, alpha);
            //panelImage.fillAmount = alpha;
        }).OnComplete(() =>
        {
            //glowEffect.DORotate(new Vector3(0, 0, 225), 1f).SetLoops(-1, LoopType.Restart);
            titleText.transform.localScale = Vector3.one;
            titleText.DOAnchorPosY(-158.2f, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
            {
                //nameText.rectTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
                nameText.transform.localScale = Vector3.one;
                nameText.rectTransform.DOAnchorPosY(90.1f, 0.5f).SetEase(Ease.OutExpo);
                image.transform.DOScale(1f, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
                {
                    closeButton.DOScale(1.0f, 0.25f).SetEase(Ease.OutBounce);
                });
                
            });
           
        });
    }
    public void TriggerExistingReveal()
    {

    }
    public void CloseAndReset()
    {
        panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, 0f);
        closeButton.localScale = Vector3.zero;

        titleText.anchoredPosition = titlePos;
        nameText.rectTransform.anchoredPosition = namePos;
        image.transform.localScale = Vector3.zero;

        
    }

}
