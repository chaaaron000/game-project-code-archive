using System.IO;
using Newtonsoft.Json;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(
    fileName = "SO_MascotReactionTableEditor",
    menuName = "GreenClean/Mascot Reaction Table Editor",
    order = 0
)]
public class MascotReactionTableEditor : ScriptableObject
{
    private const string JsonPath = "Assets/Resources/GameData/mascot_reaction_table.json";

    public MascotReactionTable mascotReactionTable;

    public void LoadFromJson()
    {
        if (!File.Exists(JsonPath))
        {
            mascotReactionTable = new MascotReactionTable();
            return;
        }

        string json = File.ReadAllText(JsonPath);
        mascotReactionTable = JsonConvert.DeserializeObject<MascotReactionTable>(
            json,
            MascotReactionTable.JsonConvertSettings
        );
    }

    public void SaveToJson()
    {
        if (mascotReactionTable == null)
        {
            mascotReactionTable = new MascotReactionTable();
        }

        string directory = Path.GetDirectoryName(JsonPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonConvert.SerializeObject(
            mascotReactionTable,
            MascotReactionTable.JsonConvertSettings
        );
        File.WriteAllText(JsonPath, json);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Saved", $"Saved mascot reaction rules.\n{JsonPath}", "OK");
#endif
    }
}
