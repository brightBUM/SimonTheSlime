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
    [SerializeField] GameObject[] stars;
    bool unlocked = false;

    int levelNum;

    public void Init(int num)
    {
        levelNum = num;
        var podNum = num + 1;
        levelNumText.text = "level " + podNum;

        unlocked = levelNum <= SaveLoadManager.Instance.GetLevelUnlockData(); 
        unlockedImage.SetActive(unlocked);
        lockedImage.SetActive(!unlocked);

        if(unlocked)
        {
            var collectedStar = SaveLoadManager.Instance.GetLevelStarData(levelNum);
            for (int i = 0; i < collectedStar; i++)
            {
                stars[i].SetActive(true);
            }
        }
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
            LevelSelectionScreen.Instance.LoadLevel(levelNum+3); // 3 is for the build setting scene index
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
