using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/TutorialData")]
public class TutorialData : ScriptableObject
{
    public List<RecordedInput> inputs;
}
