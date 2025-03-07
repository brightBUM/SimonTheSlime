using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionPointerUI : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler
{
    [SerializeField] Transform pointer;
    [SerializeField] int sceneToLoad;
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] GameObject unlockedImage;
    [SerializeField] GameObject lockedImage;
    bool unlocked = false;

    private void Awake()
    {
        unlocked = SaveLoadManager.Instance.GetLevelUnlockData(sceneToLoad - 3); //3 for the scene index
        unlockedImage.SetActive(unlocked);
        lockedImage.SetActive(!unlocked);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(unlocked)
        {
            SoundManager.Instance.PlayPoundSFx();
            sceneLoader.SceneViaLoadingScreen(sceneToLoad);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(unlocked)
        {
            pointer.position = this.transform.position;
            SoundManager.Instance.PlayStickSFx();
        }
    }

   
}
