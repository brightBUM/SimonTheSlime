using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOrb : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    Color iconColor;
    // Start is called before the first frame update
    void Start()
    {
        iconColor = spriteRenderer.color;
        DOTween.To(() => iconColor.a, x => iconColor.a = x, 0, 0.5f).SetLoops(-1).SetEase(Ease.OutSine).OnUpdate(() =>
        {
            spriteRenderer.color = iconColor;
        });
    }

   
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            Debug.Log("player entered time orb");
            //refill bullet time consumable
            playerController.RefillBulletTime();
            //play audio and destroy
            Destroy(this.gameObject);
        }
    }
}
