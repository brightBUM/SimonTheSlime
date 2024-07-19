using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    [SerializeField] GameObject collectionVFX;
    [SerializeField] private float effectScale = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            var effect = Instantiate(collectionVFX, transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * effectScale;
            //SoundManager.instance.PlayCollectibleSFx();
            LevelManager.Instance.CollectBanana();
            Destroy(this.gameObject);
        }
    }


}
