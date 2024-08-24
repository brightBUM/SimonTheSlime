using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManger : Singleton<GameManger>
{
    public int selectedIndex;
    public readonly string LOADINGSCENE = "SceneTrans";
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
