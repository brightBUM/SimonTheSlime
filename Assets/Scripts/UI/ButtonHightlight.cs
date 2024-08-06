using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHightlight : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] Transform uiImage;
    public void OnPointerEnter(PointerEventData eventData)
    {
        uiImage.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce);
        SoundManager.instance.PlayStickSFx();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiImage.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutFlash);

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
