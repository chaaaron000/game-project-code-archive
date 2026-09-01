using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamTest : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        if(!SteamManager.Initialized)
        {
            Debug.Log("SteamManager is not initialized");
            return;
        }
        
        //returns my steam name(id)
        string name = SteamFriends.GetPersonaName();
        Debug.Log(name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
