using System;
using UnityEngine;

public class SwitchPlatform : MonoBehaviour,IPoundable
{
    [SerializeField] Color unlockedColor;
    [SerializeField] GameObject sparkVFX;
    [SerializeField] DrawGateUnlock DrawGateUnlock;
    SpriteRenderer spriteRenderer;

    bool unlocked = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void OnPlayerPounded(Action<IPoundable> ContinuePound)
    {
        ContinuePound(this);

        if (unlocked)
            return;

        //turn to green color on pound
        sparkVFX.SetActive(false);
        var unlockVFX = ObjectPoolManager.Instance.Spawn(1,transform.position,Quaternion.identity);
        unlockVFX.transform.localScale = Vector3.one*5f;
        spriteRenderer.color = unlockedColor;

        //pan camera to gear turning and door opening
        DrawGateUnlock.TriggerUnlock();


        unlocked = true;
    }

    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
