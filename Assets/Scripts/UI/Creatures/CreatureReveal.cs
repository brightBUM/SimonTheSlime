using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureReveal : MonoBehaviour
{
    [SerializeField] Image panelImage;
    [SerializeField] Color[] typeColors;
    [SerializeField] Color newColorText;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image image;
    [SerializeField] Transform imageParent;
    [SerializeField] Transform glowImage;
    [SerializeField] GameObject sparkles;
    [SerializeField] Transform closeButton;
    Vector3 titlePos;
    Vector3 namePos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        titlePos = titleText.rectTransform.anchoredPosition;
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

        titleText.text = "New Creature \nRescued";
        titleText.color = newColorText;

        panelImage.color = typeColors[(int)creatureData.creatureType];
        sparkles.SetActive(true);

        float alpha = 0.0f;

        DOTween.To(() => alpha, x => alpha = x, 1, 1f).OnUpdate(() =>
        {
            panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, alpha);
            //panelImage.fillAmount = alpha;
        }).OnComplete(() =>
        {
            SoundManager.Instance.PlayNewCreatureReveal();
            //glowEffect.DORotate(new Vector3(0, 0, 225), 1f).SetLoops(-1, LoopType.Restart);
            titleText.transform.localScale = Vector3.one;
            titleText.rectTransform.DOAnchorPosY(-158.2f, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
            {
                //nameText.rectTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
                nameText.transform.localScale = Vector3.one;
                nameText.rectTransform.DOAnchorPosY(90.1f, 0.5f).SetEase(Ease.OutExpo);
                imageParent.transform.DOScale(1f, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
                {
                    glowImage.DOLocalRotate(new Vector3(0, 0, 360), 10f, RotateMode.FastBeyond360)
                                           .SetEase(Ease.Linear)
                                           .SetLoops(-1, LoopType.Restart);
                    closeButton.DOScale(1.0f, 0.25f).SetEase(Ease.OutBounce);
                });
                
            });
           
        });
    }
    public void TriggerExistingReveal(CreatureData creatureData)
    {
        image.sprite = creatureData.sprite;
        nameText.text = creatureData.name;

        titleText.text = "Creature Rescued";
        titleText.color = Color.white;

        panelImage.color = typeColors[(int)creatureData.creatureType];


        float alpha = 0.0f;

        DOTween.To(() => alpha, x => alpha = x, 1, 1f).OnUpdate(() =>
        {
            panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, alpha);
            //panelImage.fillAmount = alpha;
        }).OnComplete(() =>
        {

            //glowEffect.DORotate(new Vector3(0, 0, 225), 1f).SetLoops(-1, LoopType.Restart);
            titleText.transform.localScale = Vector3.one;
            titleText.rectTransform.DOAnchorPosY(-158.2f, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
            {
                //nameText.rectTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
                nameText.transform.localScale = Vector3.one;
                nameText.rectTransform.DOAnchorPosY(90.1f, 0.5f).SetEase(Ease.OutExpo);
                imageParent.transform.DOScale(1f, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
                {
                    SoundManager.Instance.PlayExistingCreatureReveal();
                    closeButton.DOScale(1.0f, 0.25f).SetEase(Ease.OutBounce);
                    glowImage.DOLocalRotate(new Vector3(0, 0, 360),10f,RotateMode.FastBeyond360)
                                            .SetEase(Ease.Linear)
                                            .SetLoops(-1, LoopType.Restart);
                });

            });

        });
    }
    public void CloseAndReset()
    {
        panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, 0f);
        closeButton.localScale = Vector3.zero;

        titleText.rectTransform.anchoredPosition = titlePos;
        nameText.rectTransform.anchoredPosition = namePos;
        imageParent.transform.localScale = Vector3.zero;
        DOTween.Kill(glowImage);
        sparkles.SetActive(false);
    }

}
