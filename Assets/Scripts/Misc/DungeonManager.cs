using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] Transform followDummy;
    [SerializeField] Transform parallaxParent;
    [SerializeField] GameObject spotLightPrefab;
    public ParallaxLooper[] parallaxLoopers;
    public static DungeonManager Instance;
    GameObject spotLightObject;
    private void Awake()
    {
        Instance = this;

        //parallaxParent.transform.position += Vector3.right * 2000f;
        //followDummy.transform.position += Vector3.right * 2000f;
        //ToggleLevelParallaxLayers(followDummy.transform);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SpawnSpotLight(Transform player)
    {
        spotLightObject = Instantiate(spotLightPrefab);
        spotLightObject.transform.parent = player;
        spotLightObject.transform.localPosition = Vector3.zero;
        spotLightObject.SetActive(true);
    }

    public void RemoveSpotlight()
    {
        spotLightObject.transform.parent = null;
        Destroy(spotLightObject);
    }
    public void ToggleLevelParallaxLayers(Transform target)
    {
        foreach (var layer in parallaxLoopers)
        {
            layer.SetTarget(target);
        }
    }
    public void SetParallaxPositions(Transform target)
    {
        foreach (var layer in parallaxLoopers)
        {
            layer.gameObject.SetActive(true);
            layer.transform.position = target.position;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
