using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
    public void ReloadCurrentScreen()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public void LoadAdditiveScene(int index)
    {
        SceneManager.LoadScene(index, LoadSceneMode.Additive);
    }
    public void UnloadScene(int index)
    {
        SceneManager.UnloadSceneAsync(index);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
