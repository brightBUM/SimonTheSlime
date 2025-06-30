using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    [SerializeField] private float effectScale = 3f;
    public bool runTime;
    // Start is called before the first frame update
    ComboGroup comboGroup;
    void Start()
    {
        comboGroup = GetComponentInParent<ComboGroup>();

        if(!runTime) // bananas spawned in runtime by bangeable pt shouldn't increase the target 
            LevelManager.Instance.UpdateTargetBananas(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            CollectEffect();
        }
    }

    public void CollectEffect()
    {
        var effect = ObjectPoolManager.Instance.Spawn(2, transform.position, Quaternion.identity);
        effect.transform.localScale = Vector3.one * effectScale;
        //SoundManager.instance.PlayCollectibleSFx();
        LevelManager.Instance.CollectBanana();
        SoundManager.Instance.PlayCollectibleSFx();

        if(comboGroup!=null && comboGroup.enabled)
        {
            comboGroup.Collected.Invoke();
        }

        Destroy(this.gameObject);
    }


}
