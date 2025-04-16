using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] Transform[] buttonsTabs;
    [SerializeField] Transform[] ScreenTabs;
    [SerializeField] TextMeshProUGUI nanasText;
    [SerializeField] TextMeshProUGUI melonsText;
    [SerializeField] CharSkinLoad charSkinLoad;
    // Start is called before the first frame update

    public List<CharSkinBase> charSkinList;
    public List<CharSkinBase> podList;
    public static ShopManager instance;
    public bool Init = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        charSkinList = new List<CharSkinBase>();
        podList = new List<CharSkinBase>();
        Init = true;
    }

    private void OnEnable()
    {
        UpdateCurrencyUI();
    }
    public void UpdateCurrencyUI()
    {
        nanasText.text = SaveLoadManager.Instance.playerProfile.nanas.ToString();
        melonsText.text = SaveLoadManager.Instance.playerProfile.melons.ToString();
    }
    public void AddToList(CharSkinBase charSkinBase)
    {
        if (charSkinBase.isPod)
        {
            podList.Add(charSkinBase);
        }
        else
        {
            charSkinList.Add(charSkinBase);
        }
    }
    public void SaveOnShopExit()
    {
        charSkinList.Clear();
        podList.Clear();
        SaveLoadManager.Instance.SaveGame();
        charSkinLoad.RefreshSkin();
    }
    public void SetEquippedSkin(CharSkinBase charSkinBase)
    {
        if (charSkinBase.isPod)
        {
            //loop all the pods
            foreach (CharSkinBase podSkin in podList)
            {
                if (charSkinBase.skinNum == podSkin.skinNum)
                {
                    podSkin.FlipSelection(true);
                }
                else
                {
                    podSkin.FlipSelection(false);
                }
            }

        }
        else
        {
            foreach (CharSkinBase charSkin in charSkinList)
            {
                if (charSkinBase.skinNum == charSkin.skinNum)
                {
                    charSkin.FlipSelection(true);
                }
                else
                {
                    charSkin.FlipSelection(false);
                }
            }
        }
        Debug.Log(string.Format("pod list count : {0} ,charSkin List Count : {1}", charSkinList.Count, podList.Count));
    }
    public void SelectPage(int index)
    {
        for (int i = 0; i < buttonsTabs.Length; i++)
        {
            if(i==index)
            {
                buttonsTabs[i].DOScale(Vector3.one, 0.1f).SetEase(Ease.OutFlash).SetUpdate(true);
                ScreenTabs[i].gameObject.SetActive(true);
            }
            else
            {
                buttonsTabs[i].localScale = Vector3.zero;
                ScreenTabs[i].gameObject.SetActive(false);
            }
        }
    }
        
    
}
