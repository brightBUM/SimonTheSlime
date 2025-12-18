using UnityEngine;
using UnityEditor;
using System.IO;

public class CreatureDataImporter
{
    private const string CsvPath = "Assets/Scripts/ScriptableObjects/Creatures/Slunky Creatures - Sheet.csv";
    private const string BaseOutputPath = "Assets/Scripts/ScriptableObjects/Creatures";

    [MenuItem("Tools/Import Creature Data")]
    public static void Import()
    {
        if (!File.Exists(CsvPath))
        {
            Debug.LogError("CSV file not found!");
            return;
        }

        string[] lines = File.ReadAllLines(CsvPath);

        // Skip header
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] columns = lines[i].Split(',');

            string tier = columns[0].Trim();
            string name = columns[1].Trim();

            string folderPath = $"{BaseOutputPath}/{tier}";
            EnsureFolderExists(folderPath);

            string assetPath = $"{folderPath}/{name}.asset";

            CreatureData data = AssetDatabase.LoadAssetAtPath<CreatureData>(assetPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<CreatureData>();
                AssetDatabase.CreateAsset(data, assetPath);
            }

            data.creatureName = name;
            data.creatureType = ParseCreatureType(tier);
            data.weight = columns[2].Trim();
            data.region = columns[3].Trim();
            data.info = columns[4].Trim();
            data.unq_info = columns[5].Trim();

            // Optional: Auto-assign sprite if naming matches
            data.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                $"Assets/Creatures/Sprites/{name}.png"
            );

            EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Creature data imported into tier folders successfully!");
    }

    private static void EnsureFolderExists(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path);
        string folderName = Path.GetFileName(path);

        EnsureFolderExists(parent);
        AssetDatabase.CreateFolder(parent, folderName);
    }

    private static CreatureType ParseCreatureType(string tier)
    {
        return tier switch
        {
            "Common" => CreatureType.Common,
            "Rare" => CreatureType.Rare,
            "Epic" => CreatureType.Epic,
            _ => CreatureType.Common
        };
    }
}
