using UnityEngine;

[CreateAssetMenu(fileName = "CreatureData", menuName = "CreatureData")]
public class CreatureData : ScriptableObject
{
    public string creatureName;
    public CreatureType creatureType;
    public Sprite sprite;
    public string weight;
    public string region;
    public string info;
    public string unq_info;
}
