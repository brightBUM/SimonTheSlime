using UnityEngine;

public enum InputType
{
    AimDrag,
    AimRelease,
    Dash,
    SlamActive,
    SlamRelease,
    Position,
    Grapple
}

[System.Serializable]
public struct RecordedInput
{
    public float time;
    public InputType type;
    public Vector2 value;
    public bool pauseBefore;
}



