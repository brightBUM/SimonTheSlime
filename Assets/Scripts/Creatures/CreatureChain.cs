using System.Collections.Generic;
using UnityEngine;

public class CreatureChain : MonoBehaviour
{
    public GameObject segmentPrefab;
    public int segmentCount = 5;
    public float segmentSpacing = 0.5f;

    public List<Transform> segments = new List<Transform>();
    public List<CreatureType> creatureTypes = new List<CreatureType>();
    public SpriteRenderer playerSprite;
    private Vector3 lastPosition;
    private Vector3 baseOffset;
    private Vector3 targetOffset;
    private float modeHoldTime = 0.01f; // min time before switching mode
    private float modeTimer = 0f;
    void Start()
    {
        lastPosition = transform.position;
        baseOffset = playerSprite.flipX ? Vector3.right : Vector3.left;
        targetOffset = baseOffset;
    }

    public void AddToChain(CreatureType creature,Sprite sprite)
    {
        GameObject seg = Instantiate(segmentPrefab, transform.position, Quaternion.identity);
        seg.SetActive(true);
        seg.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        segments.Add(seg.transform);
        creatureTypes.Add(creature);
    }
    public void SpriteSortChain(int value)
    {
        foreach(Transform seg in segments)
        {
            seg.GetComponentInChildren<SpriteRenderer>().sortingOrder = value;
        }
    }
    void Update()
    {
        if (segments.Count == 0) return;

        Vector3 moveDelta = transform.position - lastPosition;
        float xChange = Mathf.Abs(moveDelta.x);
        float yChange = Mathf.Abs(moveDelta.y);

        Vector3 desiredOffset;

        if (yChange > 0.001f && xChange < 0.001f)
            desiredOffset = moveDelta.y < 0f ? Vector3.up : Vector3.down;
        else
            desiredOffset = playerSprite.flipX ? Vector3.right : Vector3.left;

        // Only switch to new offset after it's been consistently desired for modeHoldTime
        if (desiredOffset != targetOffset)
        {
            modeTimer = 0f;
            targetOffset = desiredOffset;
        }
        else
        {
            modeTimer += Time.deltaTime;
        }

        if (modeTimer >= modeHoldTime)
            baseOffset = Vector3.Lerp(baseOffset, targetOffset, Time.deltaTime * 5f);

        lastPosition = transform.position;

        Vector3 targetPos = transform.position + baseOffset * segmentSpacing;
        segments[0].position = Vector3.Lerp(segments[0].position, targetPos, Time.deltaTime * 10f);

        for (int i = 1; i < segments.Count; i++)
        {
            Vector3 followTarget = segments[i - 1].position + baseOffset * segmentSpacing;
            segments[i].position = Vector3.Lerp(segments[i].position, followTarget, Time.deltaTime * 10f);
        }
    }

    
}
