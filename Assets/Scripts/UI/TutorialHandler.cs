using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEngine;

enum Tutorial
{
    LAUNCH,
    SLAM,
    COMBO,

}
public class TutorialHandler : MonoBehaviour
{
    [SerializeField] Transform ptA;
    [SerializeField] Transform ptB;
    [SerializeField] Transform mouse;
    [SerializeField] Tutorial tutorial;
    [SerializeField] GameObject line;
    [SerializeField] GameObject rightClick;
    private TweenerCore<Vector3, Vector3, VectorOptions> tween;
    int count = 0;
    bool slamPerformed = false;
    // Start is called before the first frame update
    void Start()
    {
        
        switch(tutorial)
        {
            case Tutorial.LAUNCH:
                mouse.position = ptA.position;
                tween = mouse.DOMove(ptB.position, 1f).SetLoops(-1, LoopType.Restart);
                break;
            case Tutorial.SLAM:
                SlamTutorial();
                break;
        }
    }
    public void SlamTutorial()
    {
        mouse.DOMove(ptB.position, 1f).OnComplete(() =>
        {
            mouse.gameObject.SetActive(false);
            line.gameObject.SetActive(false);
            rightClick.gameObject.SetActive(true);
            DOVirtual.DelayedCall(1f, () =>
            {
                rightClick.gameObject.SetActive(false);
                mouse.gameObject.SetActive(true);
                mouse.transform.position = ptA.position;
                line.gameObject.SetActive(true);

                if(!slamPerformed)
                {
                    SlamTutorial();
                }
                else
                {
                    mouse.gameObject.SetActive(false);
                    line.SetActive(false);
                    rightClick.gameObject.SetActive(false);
                }
            });
        });
    }
    // Update is called once per frame
    void Update()
    {
        switch(tutorial)
        {
            case Tutorial.LAUNCH:

                if (Input.GetMouseButtonUp(0) && count < 3)
                {
                    count++;
                    if (count >= 3)
                    {
                        tween.Kill();
                        mouse.gameObject.SetActive(false);
                        line.SetActive(false);
                    }

                }
                break;
            case Tutorial.SLAM:
                if (Input.GetMouseButtonUp(0) && count == 0)
                {
                    count++;
                } 
                else if (Input.GetMouseButtonDown(1) && count == 1)
                {
                    count++;
                    if(count==2)
                    {
                        slamPerformed = true;
                    }
                    
                }

                break;
            case Tutorial.COMBO:
                break;
        }
        
        
    }
}
