using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TutorialPlayback))]
public class TutorialPlaybackEditor:Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector first
        DrawDefaultInspector();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Playback Controls", EditorStyles.boldLabel);

        TutorialPlayback playback = (TutorialPlayback)target;

        if (GUILayout.Button("Resume"))
        {
            playback.ResumeTutorial();
        }
    }
}
