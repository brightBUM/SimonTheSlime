using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InputRecorder))]
public class InputRecorderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector first
        DrawDefaultInspector();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Recording Controls", EditorStyles.boldLabel);

        InputRecorder recorder = (InputRecorder)target;

        if (GUILayout.Button("Start Recording"))
        {
            recorder.StartInputRecording();
        }

        if (GUILayout.Button("Stop Recording"))
        {
            recorder.SetTutorialDataSO();
        }
        
    }
}
