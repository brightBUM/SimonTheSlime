using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static bool shuttingDown = false;

    public static T Instance { 

        get 
        {
            if(shuttingDown)
            {
                Debug.Log($"Singleton {typeof(T)} already destroyed ,returning null");
                return null;
            }

            if( instance == null )
            {
                instance = (T)FindAnyObjectByType(typeof(T));

                if(instance == null )
                {
                    var singletonObject = new GameObject();
                    singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + "_Singleton";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
        
    }

    private void OnApplicationQuit()
    {
        shuttingDown = true;
    }
    private void OnDestroy()
    {
        shuttingDown = true;   
    }
}
