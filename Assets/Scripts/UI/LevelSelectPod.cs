using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelSelectPod : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler
{
    [SerializeField] int sceneToLoad;
    [SerializeField] GameObject unlockedImage;
    [SerializeField] GameObject lockedImage;
    [SerializeField] TextMeshProUGUI levelNumText;
    bool unlocked = false;

    int levelNum;

    public void Init(int num)
    {
        levelNum = num;
        levelNumText.text = "level " + num;

        unlocked = SaveLoadManager.Instance.GetLevelUnlockData(sceneToLoad - 3); //3 for the scene index
        unlockedImage.SetActive(unlocked);
        lockedImage.SetActive(!unlocked);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(unlocked)
        {
            //for mobile touch , have delayed load
#if UNITY_ANDROID && !UNITY_EDITOR
            var pointer = LevelSelectionScreen.Instance.GetPlayerPointer();

            pointer.localScale = Vector3.zero;
            pointer.position = this.transform.position;
            pointer.SetParent(this.transform);
            pointer.DOScale(1, 0.5f).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                SoundManager.Instance.PlayPoundSFx();
                LevelSelectionScreen.Instance.LoadLevel(levelNum+2);
            });
#endif

#if UNITY_EDITOR
            LevelSelectionScreen.Instance.LoadLevel(levelNum+2); // 2 is for the build setting scene index
            SoundManager.Instance.PlayPoundSFx();
#endif
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
#if UNITY_EDITOR
        if (unlocked)
        {
            var pointer = LevelSelectionScreen.Instance.GetPlayerPointer();
            pointer.localScale = Vector3.zero;
            pointer.position = this.transform.position;
            pointer.SetParent(this.transform);
            pointer.DOScale(1, 0.5f).SetEase(Ease.OutElastic);
            SoundManager.Instance.PlayStickSFx();
        }
#endif
    }

   
}
