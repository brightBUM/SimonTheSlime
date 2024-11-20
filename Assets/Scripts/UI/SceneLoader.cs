
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : Singleton<SceneLoader>
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
        var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex == 9) //to prevent infinite loading screens temporarily
            return;
        SceneViaLoadingScreen(nextSceneIndex);

    }
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void SceneViaLoadingScreen(int index)
    {
        GameManger.Instance.selectedIndex = index;
        SceneManager.LoadScene(GameManger.Instance.LOADINGSCENE);
    }
    public void ReloadCurrentScreen()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public void ReloadCutScene()
    {
        SceneManager.LoadScene(0);
        GameManger.Instance.ReloadIntroDelayed();
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
