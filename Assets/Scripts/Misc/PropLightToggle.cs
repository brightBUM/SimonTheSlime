using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PropLightToggle : MonoBehaviour
{
    [SerializeField] GameObject lightObject;
    [SerializeField] float frequency = 0.1f;
    [SerializeField] float randomness = 0.05f;      // Max random offset added to frequency
    [SerializeField] int triggerFlickerCount = 6;   // How many times to flicker on trigger
    [SerializeField] bool defaultState = true;       // The resting on/off state for trigger variation
    [SerializeField] bool onAwake;
    [SerializeField] bool onTrigger;

    public static event System.Action OnSlamEvent;  // Fire this from your slam logic

    void Start()
    {
        if (onAwake)
            StartCoroutine(ToggleOnLoop());

        if (onTrigger)
        {
            lightObject.SetActive(defaultState);
            OnSlamEvent += HandleSlam;
        }
    }

    void OnDestroy()
    {
        if (onTrigger)
            OnSlamEvent -= HandleSlam;
    }

    // Call this from anywhere to fire the slam flicker
    // e.g. LightFlicker.OnSlamEvent?.Invoke();
    public static void TriggerSlam() => OnSlamEvent?.Invoke();

    void HandleSlam() => StartCoroutine(ToggleOnTrigger());

    IEnumerator ToggleOnLoop()
    {
        while (true)
        {
            lightObject.SetActive(!lightObject.activeSelf);
            float randomOffset = Random.Range(-randomness, randomness);
            yield return new WaitForSeconds(Mathf.Max(0.01f, frequency + randomOffset));
        }
    }

    IEnumerator ToggleOnTrigger()
    {
        for (int i = 0; i < triggerFlickerCount; i++)
        {
            lightObject.SetActive(!lightObject.activeSelf);
            float randomOffset = Random.Range(-randomness, randomness);
            yield return new WaitForSeconds(Mathf.Max(0.01f, frequency + randomOffset));
        }

        // Restore resting state when done flickering
        lightObject.SetActive(defaultState);
    }
}