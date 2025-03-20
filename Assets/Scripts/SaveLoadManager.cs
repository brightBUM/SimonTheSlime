using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Numerics;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    private string fileName = "GameSave.json";
    private string filePath;
    public Action<bool> skipCutScene;
    public PlayerProfile playerProfile;
    public bool firstLoad = false;
    public DateTime lastRewardedAdTime;
    
    private void Awake()
    {
        
    }

    public void InitFileSystem()
    {
        filePath = Application.persistentDataPath + "/" + fileName;
        //Directory.CreateDirectory(filePath);
        if (File.Exists(filePath))
        {
            //get data
            LoadFromFile();
            skipCutScene(true);
        }
        else
        {
            //create a new save file with default values;
            playerProfile = new PlayerProfile
            {
                profileName = "default",
                levelStats = new List<LevelStats>(6),
                volumeControls = new List<VolumeControl>(3),
                unlockedCharSkins = new List<int>() { 0 },
                unlockedPodSkins = new List<int>() { 0 },
                nanas = GameManger.Instance.gameConfig.nanasCount,
                melons = GameManger.Instance.gameConfig.melonsCount
            };

            for(int i=0;i<3;i++)
            {
                playerProfile.volumeControls.Add(new VolumeControl());
            }
            for (int i = 0; i < 6; i++)
            {
                playerProfile.levelStats.Add(new LevelStats
                {
                    levelIndex = i
                });
            }

            //unlocked lv1
            //playerProfile.levelStats[0].unlocked = true;

            //main menu rewarded ad ready
            this.lastRewardedAdTime = DateTime.Now.AddHours(-25);

            SaveGame();
            Debug.Log("New save file created @" + filePath);
            skipCutScene(false);
            firstLoad = true;
        }
    }

    public bool CheckFileExist()
    {
        return File.Exists(filePath);
    }

    
    public void LoadFromFile()
    {
        string data = File.ReadAllText(filePath);
        playerProfile = JsonUtility.FromJson<PlayerProfile>(data);

        //parse string to dateTime
        this.lastRewardedAdTime = DateTime.Parse(playerProfile.lastrewardedAdTime);
        Debug.Log("Game loaded from file");
    }
    public void SaveGame() 
    {
        //date time conversion
        playerProfile.lastrewardedAdTime = lastRewardedAdTime.ToString("o"); // "o" = ISO 8601 format

        string data = JsonUtility.ToJson(playerProfile);
        File.WriteAllText(filePath, data);
        Debug.Log("Game saved");
    }
    public void SaveProfileInfo(string name , int age)
    {
        firstLoad = false;
        this.playerProfile.profileName = name;
        this.playerProfile.age = age;
        SaveGame();
    }
    public void EquipSkin(CharSkinBase charSkinBase)
    {
        if(charSkinBase.isPod)
        {
            playerProfile.equippedPod = charSkinBase.skinNum;
        }
        else
        {
            playerProfile.equippedSkin = charSkinBase.skinNum;
        }
    }
    public bool PurchaseSkin(CharSkinBase charSkinBase)
    {
        if(playerProfile.melons>10)
        {
            playerProfile.melons -= 10;
            ShopManager.instance.UpdateCurrencyUI();

            if(charSkinBase.isPod)
            {
                playerProfile.unlockedPodSkins.Add(charSkinBase.skinNum);

            }
            else
            {
                playerProfile.unlockedCharSkins.Add(charSkinBase.skinNum);
            }

            SaveGame();
            return true;
            //unlock in shop & equip in game
        }
        return false;
    }
    public int GetLevelStarData(int index)
    {
        return playerProfile.levelStats[index].stars;
    }
    public bool GetLevelUnlockData(int index)
    {
        return playerProfile.levelStats[index].unlocked;
    }
    public void SetLevelStats(int index,int stars)
    {
        playerProfile.levelStats[index].stars = stars;
        SaveGame();
    }
    public void UnlockLevel(int index)
    {
        playerProfile.levelStats[index].unlocked = true;
        SaveGame();
    }

    public bool CheckIfSkinUnlocked(int item)
    {
        return playerProfile.unlockedCharSkins.Contains(item);
    }
    public bool CheckIfPodUnlocked(int item)
    {
        return playerProfile.unlockedPodSkins.Contains(item);
    }
    public int CheckIfSkinSelectedOrUnlocked(bool isPod,int item)
    {
        if(isPod)
        {
            if (playerProfile.equippedPod == item)
            {
                return 0;
            }
            else if (playerProfile.unlockedPodSkins.Contains(item))
            {
                return 1;
            }

            return 2;
        }


        if (playerProfile.equippedSkin == item)
        {
            return 0;
        }
        else if (playerProfile.unlockedCharSkins.Contains(item))
        {
            return 1;
        }

        return 2;

    }
    public void ToggleVolumeState(int index)
    {
        playerProfile.volumeControls[index].volumeState = !playerProfile.volumeControls[index].volumeState;
    }
    public VolumeControl GetVolumeControls(int index)
    {
        return playerProfile.volumeControls[index];
    }
    public void SetVolumeValue(int index, float volume)
    {
        playerProfile.volumeControls[index].volumeValue = volume;
    }
    public void SetVolumeState(int index, bool state)
    {
        playerProfile.volumeControls[index].volumeState = state;
    }

    public DateTime GetLastRewardedAdTime()
    {
        return this.lastRewardedAdTime;
    }   
    public void SetLastRewardedAdTime(DateTime dateTime)
    {
        this.lastRewardedAdTime = dateTime;
    }

    public bool CheckInterstitialAdCondition()
    {
        return playerProfile.interStitialAdCount >= GameManger.Instance.gameConfig.interstitialAdCheckPerLevel;
    }

    public void MainMenuAdRewarded()
    {
        playerProfile.nanas += GameManger.Instance.gameConfig.mainMenuRewardedAdNanas;
        SetLastRewardedAdTime(DateTime.Now);
        SaveGame();
    }
    
}
[System.Serializable]
public class PlayerProfile
{
    public string profileName;
    public int age;
    public int nanas;
    public int melons;
    public List<int> unlockedCharSkins;
    public List<int> unlockedPodSkins;
    public int equippedSkin;
    public int equippedPod;
    public List<LevelStats> levelStats;
    public List<VolumeControl> volumeControls;
    public string lastrewardedAdTime;
    public int interStitialAdCount;
}
[System.Serializable]
public class LevelStats
{
    public int levelIndex = 0;
    public int coinsCollected = 0;
    public float bestTime = 0.0f;
    public int minLaunches = 0;
    public int stars;
    public bool unlocked = true;
}
[System.Serializable]
public class VolumeControl
{
    public float volumeValue = 1.0f;
    public bool volumeState = true;
}
