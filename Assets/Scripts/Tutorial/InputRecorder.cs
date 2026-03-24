using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InputRecorder : MonoBehaviour
{
    [HideInInspector] public List<RecordedInput> recordedInputs = new();
    public PlayerInput playerInput;
    public TutorialData tutorialDataSO;
    [SerializeField] private float recordFPS = 30f;
    float startTime;

    float recordInterval;
    float nextRecordTime;
    
  
    public void StartInputRecording()
    {
        startTime = Time.time;
        recordInterval = 1f / recordFPS;
        nextRecordTime = 0f;

        //tutorialDataSO.inputs.Clear();
        recordedInputs = new List<RecordedInput>();

        startTime = Time.time;
        recordedInputs.Add(new RecordedInput
        {
            time = Time.time - startTime,
            type = InputType.Position,
            value = playerInput.transform.position
        });
    }
    private void OnEnable()
    {
        playerInput.mouseDragging += MouseDragging;
        playerInput.mouseReleased += MouseReleased;
        playerInput.PoundAbility  += PoundActive;
        playerInput.PoundReleased  += PoundRelease;
        playerInput.DashAbility += Dash;
        playerInput.GrappleAbility += Grapple;
    }

    private void PoundActive()
    {
        recordedInputs.Add(new RecordedInput
        {
            time = Time.time - startTime,
            type = InputType.SlamActive,
            value = Vector2.zero
        });
    }
    private void Dash()
    {
        recordedInputs.Add(new RecordedInput
        {
            time = Time.time - startTime,
            type = InputType.Dash,
            value = Vector2.zero
        });
    }
    private void PoundRelease()
    {
        recordedInputs.Add(new RecordedInput
        {
            time = Time.time - startTime,
            type = InputType.SlamRelease,
            value = Vector2.zero
        });
    }
    private void MouseDragging(Vector2 move)
    {
        if (move == Vector2.zero) return;

        float elapsed = Time.time - startTime;

        if (elapsed < nextRecordTime) return;

        recordedInputs.Add(new RecordedInput
        {
            time = elapsed,
            type = InputType.AimDrag,
            value = move
        });

        nextRecordTime += recordInterval;
    }

    private void MouseReleased()
    {
        recordedInputs.Add(new RecordedInput
        {
            time = Time.time - startTime,
            type = InputType.AimRelease,
            value = Vector2.zero
        });
    }
    
    private void Grapple()
    {
        recordedInputs.Add(new RecordedInput
        {
            time = Time.time - startTime,
            type = InputType.Grapple,
            value = Vector2.zero
        });
    }
    
    public void SetTutorialDataSO()
    {
        tutorialDataSO.inputs = recordedInputs;

#if UNITY_EDITOR
        EditorUtility.SetDirty(tutorialDataSO);
        AssetDatabase.SaveAssets();
#endif
    }

    private void OnDisable()
    {
        playerInput.mouseDragging  -= MouseDragging;
        playerInput.mouseReleased  -= MouseReleased;
        playerInput.PoundAbility   -= PoundActive;
        playerInput.PoundReleased  -= PoundRelease;
        playerInput.GrappleAbility -= Grapple;

    }

}

