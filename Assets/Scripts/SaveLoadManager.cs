using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private string fileName = "GameSave.json";
    private string filePath;
    public Action<bool> skipCutScene;
    public PlayerProfile playerProfile;
    public bool firstLoad = false;
    public DateTime lastRewardedAdTime;
    int debugUnlock = 0;
    public static SaveLoadManager Instance;
    public Dictionary<string, bool> unlockMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
            var gameConfig = GameManger.Instance.gameConfig;
            //create a new save file with default values;
            playerProfile = new PlayerProfile
            {
                profileName = "default",
                levelUnlockProgress = debugUnlock,  // unlocked lv 1
                volumeControls = new List<VolumeControl>(3),
                unlockedCharSkins = new List<int>() { 0 },
                unlockedPodSkins = new List<int>() { 0 },
                levelStars = new List<int>(debugUnlock + 1), //assign 0 stars for all the debug unlock levels
                nanas = gameConfig.nanasCount,
                melons = gameConfig.melonsCount,
                dragSens = 3.5f,
                recoveryPodData = new List<RecoveryPodData>(),
                inventoryData = new List<InventoryState>(),
                creatureUnlockStates = new List<CreatureUnlockState>()
            };

            for(int i=0;i<3;i++)
            {
                playerProfile.volumeControls.Add(new VolumeControl());
            }

            for(int i=0;i<debugUnlock+1; i++)
            {
                playerProfile.levelStars.Add(0);
            }

            //give 2 free pods when game starts
            for (int i = 0; i < 2; i++)
            {
                playerProfile.recoveryPodData.Add(new RecoveryPodData
                {
                    podLevel = 1,
                    isUnlocked = true,
                    creatureType = 0
                });
            }

            //5 slot inventory
            playerProfile.inventoryData.Add(InventoryState.common);
            playerProfile.inventoryData.Add(InventoryState.rare);
            playerProfile.inventoryData.Add(InventoryState.vacant);
            playerProfile.inventoryData.Add(InventoryState.buy);
            playerProfile.inventoryData.Add(InventoryState.buy);

            //creature unlock state
            List<CreatureData> allCreatureData = new List<CreatureData>();
            allCreatureData.AddRange(gameConfig.commonData);
            allCreatureData.AddRange(gameConfig.rareData);
            allCreatureData.AddRange(gameConfig.epicData);

            //mark all as locked state 
            foreach(var creatureData in allCreatureData)
            {
                playerProfile.creatureUnlockStates.Add(new CreatureUnlockState(creatureData.creatureId,false));
            }

            //main menu rewarded ad ready
            this.lastRewardedAdTime = DateTime.Now.AddHours(-25);

            SaveGame();
            Debug.Log("New save file created @" + filePath);
            skipCutScene(false);
            firstLoad = true;
        }

        unlockMap = new Dictionary<string, bool>();
        foreach (var item in SaveLoadManager.Instance.playerProfile.creatureUnlockStates)
        {
            unlockMap[item.id] = item.acquired;
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
        var skinCost = GameManger.Instance.GetSkinByIndex(charSkinBase.isPod,charSkinBase.skinNum).melonCost;
        if(playerProfile.melons>=skinCost)
        {
            int endMelons = playerProfile.melons - skinCost;
            ShopManager.instance.UpdateCurrencyUI(1, playerProfile.melons, endMelons);
            playerProfile.melons = endMelons;

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
        if(index == playerProfile.levelUnlockProgress)
        {
            return 0;
        }
        return playerProfile.levelStars[index];
    }
    public int GetLevelUnlockData()
    {
        return playerProfile.levelUnlockProgress;
    }
    
    public void FirstOrReplay(int currentStars)
    {
        int levelIndex = LevelManager.Instance.levelIndex;
        if (levelIndex == playerProfile.levelUnlockProgress)
        {
            // first play
            if(levelIndex == 0)
            {
                //overwrite if first level
                playerProfile.levelStars[levelIndex] = currentStars;
            }
            else
            {
                playerProfile.levelStars.Add(currentStars);
            }
            Debug.Log($"stars awarded first time ,lvl {levelIndex + 1} : {currentStars} stars");
        }
        else if(currentStars > playerProfile.levelStars[levelIndex])
        {
            //replay 
            //save the currentstars if they are more than stored
            playerProfile.levelStars[levelIndex] = currentStars;
            Debug.Log($"replay level , stars overwritten, {levelIndex} : {currentStars}");
        }
    }
    
    
    public void UnlockLevel()
    {
        if(LevelManager.Instance.levelIndex == playerProfile.levelUnlockProgress)
        {
            playerProfile.levelUnlockProgress++;
        }
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

    //recoveryPodData
    public TimeSpan GetAssignPodRemainingTime(int podId)
    {
        return playerProfile.recoveryPodData[podId].GetRemainingTime();
    }
    public void BuyNewPod()
    {
        playerProfile.recoveryPodData.Add(new RecoveryPodData
        {
            podLevel = 1,
            isUnlocked = true,
            creatureType = 0
        });
        SaveGame();
    }
    public void UpgradePod(int podId)
    {
        playerProfile.recoveryPodData[podId].UpgradePodLevel();
        SaveGame();
    }
    public void AssignCreature(int podId,int creatureType)
    {
        playerProfile.recoveryPodData[podId].AssignCreature(creatureType);
    }
    public void CompleteRecovery(int podId)
    {

        playerProfile.recoveryPodData[podId].ClearPod();
    }

    //inventory data
    public bool IsInventorySlotAvailable()
    {
        return playerProfile.inventoryData.Contains(InventoryState.vacant);
    }
    public void AddCreatureToInventory(int creature)
    {
        int index = playerProfile.inventoryData.IndexOf(InventoryState.vacant); //gets first available slot
        if (index == -1)
            return; // inventory full

        playerProfile.inventoryData[index] = (InventoryState)creature;
    }
    public void BuyInventorySlot(int index)
    {
        playerProfile.inventoryData[index] = InventoryState.vacant;
        RearrangeInventory();
    }

    public Action InventoryArranged;

    public void RemoveCreatureFromInventory(int index)
    {
        playerProfile.inventoryData[index] = InventoryState.vacant;
        RearrangeInventory();
    }
    public void RearrangeInventory()
    {
        //rearrange after removal
        playerProfile.inventoryData.Sort((a, b) =>
        {
            return a.CompareTo(b);
        });
        InventoryArranged.Invoke();

        //var inventoryData = playerProfile.inventoryData;
        //var occupied = inventoryData.FindAll(x => x != InventoryState.vacant);
        //int vacantCount = inventoryData.Count - occupied.Count;

        //inventoryData.Clear();
        //inventoryData.AddRange(occupied);

        //for (int i = 0; i < vacantCount; i++)
        //    inventoryData.Add(InventoryState.vacant);
        ;
    }

    //creature collection data
    public void UnlockCreature(string creatureId)
    {
        unlockMap[creatureId] = true;
        playerProfile.creatureUnlockStates.Find(x => x.id == creatureId).acquired = true;
    }
    public bool IsCreatureUnlocked(string creatureId)
    {
        return unlockMap[creatureId];
    }
}
[System.Serializable]
public class PlayerProfile
{
    public string profileName;
    public int age;
    public int nanas;
    public int melons;
    public int screws;
    public int batteries;
    public List<int> unlockedCharSkins;
    public List<int> unlockedPodSkins;
    public int equippedSkin;
    public int equippedPod;
    public int levelUnlockProgress;
    public int pageUnlockProgress;
    public List<int> levelStars;
    public List<VolumeControl> volumeControls;
    public float dragSens;
    public string lastrewardedAdTime;
    public int interStitialAdCount;
    public List<RecoveryPodData> recoveryPodData;
    public List<InventoryState> inventoryData;
    public List<CreatureUnlockState> creatureUnlockStates;
}

[System.Serializable]
public class VolumeControl
{
    public float volumeValue = 1.0f;
    public bool volumeState = true;
}


