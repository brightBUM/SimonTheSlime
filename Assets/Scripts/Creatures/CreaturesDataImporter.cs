//using System.Collections.Generic;
//using System.IO;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//using UnityEngine;

//public class CreatureDataImporter
//{
//    private const string CsvPath = "Assets/Scripts/ScriptableObjects/Creatures/Slunky Creatures - Sheet.csv";
//    private const string BaseOutputPath = "Assets/Scripts/ScriptableObjects/Creatures";
//#if UNITY_EDITOR
//    [MenuItem("Tools/Import Creature Data")]
//    public static void Import()
//    {
//        if (!File.Exists(CsvPath))
//        {
//            Debug.LogError("CSV file not found!");
//            return;
//        }

//        string[] lines = File.ReadAllLines(CsvPath);

//        // Track running index per tier
//        Dictionary<string, int> tierCounters = new()
//        {
//            { "Common", 0 },
//            { "Rare", 0 },
//            { "Epic", 0 }
//        };

//        // Skip header
//        for (int i = 1; i < lines.Length; i++)
//        {
//            if (string.IsNullOrWhiteSpace(lines[i]))
//                continue;

//            string[] columns = lines[i].Split(',');

//            string tier = columns[0].Trim();
//            string name = columns[1].Trim();

//            tierCounters[tier]++;

//            string folderPath = $"{BaseOutputPath}/{tier}";
//            EnsureFolderExists(folderPath);

//            string assetPath = $"{folderPath}/{name}.asset";
//            CreatureData data = AssetDatabase.LoadAssetAtPath<CreatureData>(assetPath);

//            bool isNewAsset = false;
//            if (data == null)
//            {
//                data = ScriptableObject.CreateInstance<CreatureData>();
//                AssetDatabase.CreateAsset(data, assetPath);
//                isNewAsset = true;
//            }

//            // Assign ID only once (never overwrite)
//            if (string.IsNullOrEmpty(data.creatureId))
//            {
//                data.creatureId = GenerateCreatureId(tier, tierCounters[tier]);
//            }

//            data.creatureName = name;
//            data.creatureType = ParseCreatureType(tier);
//            data.weight = columns[2].Trim();
//            data.region = columns[3].Trim();
//            data.info = columns[4].Trim();
//            data.unq_info = columns[5].Trim();

//            // Optional sprite auto-link
//            data.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
//                $"Assets/Creatures/Sprites/{name}.png"
//            );

//            EditorUtility.SetDirty(data);
//        }

//        AssetDatabase.SaveAssets();
//        AssetDatabase.Refresh();

//        Debug.Log("Creature data imported with stable IDs!");
//    }
//#endif
//    private static string GenerateCreatureId(string tier, int index)
//    {
//        return $"C_{tier.ToUpper()}_{index:D3}";
//    }
//    private static void EnsureFolderExists(string path)
//    {
//        if (AssetDatabase.IsValidFolder(path))
//            return;

//        string parent = Path.GetDirectoryName(path);
//        string folderName = Path.GetFileName(path);

//        EnsureFolderExists(parent);
//        AssetDatabase.CreateFolder(parent, folderName);
//    }

//    private static CreatureType ParseCreatureType(string tier)
//    {
//        return tier switch
//        {
//            "Common" => CreatureType.Common,
//            "Rare" => CreatureType.Rare,
//            "Epic" => CreatureType.Epic,
//            _ => CreatureType.Common
//        };
//    }
//}
