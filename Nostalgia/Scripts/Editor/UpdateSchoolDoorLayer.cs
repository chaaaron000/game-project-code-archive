using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UpdateSchoolDoorLayer : EditorWindow
{
    private static string targetPrefabName = "SchoolDoor"; // 변경할 프리팹 이름
    private static string targetChildName = "door"; // 변경할 자식 오브젝트 이름
    private static string newLayerName = "Obstacle"; // 변경할 Layer 이름

    [MenuItem("Tools/Update SchoolDoor Layer")]
    static void UpdateLayerInAllPrefabs()
    {
        int newLayer = LayerMask.NameToLayer(newLayerName);
        if (newLayer == -1)
        {
            Debug.LogError($"Layer '{newLayerName}'을(를) 찾을 수 없습니다. Unity의 Tags & Layers에서 확인하세요.");
            return;
        }

        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab");
        List<string> modifiedPrefabs = new List<string>();

        foreach (string guid in allPrefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            bool modified = false;

            // 프리팹 내부에서 "schoolDoor" 프리팹을 포함하는 오브젝트 찾기
            Transform[] allChildren = prefab.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.name == targetPrefabName) // "schoolDoor" 프리팹을 찾았으면
                {
                    Transform doorObject = child.Find(targetChildName); // "door"라는 빈 오브젝트 찾기
                    if (doorObject != null)
                    {
                        // 🔥 "door" 자신과 모든 자식 오브젝트 Layer 변경
                        SetLayerRecursively(doorObject, newLayer);
                        modified = true;
                        Debug.Log($"Updated '{doorObject.name}' and its children Layer to '{newLayerName}' in {path}");
                    }
                }
            }

            // 변경이 일어난 경우, 저장
            if (modified)
            {
                PrefabUtility.SavePrefabAsset(prefab);
                modifiedPrefabs.Add(path);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"총 {modifiedPrefabs.Count}개의 프리팹이 업데이트되었습니다!");
    }

    // 🔥 모든 자식 오브젝트까지 Layer를 변경하는 함수
    static void SetLayerRecursively(Transform obj, int layer)
    {
        obj.gameObject.layer = layer; // 자신 변경
        foreach (Transform child in obj)
        {
            SetLayerRecursively(child, layer); // 자식도 변경
        }
    }
}