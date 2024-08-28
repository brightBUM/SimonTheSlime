using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHightlight : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] Transform uiImage;
    public void OnPointerEnter(PointerEventData eventData)
    {
        uiImage.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutElastic).SetUpdate(true);
        SoundManager.instance.PlayStickSFx();
        GameManger.Instance.SwapCursor(true);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(DOTween.IsTweening(uiImage))
        {
            DOTween.Kill(uiImage);
        }
        uiImage.DOScale(Vector3.zero, 0.1f).SetEase(Ease.OutFlash).SetUpdate(true);
        GameManger.Instance.SwapCursor(false);

    }

    public void PlayStartGameSFX()
    {
        SoundManager.instance.PlayPoundSFx();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
