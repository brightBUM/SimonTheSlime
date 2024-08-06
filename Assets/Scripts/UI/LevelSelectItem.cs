using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelTitleText;
    [SerializeField] Transform starparent;
    [SerializeField] GameObject starPrefab;
    [SerializeField] TextMeshProUGUI bestTimeText; 
    [SerializeField] Image levelImage;
    [SerializeField] Image specialItemImage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetBestTime()
    {
        // to be fetched from save/load
    }
    public void SetLevelSelectItem(LevelSelectInfo levelSelectInfo)
    {
        this.levelTitleText.text = levelSelectInfo.levelName;
        this.levelImage.sprite = levelSelectInfo.levelImage.sprite;
        this.specialItemImage.sprite = levelSelectInfo.specialItemImage.sprite;
    }
}

public class LevelSelectInfo
{
    public string levelName;
    public Image specialItemImage;
    public Image levelImage;
}
