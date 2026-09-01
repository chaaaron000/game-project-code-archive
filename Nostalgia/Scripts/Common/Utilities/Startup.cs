using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InstantiatePrefabs()
    {
        Debug.Log("--- Startup.InstantiatePrefabs() ---");

        GameObject[] prefabsToInstantiate = Resources.LoadAll<GameObject>("Manager/");

        foreach (GameObject prefab in prefabsToInstantiate)
        {
            GameObject.Instantiate(prefab);
        }

        Debug.Log("--- Startup.InstantiatePrefabs() Done ---");
    }
}
