using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureInfoPanel : MonoBehaviour
{
    [SerializeField] Image creatureImg;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI weight_regionText;
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] TextMeshProUGUI unq_descText;
    public void SetInfoData(CreatureData creatureData)
    {
        creatureImg.sprite = creatureData.sprite;
        nameText.text = creatureData.name;
        var joinedText = creatureData.weight + " / "+creatureData.region;
        weight_regionText.text = joinedText;
        descText.text = creatureData.info;
        unq_descText.text = creatureData.unq_info;
    }
}
