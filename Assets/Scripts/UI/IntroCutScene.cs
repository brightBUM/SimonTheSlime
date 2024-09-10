using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CutScene
{
    public class IntroCutScene : MonoBehaviour
    {
        [SerializeField] List<Scene> scenes;
        [SerializeField] Sprite splashScreen;
        [SerializeField] Image mainScreen;
        [SerializeField] Transform camTransform;
        [SerializeField] Ease sceneTrans;
        [SerializeField] float sceneTransDuration = 0.5f;
        int currentTween = 0;

        // Start is called before the first frame update
        void Start()
        {
            //check for save file 


            StartCoroutine(StartCutScene());
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                StopAllCoroutines();
                DOTween.KillAll();

                //load to main menu

            }
        }
        IEnumerator StartCutScene()
        {
            //loop through all scenes 
            for(int i=0;i<scenes.Count;i++)
            {
                //scenes[i].gameObject.SetActive(true);
                yield return StartCoroutine(ProcessScene(i));

                Debug.Log(string.Format($"scene {i+1} complete"));

                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

                //move to next slide
                if(i!=scenes.Count-1)
                {
                    var newPos = scenes[i + 1].transform.position;
                    newPos.z = -10;
                    var tween = camTransform.DOMove(newPos, sceneTransDuration).SetEase(sceneTrans);
                    yield return tween.WaitForCompletion();
                }
                
            }
            Debug.Log("all scenes complete");

            //load main menu
        }

        IEnumerator ProcessScene(int index)
        {
            var tweenObjects = scenes[index].tweenObjects;
            int tweensCount = tweenObjects.Count;
            Debug.Log("scene tween count -" + tweensCount);
            currentTween = 0;

            while(tweensCount!=currentTween)
            {
                var tweenObject = tweenObjects[currentTween];
                yield return StartCoroutine(ProcessTweens(tweenObject));
                //Debug.Log(string.Format($"cur - {currentTween} , total - {tweensCount}"));
            }
            yield return null;
        }

        IEnumerator ProcessTweens(TweenObject tweenObject)
        {
            switch (tweenObject.tweenType)
            {
                case TweenType.MOVING:
                    var tween = tweenObject.transform.DOLocalMove(tweenObject.endValue, tweenObject.duration).SetEase(tweenObject.easeType);
                    yield return tween.WaitForCompletion();
                    currentTween++;
                    break;
                case TweenType.SCALING:
                    var tween2 = tweenObject.transform.DOScale(tweenObject.endValue,tweenObject.duration).SetEase(tweenObject.easeType);
                    yield return tween2.WaitForCompletion();
                    currentTween++;
                    break;

                default:
                    break;
            }
        }
        
    }

    public enum TweenType
    {
        MOVING,
        SCALING,
    }
}
