using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SaveLoadManager.Instance.InitFileSystem();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
