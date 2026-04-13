using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Scene Names")]
    public string mainSceneName = "Main";
    public string loadingSceneName = "SceneTrans drop";
    public string secretRoomSceneName = "Proc_Gen";

    [Header("Offsets")]
    public Vector3 loadingSceneOffset = new Vector3(10000f, 0f, 0f);
    public Vector3 secretRoomOffset = new Vector3(20000f, 0f, 0f);

    [Header("Main Scene References")]
    public Camera mainCam;
    public Transform playerTransform;
    public MonoBehaviour playerController;
    public CinemachineVirtualCamera mainVirtualCam;

    [Header("Camera Blending")]
    public float blendDuration = 1f;

    float progress;
    private CinemachineVirtualCamera loadingVirtualCam;
    private CinemachineVirtualCamera secretRoomVirtualCam;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SetBrainBlendDuration(blendDuration);
    }

    // ── Called when player breaks the platform ───────────────────────────────
    public void TriggerSecretRoomTransition()
    {
        StartCoroutine(LoadSecretRoom());
    }

    IEnumerator LoadSecretRoom()
    {
        // 1. Freeze player — keep camera ALIVE, brain must stay active
        //playerController.enabled = false;
        GamePlayScreenUI.Instance.ToggleGameplayScreen(false);

        // Freeze main scene parallax — null target so layers stop moving
        LevelManager.Instance.ToggleLevelParallaxLayers(null);

        // Ensure main vcam is active before blending away
        BlendToCamera(mainVirtualCam, cut: true);

        //Debug.Break();
        // 2. Load loading screen, offset, cut to its camera
        yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
        loadingVirtualCam = FindVirtualCamInScene(loadingSceneName);
        BlendToCamera(loadingVirtualCam, cut: true);
        OffsetSceneObjects(loadingSceneName, loadingSceneOffset);

        // 3. Begin loading secret room without activating it yet
        AsyncOperation secretLoad = SceneManager.LoadSceneAsync(secretRoomSceneName, LoadSceneMode.Additive);
        secretLoad.allowSceneActivation = false;

        float elapsed = 0f;
        float minLoadTime = 1f;

        // Wait until BOTH: 1 second passed AND scene data ready
        while (elapsed < minLoadTime || secretLoad.progress < 0.9f)
        {
            elapsed += Time.deltaTime;
            progress = secretLoad.progress;
            yield return null;
        }

        // 4. Activate scene — Awake/Start run on secret room objects
        secretLoad.allowSceneActivation = true;
        while (!secretLoad.isDone)
            yield return null;

        //Debug.Break();

        // 5. Generate chunks — wait for full completion before proceeding
        bool generateDone = false;

        var chunkGenerator = FindAnyObjectByType<ChunkGenerator>();
        if (chunkGenerator != null)
        {
            chunkGenerator.Generate(() =>
            {
                // Offset AFTER generation so chunks spawn at local origin first
                OffsetSceneObjects(secretRoomSceneName, secretRoomOffset);
                secretRoomVirtualCam = FindVirtualCamInScene(secretRoomSceneName);

                //DungeonManager.Instance.SetParallaxPositions(secretRoomVirtualCam.transform);
                DungeonManager.Instance.ToggleLevelParallaxLayers(Camera.main.transform);

                generateDone = true;
            });
        }
        else
        {
            Debug.LogError("ChunkGenerator not found in scene");
            generateDone = true;
        }

        while (!generateDone)
            yield return null;

        

        // 6. Cut to secret room camera
        BlendToCamera(secretRoomVirtualCam, cut: true);
        yield return new WaitForSeconds(0.1f);

        // 7. Unload loading screen
        yield return SceneManager.UnloadSceneAsync(loadingSceneName);
        loadingVirtualCam = null;

        // 8. Unfreeze player
        //playerController.enabled = true;
        GamePlayScreenUI.Instance.ToggleGameplayScreen(true);

        Debug.Log("Transition to secret room complete");
    }

    // ── Called when player exits the secret room ─────────────────────────────
    public void ReturnToMainScene()
    {
        StartCoroutine(UnloadSecretRoom());
    }

    IEnumerator UnloadSecretRoom()
    {
        playerController.enabled = false;
        GamePlayScreenUI.Instance.ToggleGameplayScreen(false);

        // Freeze dungeon parallax
        //SetParallaxTarget(secretRoomSceneName, null);

        // Load loading screen for reverse transition
        yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
        OffsetSceneObjects(loadingSceneName, loadingSceneOffset);
        loadingVirtualCam = FindVirtualCamInScene(loadingSceneName);
        BlendToCamera(loadingVirtualCam, cut: true);

        // Minimum display time for loading screen on return
        float elapsed = 0f;
        float minLoadTime = 1f;
        while (elapsed < minLoadTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Unload secret room while loading screen is visible
        yield return SceneManager.UnloadSceneAsync(secretRoomSceneName);
        secretRoomVirtualCam = null;

        // TODO: teleport player back to main scene return spawn
        // playerTransform.position = mainReturnSpawn.position;

        // Restore main scene parallax — player is back in main scene world space
        //SetParallaxTarget(mainSceneName, playerTransform);
        //SetParallaxActive(mainSceneName, true);

        // Cut back to main cam
        BlendToCamera(mainVirtualCam, cut: true);
        yield return new WaitForSeconds(0.1f);

        // Unload loading screen
        yield return SceneManager.UnloadSceneAsync(loadingSceneName);
        loadingVirtualCam = null;

        playerController.enabled = true;
        GamePlayScreenUI.Instance.ToggleGameplayScreen(true);

        Debug.Log("Transition back to main scene complete");
    }

    

    // ── Camera helpers ────────────────────────────────────────────────────────
    void BlendToCamera(CinemachineVirtualCamera targetCam, bool cut = false)
    {
        if (targetCam == null)
        {
            Debug.LogWarning("BlendToCamera: targetCam is null");
            return;
        }

        var brain = mainCam.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            brain.m_DefaultBlend = new CinemachineBlendDefinition(
                cut ? CinemachineBlendDefinition.Style.Cut
                    : CinemachineBlendDefinition.Style.EaseInOut,
                cut ? 0f : blendDuration
            );
        }

        if (mainVirtualCam != null) mainVirtualCam.Priority = 0;
        if (loadingVirtualCam != null) loadingVirtualCam.Priority = 0;
        if (secretRoomVirtualCam != null) secretRoomVirtualCam.Priority = 0;

        targetCam.Priority = 10;
    }

    void SetBrainBlendDuration(float duration)
    {
        if (mainCam == null) return;
        var brain = mainCam.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            brain.m_DefaultBlend = new CinemachineBlendDefinition(
                CinemachineBlendDefinition.Style.EaseInOut, duration);
        }
    }

    CinemachineVirtualCamera FindVirtualCamInScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            var vcam = root.GetComponentInChildren<CinemachineVirtualCamera>();
            if (vcam != null) return vcam;
        }
        Debug.LogWarning($"No CinemachineVirtualCamera found in scene: {sceneName}");
        return null;
    }

    // ── Scene offset ──────────────────────────────────────────────────────────
    void OffsetSceneObjects(string sceneName, Vector3 offset)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            // Disable cameras in other scenes — main scene brain drives everything
            if (root.GetComponent<Camera>() != null)
            {
                root.GetComponent<Camera>().enabled = false;
                var brain = root.GetComponent<CinemachineBrain>();
                if (brain != null) brain.enabled = false;
                continue;
            }
            if (root.GetComponent<CinemachineVirtualCamera>() != null) continue;
            if (root.GetComponent<CinemachineBrain>() != null) continue;

            root.transform.position += offset;
        }
    }
}