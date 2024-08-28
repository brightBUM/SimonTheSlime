using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManger : Singleton<GameManger>
{
    [HideInInspector] public int selectedIndex;
    public readonly string LOADINGSCENE = "SceneTrans";
    // Start is called before the first frame update
    private void Awake()
    {
        SaveLoadManager.Instance.InitFileSystem();
        //Debug.Log("game manager , start");
    }
    void Start()
    {
        
    }
    


    // Update is called once per frame
    void Update()
    {
        
    }

    
}
