using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CutScene
{
    public class IntroCutScene : MonoBehaviour
    {
        [SerializeField] List<Scene> scenes;
        [SerializeField] GameObject splashScreen;
        [SerializeField] GameObject privacyPanel;
        [SerializeField] Transform camTransform;
        [SerializeField] Ease sceneTrans;
        [SerializeField] CutSceneAudio cutSceneAudio;
        [SerializeField] float sceneTransDuration = 0.5f;
        [SerializeField] float splashScreenDelay = 1f;
        [SerializeField] Button skipButton;
        int currentTween = 0;
        bool next = false;
      
        // Start is called before the first frame update
        void Awake()
        {
            //check for save file 
            SaveLoadManager.Instance.skipCutScene += CheckForSaveLoad;

        }
        private void OnEnable()
        {
            skipButton.onClick.AddListener(SkipEscape);
        }
        private void LoadMainMenu()
        {
            SceneLoader.Instance.LoadNextScene();
            GameManger.Instance.ToggleMenuMusic(true);
        }
        public void CheckForSaveLoad(bool skip)
        {
            if(skip)
            {
                // goto main menu
                DOVirtual.DelayedCall(1f,() =>
                {
                    LoadMainMenu();
                });
            }
            else
            {
                // privacy policy will be enabled for first time
                privacyPanel.SetActive(true);

            }
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                SkipEscape();
            }
        }
        public void PrivacyAgreeTrigger()
        {
            //show cutscene
            DOVirtual.DelayedCall(splashScreenDelay, () =>
            {
                splashScreen.SetActive(false);
                StartCoroutine(StartCutScene());
            });
        }
        private void SkipEscape()
        {
            StopAllCoroutines();
            DOTween.KillAll();

            //load to main menu
            LoadMainMenu();
        }
        IEnumerator StartCutScene()
        {
            //loop through all scenes 
            for(int i=0;i<scenes.Count;i++)
            {
                //scenes[i].gameObject.SetActive(true);
                yield return StartCoroutine(ProcessScene(i));

                //Debug.Log(string.Format($"scene {i+1} complete"));

                yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Space)) || Input.GetMouseButtonDown(0));

                //move to next slide
                if(i!=scenes.Count-1)
                {
                    cutSceneAudio.PlaySwooshSFX();
                    var newPos = scenes[i + 1].transform.position;
                    newPos.z = -10;
                    var tween = camTransform.DOMove(newPos, sceneTransDuration).SetEase(sceneTrans);
                    yield return tween.WaitForCompletion();

                    if (scenes[i].nextSceneSound != null)
                        cutSceneAudio.PlayClip(scenes[i].nextSceneSound);
                }

            }
            Debug.Log("all scenes complete");

            //load main menu
            yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Space)) || Input.GetMouseButtonDown(0));
            LoadMainMenu();
        }

        IEnumerator ProcessScene(int index)
        {
            var tweenObjects = scenes[index].tweenObjects;
            int tweensCount = tweenObjects.Count;
            //Debug.Log("scene tween count -" + tweensCount);
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
                    tweenObject.transform.localScale = Vector3.one;
                    var tween = tweenObject.transform.DOLocalMove(tweenObject.endValue, tweenObject.duration).SetEase(tweenObject.easeType);
                    cutSceneAudio.PlayPopSounds();
                    yield return tween.WaitForCompletion();
                    currentTween++;
                    break;
                case TweenType.SCALING:
                    var tween2 = tweenObject.transform.DOScale(tweenObject.endValue,tweenObject.duration).SetEase(tweenObject.easeType);
                    cutSceneAudio.PlayPopSounds();
                    yield return tween2.WaitForCompletion();
                    currentTween++;
                    break;

                default:
                    break;
            }
        }

        public void PrivacyPolicyLink()
        {
            GameManger.Instance.PrivacyPolicy();
        }
        public void TermsLink()
        {
           GameManger.Instance.TermsAndConditions();
        }
        private void OnDisable()
        {
            SaveLoadManager.Instance.skipCutScene -= CheckForSaveLoad;
            skipButton.onClick.RemoveListener(SkipEscape);

        }

    }
    

    public enum TweenType
    {
        MOVING,
        SCALING,
    }
}
