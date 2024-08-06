using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionPointerUI : MonoBehaviour,IPointerEnterHandler
{
    [SerializeField] Transform pointer;
    public void OnPointerEnter(PointerEventData eventData)
    {
        pointer.position = this.transform.position;
        SoundManager.instance.PlayStickSFx();
        Debug.Log("entered : " + this.transform.name);
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
