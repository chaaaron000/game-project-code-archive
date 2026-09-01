using UnityEditor;
using UnityEngine;

public class FindMissingScript
{
    [MenuItem("Tools/Find Missing Scripts in Scene")]
    public static void FindMissingScripts()
    {
        GameObject[] gos = GameObject.FindObjectsOfType<GameObject>();
        int missingCount = 0;

        foreach (GameObject go in gos)
        {
            Component[] components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null) {
                    Debug.LogWarning($"Missing script in GameObject: {go.name}", go);
                }
            }
        }

        Debug.Log($"총 Missing Script 수: {missingCount}");
    }
}