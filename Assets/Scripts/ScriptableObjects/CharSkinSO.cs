using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CharSkin")]
public class CharSkinSO : ScriptableObject
{
    public List<Skin> skinList = new List<Skin>();
    public List<Skin> podList = new List<Skin>();
}

[System.Serializable]
public class Skin
{
    public string skinName;
    public Color tintColor;
    public float hueshift;
    public float saturation;
    public float invert;
}
