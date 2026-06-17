using DG.Tweening;
using Magar;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReelTween : MonoBehaviour
{
    [SerializeField] Transform mainSetup;
    [SerializeField] Image charImage;
    [SerializeField] Sprite startSprite;
    [SerializeField] Sprite tiltedSprite;
    [SerializeField] Sprite spookedSprite;
    [SerializeField] Transform commonImg;
    [SerializeField] Transform rareImg;
    [SerializeField] Transform epicImg;
    [SerializeField] float commonAnchorPosX;
    [SerializeField] float rareAnchorPosX;
    [SerializeField] float epicAnchorPosX;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip whooshClip;
    [SerializeField] AudioClip slimeClip;
    [SerializeField] AudioClip swooshinClip;
    [SerializeField] AudioClip crashClip;
    [SerializeField] RectTransform[] textTweens;
    [SerializeField] AudioClip[] clips;
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //TriggerSequence();
        animator = GetComponent<Animator>();
    }

    private void TriggerSequence()
    {
        animator.enabled = false;
        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(0.5f);

        //1
        seq.AppendCallback(() =>
        {
            charImage.sprite = tiltedSprite;
            audioSource.clip = whooshClip;
            audioSource.Play();
        });

        seq.AppendInterval(0.25f);
        seq.AppendCallback(() =>
        {
            ScaleandMoveX(seq, commonImg,charImage.transform.position.x, commonAnchorPosX);
        });

        //2
        seq.AppendInterval(1f);

        seq.AppendCallback(() =>
        {
            commonImg.localScale = Vector3.zero;
            mainSetup.localScale = new Vector3(-1,1,1);
            audioSource.Play();

            ScaleandMoveX(seq, commonImg, charImage.transform.position.x, commonAnchorPosX);
            
        });

        seq.AppendInterval(0.25f);
        seq.AppendCallback(() =>
        {
            ScaleandMoveX(seq, rareImg, commonImg.transform.position.x, rareAnchorPosX);
        });

        //3
        seq.AppendInterval(1f);

        seq.AppendCallback(() =>
        {
            rareImg.localScale = Vector3.zero;
            commonImg.localScale = Vector3.zero;
            mainSetup.localScale = Vector3.one;
            audioSource.Play();

            ScaleandMoveX(seq, commonImg, charImage.transform.position.x, commonAnchorPosX);

        });

        seq.AppendInterval(0.25f);
        seq.AppendCallback(() =>
        {
            ScaleandMoveX(seq, rareImg, commonImg.transform.position.x, rareAnchorPosX);
        });

        seq.AppendInterval(0.25f);
        seq.AppendCallback(() =>
        {
            ScaleandMoveX(seq, epicImg, rareImg.transform.position.x, epicAnchorPosX);
        });

        //4
        seq.AppendInterval(1f);

        seq.AppendCallback(() =>
        {
            rareImg.localScale = Vector3.zero;
            commonImg.localScale = Vector3.zero;
            epicImg.localScale = Vector3.zero;
            mainSetup.localScale = new Vector3(-1, 1, 1);
            audioSource.Play();

            ScaleandMoveX(seq, commonImg, charImage.transform.position.x, commonAnchorPosX);

        });

        seq.AppendInterval(0.25f);
        seq.AppendCallback(() =>
        {
            ScaleandMoveX(seq, rareImg, commonImg.transform.position.x, rareAnchorPosX);
        });

        seq.AppendInterval(0.25f);
        seq.AppendCallback(() =>
        {
            ScaleandMoveX(seq, epicImg, rareImg.transform.position.x, epicAnchorPosX);
        });

        //5
        seq.AppendInterval(0.25f);
        seq.AppendCallback(() =>
        {
            epicImg.DOMoveX(rareImg.transform.position.x, 0.25f);
            epicImg.DOScale(Vector3.zero, 0.25f);
        });
        
        seq.AppendInterval(0.15f);
        seq.AppendCallback(() =>
        {
            rareImg.DOMoveX(commonImg.transform.position.x, 0.25f);
            rareImg.DOScale(Vector3.zero, 0.25f);
        });
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() =>
        {
            commonImg.DOMoveX(charImage.transform.position.x, 0.25f);
            commonImg.DOScale(Vector3.zero, 0.25f);
        });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            charImage.sprite = startSprite;

        });
        seq.AppendInterval(1f);
        seq.AppendCallback(() =>
        {
            charImage.sprite = spookedSprite;
            audioSource.clip = slimeClip;
            audioSource.Play();
        });
        //6

        seq.AppendInterval(1f);
        seq.AppendCallback(() =>
        {
            mainSetup.localScale = new Vector3(1, 1, 1);
            charImage.sprite = startSprite;
           
        });

        //7
        seq.AppendInterval(1f);
        seq.AppendCallback(() =>
        {
            animator.enabled = true;
            animator.SetTrigger("out");
        });

    }
    private void ScaleandMoveX(Sequence seq,Transform item,float currentXpos,float targetXpos)
    {
        // Prepare initial scale
        item.position = new Vector3(currentXpos,item.position.y,item.position.z);
        item.localScale = Vector3.zero;

        seq.Join(
            item.DOLocalMoveX(targetXpos, 0.5f)
        );

        seq.Join(
            item.DOScale(Vector3.one, 0.5f)
        );
    }
    AudioSource activeSource;
    public void TitleTextTween()
    {
        var seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
           PlayClip(clips[0]);
            textTweens[0].DOAnchorPosX(0, 0.5f);
        });
        seq.AppendInterval(0.25f);
        seq.AppendCallback(() =>
        {
            PlayClip(clips[1]);
            textTweens[1].DOAnchorPosX(-60, 0.5f);
        });
        seq.AppendInterval(0.25f);
        seq.AppendCallback(() =>
        {
            PlayClip(clips[2]);
            textTweens[2].DOAnchorPosX(0, 0.5f);
        });
        seq.AppendInterval(0.25f);
        seq.AppendCallback(() =>
        {
            PlayClip(clips[0]);
            textTweens[3].DOAnchorPosY(-1200, 0.5f);
        });

    }

    public void PlaySwingIn()
    {
        audioSource.clip = swooshinClip;
        audioSource.Play();
    }

    public void PlayCrashedIn()
    { 
        audioSource.clip = crashClip;
        audioSource.Play();
    }
    private void PlayClip(AudioClip clip)
    {
        activeSource = GetIdleSource();
        activeSource.clip = clip;
        activeSource.Play();

        ObjectPoolManager.Instance.Despawn(activeSource.gameObject, activeSource.clip.length);
    }
    
    private AudioSource GetIdleSource()
    {
        //returns the audioSource that isnt playing
        var audioGameObject = ObjectPoolManager.Instance.Spawn(0, Vector3.zero, Quaternion.identity);
        return audioGameObject.GetComponent<AudioSource>();
    }
}
