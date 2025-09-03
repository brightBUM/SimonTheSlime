using UnityEngine;

enum PodState
{
    Buy,        //one time state 
    Upgrade,    //vacant state where you can upgrade pod to inc recovery speed 
    Assigned,   //assigned state - show recovery timer
    Recovered   //recovery complete - tap to init creature animation - new & existing
}
public class RecoveryPod : MonoBehaviour
{
    [SerializeField] GameObject buySetup;
    [SerializeField] GameObject vacantSetup;
    [SerializeField] GameObject AssignedSetup;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
