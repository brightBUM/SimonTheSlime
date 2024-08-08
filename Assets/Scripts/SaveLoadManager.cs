using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    private string fileName = "GameSave.json";
    private string filePath;

    public const string MASTER_VOLUME_TOGGLE = "MasterVolumeToggle";
    public const string MASTER_VOLUME_VALUE = "MasterVolumeValue";
    public const string MUSIC_VOLUME_TOGGLE = "MusicVolumeToggle";
    public const string MUSIC_VOLUME_VALUE = "MusicVolumeValue";
    public const string SFX_VOLUME_TOGGLE = "SfxVolumeToggle";
    public const string SFX_VOLUME_VALUE = "SfxVolumeValue";
    public const string PLAYER_PROGRESS = "PlayerProgress";
    private void Awake()
    {
        
        filePath = Application.persistentDataPath + "/" + fileName;
        //Directory.CreateDirectory(filePath);
        if(File.Exists(filePath))
        {
            //get data 
        }
        else
        {
            //create a new save file with default values;
        }
    }

    public bool CheckFileExist()
    {
        return File.Exists(filePath);
    }

    public void SaveData(Dictionary<int, string> leaderBoardData)
    {
        string fileData = " ";
        foreach (var item in leaderBoardData)
        {
            fileData += item.Key + "," + item.Value + "\n";
        }
        Debug.Log("Saved Data @" + filePath);
        File.WriteAllText(filePath, fileData);
    }
    public Dictionary<int, string> GetData()
    {
        string[] fileContent = File.ReadAllLines(filePath);
        Dictionary<int, string> dictionaryData = new Dictionary<int, string>();

        foreach (string line in fileContent)
        {
            string[] temp = line.Split(',');
            if (temp.Length == 2)
                dictionaryData.Add(Convert.ToInt32(temp[0]), temp[1]);
        }
        return dictionaryData;
    }
    public void SetKey(string key,bool value)
    {

    }
    public void GetValue(string key)
    {

    }
}
