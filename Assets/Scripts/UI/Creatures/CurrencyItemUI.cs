using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CurrencyItemUI:MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI text;

    public void SetCurrencyData(Sprite sprite,string text)
    {
        this.icon.sprite = sprite;
        this.text.text = text;
    }
}