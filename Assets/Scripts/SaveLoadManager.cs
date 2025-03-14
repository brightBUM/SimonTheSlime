using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    private string fileName = "GameSave.json";
    private string filePath;

    public const string MASTER_VOLUME_TOGGLE = "MasterVolumeToggle";
    public const string MASTER_VOLUME_VALUE  = "MasterVolumeValue";
    public const string MUSIC_VOLUME_TOGGLE  = "MusicVolumeToggle";
    public const string MUSIC_VOLUME_VALUE   = "MusicVolumeValue";
    public const string SFX_VOLUME_TOGGLE    = "SfxVolumeToggle";
    public const string SFX_VOLUME_VALUE     = "SfxVolumeValue";
    public const string PLAYER_PROGRESS      = "PlayerProgress";

    public Action<bool> skipCutScene;
    public PlayerProfile playerProfile;
    public bool firstLoad = false;
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
                unlockedSkins = new List<int>()
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
            playerProfile.levelStats[0].unlocked = true;

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
        Debug.Log("Game loaded from file");
    }
    public void SaveGame() 
    {
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

    
}
[System.Serializable]
public class PlayerProfile
{
    public string profileName;
    public int age;
    public int nanas;
    public int melons;
    public List<int> unlockedSkins;
    public int equippedSkin;
    public List<LevelStats> levelStats;
    public List<VolumeControl> volumeControls;
}
[System.Serializable]
public class LevelStats
{
    public int levelIndex = 0;
    public int coinsCollected = 0;
    public float bestTime = 0.0f;
    public int minLaunches = 0;
    public int stars;
    public bool unlocked;
}
[System.Serializable]
public class VolumeControl
{
    public float volumeValue = 1.0f;
    public bool volumeState = true;
}
