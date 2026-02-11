using System.Collections;
using UnityEngine;

public class TutorialPlayback : MonoBehaviour
{
    public TutorialData data;
    public PlayerInput ghostInput;
    public PlayerInput originalInput;

    float timer;
    int index;
    bool playbackFinished;
    private void Start()
    {
        ghostInput.gameObject.SetActive(true);
        originalInput.enabled = false;

        PlayInput(data.inputs[index]);
        index++;
    }
    void Update()
    {
        if (playbackFinished) return;

        timer += Time.deltaTime;

        while (index < data.inputs.Count && data.inputs[index].time <= timer)
        {
            PlayInput(data.inputs[index]);
            index++;
        }

        if (index >= data.inputs.Count)
        {
            playbackFinished = true;
            StartCoroutine(OnPlaybackFinished(1f));
        }
    }
    IEnumerator OnPlaybackFinished(float time)
    {
        yield return new WaitForSeconds(time);
        //trigger ghost death animation and then disable
        ghostInput.gameObject.SetActive(false);
        originalInput.enabled = true;
    }
    private void PlayInput(RecordedInput input)
    {
        switch (input.type)
        {
            case InputType.AimDrag:
                ghostInput.mouseDragging(input.value);
                break;
            case InputType.AimRelease:
                ghostInput.mouseReleased();
                break;
            case InputType.SlamActive:
                ghostInput.PoundAbility();
                break;
            case InputType.SlamRelease:
                ghostInput.PoundReleased();
                break;
            case InputType.Dash:
                //ghostController.Dash(input.value);
                break;
            case InputType.Position:
                ghostInput.transform.position = input.value;
                break;
        }
    }

    private void ResetInputs()
    {

    }
}
