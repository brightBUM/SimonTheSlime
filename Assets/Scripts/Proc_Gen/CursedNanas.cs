using UnityEngine;

public class CursedNanas : MonoBehaviour
{
    [SerializeField] private float effectScale = 1f;
    //public bool runTime;
    //// Start is called before the first frame update
    //ComboGroup comboGroup;
    //void Start()
    //{
    //    comboGroup = GetComponentInParent<ComboGroup>();

    //    if (!runTime) // bananas spawned in runtime by bangeable pt shouldn't increase the target 
    //        LevelManager.Instance.UpdateTargetBananas(1);
    //}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            CollectEffect();
        }
    }
    [ContextMenu("CursedCollecEffect")]
    public void CollectEffect()
    {
        var effect = ObjectPoolManager.Instance.Spawn(5, transform.position, Quaternion.identity);
        effect.transform.localScale = Vector3.one * effectScale;
        LevelManager.Instance.CollectCursedNanas();
        SoundManager.Instance.PlayCursedCurrencyCollectlSFx();

        Destroy(this.gameObject);
    }
}
