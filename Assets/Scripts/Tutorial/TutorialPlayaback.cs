
using System;
using System.Collections;
using UnityEngine;

public class TutorialPlayback : MonoBehaviour
{
    public GameObject UIPromptObject;
    public TutorialData data;
    public PlayerInput ghostInput;
    float timer;
    int index;

    bool finished;
    bool isPaused;

    public Action OnPlayFinished ;
    public void Init()
    {
        PlayInput(data.inputs[index]);
        index++;
    }
    void Update()
    {
        if (finished || isPaused) return;

        timer += Time.deltaTime;

        PlayInputsForCurrentStep();
        CheckStepEnd();
    }
    void PlayInputsForCurrentStep()
    {
        while (index < data.inputs.Count &&
           data.inputs[index].time <= timer)
        {
            var input = data.inputs[index];

            //Pause marker
            if (input.pauseBefore)
            {
                PauseTutorial();
                return;
            }

            PlayInput(input);
            index++;
        }
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
                ghostInput.DashAbility();
                break;
            case InputType.Position:
                ghostInput.transform.position = input.value;
                break;
        }
    }

    

    void CheckStepEnd()
    {
        if (index >= data.inputs.Count && !finished)
        {
            finished = true;
            StartCoroutine(OnPlaybackFinished(data.endDelay));
        }
    }
    IEnumerator OnPlaybackFinished(float time)
    {
        Debug.Log("on playback finish");

        yield return new WaitForSeconds(time);

        OnPlayFinished.Invoke();

    }
    void PauseTutorial()
    {
        isPaused = true;
        ghostInput.Freeze.Invoke();
        StartCoroutine(TutorialPrompt());
    }
    IEnumerator TutorialPrompt()
    {
        yield return new WaitForSeconds(0.4f);

        UIPromptObject.SetActive(true);

        yield return new WaitForSeconds(data.uiPromptDelay);

        UIPromptObject.SetActive(false);


        ResumeTutorial();
    }
    
    public void ResumeTutorial()
    {
        ghostInput.UnFreeze.Invoke();

        isPaused = false;
        PlayInput(data.inputs[index]);
        index++;
    }

   
}
