using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KillPlayerUIController : MonoBehaviour
{
    public Button KillFatherButton;
    public Button KillDaughterButton;
    public Player Father;
    public Player Daughter;
    public int MobID = 1;

    private void OnEnable()
    {
        KillFatherButton.onClick.AddListener(() => OnKillButtonClick(true));
        KillDaughterButton.onClick.AddListener(() => OnKillButtonClick(false));
    }

    private void OnDisable()
    {
        KillFatherButton.onClick.RemoveAllListeners();
        KillDaughterButton.onClick.RemoveAllListeners();
    }

    void OnKillButtonClick(bool isFather)
    {
        if (GameManager.Instance.DebugMode == false) return;
        
        if (isFather)
        {
            if (Father == null)
                Father = GameManager.Instance.FatherNetworkObject.GetComponent<Player>();
            
            if (Father != null && KillFatherButton != null)
                Father.DealDamageRpc(100, MobID);
        }
        else
        {
            if (Daughter == null)
                Daughter = GameManager.Instance.DaughterNetworkObject.GetComponent<Player>();
            
            if (Daughter != null && KillDaughterButton != null)
                Daughter.DealDamageRpc(100, MobID);
        }
    }
}
