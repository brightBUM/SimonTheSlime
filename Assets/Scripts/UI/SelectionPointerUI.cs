using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionPointerUI : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler
{
    [SerializeField] Transform pointer;
    [SerializeField] int sceneToLoad;
    [SerializeField] SceneLoader sceneLoader;

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlayPoundSFx();
        sceneLoader.SceneViaLoadingScreen(sceneToLoad);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointer.position = this.transform.position;
        SoundManager.Instance.PlayStickSFx();
        //Debug.Log("entered : " + this.transform.name);
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
