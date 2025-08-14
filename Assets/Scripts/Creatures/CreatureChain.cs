using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureChain : MonoBehaviour
{
    public GameObject segmentPrefab;
    public int segmentCount = 5;
    public float segmentSpacing = 0.5f;

    public List<Transform> segments = new List<Transform>();
    public SpriteRenderer playerSprite;

    void Start()
    {
        //// Instantiate and position the chain behind the player
        //for (int i = 0; i < segmentCount; i++)
        //{
        //    GameObject seg = Instantiate(segmentPrefab, transform.position, Quaternion.identity);
        //    seg.SetActive(true);
        //    segments.Add(seg.transform);
        //}
    }

    public void AddToChain(Sprite sprite)
    {
        GameObject seg = Instantiate(segmentPrefab, transform.position, Quaternion.identity);
        seg.SetActive(true);
        seg.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        segments.Add(seg.transform);
    }

    void Update()
    {
        if(segments.Count>0)
        {
            Vector3 baseOffset = playerSprite.flipX ? Vector3.right : Vector3.left;

            Vector3 targetPos = transform.position + baseOffset * segmentSpacing;

            // First segment follows the player
            segments[0].position = Vector3.Lerp(segments[0].position, targetPos, Time.deltaTime * 10f);

            // Remaining segments follow the one before
            for (int i = 1; i < segments.Count; i++)
            {
                Vector3 followTarget = segments[i - 1].position + baseOffset * segmentSpacing;
                segments[i].position = Vector3.Lerp(segments[i].position, followTarget, Time.deltaTime * 10f);
            }
        }

        
    }
}
