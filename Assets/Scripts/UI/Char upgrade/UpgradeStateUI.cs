using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStateUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI upgradeName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI costUI;
    [SerializeField] TextMeshProUGUI valueUI;
    //[SerializeField] Image[] upgrades;
    [SerializeField] Button upgradeButton;

    public void Init()
    {
        //get current upgrade stats from saveload
        //update the UI 
    }
    public void UpgradeButton()
    {
        //inc upgrade level
        //modify saveload 
        //update the UI 
    }
    public void UnlockCard()
    {
        upgradeButton.interactable = true;
    }
    private void UpdateText()
    {
        //simple setter function that updates the UI
    }
}
