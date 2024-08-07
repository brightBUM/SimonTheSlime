using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoreSplatter : MonoBehaviour
{
    [SerializeField] GameObject poundMask;
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
            var poundSprite = Instantiate(poundMask, collisionEvents[i].intersection, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));
            poundSprite.GetComponent<SpriteRenderer>().sprite = poundSprites[UnityEngine.Random.Range(0, poundSprites.Length)];
            Destroy(poundSprite,UnityEngine.Random.Range(0.5f,2f));
        }
    }
}
