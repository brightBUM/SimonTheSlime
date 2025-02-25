using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WatchAdRewardUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image bananaIcon;
    [SerializeField] GameObject AdImage;
    [SerializeField] GameObject lockedImage;
    [SerializeField] int remainTime = 18;
    const string rewardText = "50 Nanas";
    public bool rewardReady; /*{ get; set; }*/

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        if(rewardReady)
        {
            //trigger flashing tween
            text.text = rewardText;
            text.transform.DOScale(1.1f, 0.3f).SetLoops(-1, LoopType.Yoyo);
            
        }
        else
        {
            GetUnlockProgress();
        }
        ToggleUnlockImages();
    }
    // Update is called once per frame
    void Update()
    {
       
    }

    private void ToggleUnlockImages()
    {
        lockedImage.SetActive(!rewardReady);
        AdImage.SetActive(rewardReady);
    }

    public void GetUnlockProgress()
    {

        //gets time remain from Last unlock 
        text.text = remainTime+"H";
        //show that as fill amount for banana icon
        bananaIcon.fillAmount = (float)(24-remainTime) / (float)24;
    }

    private void OnDisable()
    {
        if(DOTween.IsTweening(text.transform))
        {
            DOTween.Kill(text.transform);
            text.transform.localScale = Vector3.one;
        }
    }
}
