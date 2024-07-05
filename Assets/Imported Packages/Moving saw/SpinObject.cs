using UnityEngine;

public class SpinObject : MonoBehaviour
{
    #region Variables
    [SerializeField] float speed;
    [SerializeField] Vector3 spinAxis;
    const float multiplier = 100f;
    #endregion

    #region UnityMethods
    void Start()
    {

    }

    void Update()
    {
        transform.Rotate(spinAxis, speed *multiplier* Time.deltaTime);
    }
    #endregion

    #region PublicMethods

    #endregion

    #region PrivateMethods

    #endregion

    #region GameEventListeners

    #endregion
}