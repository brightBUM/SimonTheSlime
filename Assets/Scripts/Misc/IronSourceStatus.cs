using UnityEngine;
using UnityEngine.UI;

public class IronSourceStatus : MonoBehaviour
{
    [SerializeField] Image[] images; // 0- sdk , 1 - inter , 2 - reward

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        images[0].color = AssignStatusColor(IronSourceAdManager.Instance.sdkInitialized);
        images[1].color = AssignStatusColor(IronSourceAdManager.Instance.IsInterstitialAdReady());
        images[2].color = AssignStatusColor(IronSourceAdManager.Instance.IsRewardedAdReady());
    }
    private Color AssignStatusColor(bool value)
    {
        return value? Color.green:Color.red;
    }
    
}
