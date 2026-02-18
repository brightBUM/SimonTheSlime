using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/TutorialData")]
public class TutorialData : ScriptableObject
{
    public List<RecordedInput> inputs;
    public float pauseTransitionTime = 0.25f;
    public float uiAnimationDuration = 1.5f;
    public float endDelay = 1f;
    public bool resumeWithPause;
}
