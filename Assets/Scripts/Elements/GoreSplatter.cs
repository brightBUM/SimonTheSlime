using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoreSplatter : MonoBehaviour
{
    [SerializeField] Sprite[] poundSprites;
    [SerializeField] ParticleSystem particles;
    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        //UnityEngine.Debug.Log("object name : " + other.name);

        ParticlePhysicsExtensions.GetCollisionEvents(particles, other, collisionEvents);
        int count = collisionEvents.Count;
        for (int i = 0; i < count; i++)
        {
            var pos = collisionEvents[i].intersection;
            pos.z = -1.5f;
            var splatterObject = ObjectPoolManager.Instance.Spawn(0, pos, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));
            var splatterSpriteRenderer = splatterObject.GetComponent<SpriteRenderer>();
            splatterSpriteRenderer.sprite = poundSprites[UnityEngine.Random.Range(0, poundSprites.Length)];
            splatterSpriteRenderer.color = new Color(splatterSpriteRenderer.color.r, splatterSpriteRenderer.color.g, splatterSpriteRenderer.color.b, 1f);

            //float alpha = 1.0f;
            //DOTween.To(() => alpha, x => alpha = x, 0, 1f).OnUpdate(() =>
            //{
            //    splatterSpriteRenderer.color = new Color(splatterSpriteRenderer.color.r, splatterSpriteRenderer.color.g, splatterSpriteRenderer.color.b, alpha);

            //}).OnComplete(() =>
            //{
            //    splatterObject.SetActive(false);
            //});

            ObjectPoolManager.Instance.Despawn(splatterObject, 1f);

            
        }
    }
}
