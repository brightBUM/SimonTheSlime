using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class BangablePlatform : MonoBehaviour, IPoundable
{
    [SerializeField] GameObject bananaPrefab;
    [SerializeField] float coinForce = 5f;
    [SerializeField] int HitCount = 0;
    [SerializeField] SpriteRenderer originalSprite;
    [SerializeField] Color breakColor;
    [SerializeField] TextMeshProUGUI hitCountText;
    [SerializeField] BreakablePlatform breakablePlatform;

    private void Start()
    {
        LevelManager.Instance.UpdateTargetBananas(4);
    }
    public void OnPlayerPounded(System.Action<IPoundable> ContinuePound)
    {
        //spawn coins on hit
        if(HitCount>0)
        {
            GetComponentInChildren<BouncyDeform>().HitDeform();
            SoundManager.Instance.PlayCoinBangSFX();
            var coin = Instantiate(bananaPrefab, transform.position, Quaternion.identity);
            coin.AddComponent<Rigidbody2D>().AddForce(Vector2.up * coinForce, ForceMode2D.Impulse);
            DOVirtual.DelayedCall(0.5f, () =>
            {
                coin.GetComponent<Banana>().CollectEffect();
            });
            HitCount--;
            //UpdateHitCount();
            originalSprite.color = Color.Lerp(originalSprite.color, breakColor, 1 - (float)HitCount / 5.0f);
            ContinuePound(this);
            if(HitCount==1)
            {
                breakablePlatform.SetSize(originalSprite.size);
                Instantiate(breakablePlatform, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }
    }
    private void UpdateHitCount()
    {
        hitCountText.text = HitCount.ToString();
    }

}