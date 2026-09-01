using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemOnOff : MonoBehaviour
{
    private Canvas canvas;
    public Image cube;
    public GameObject inventoryUI;

    private void Awake()
    {
        canvas = GameObject.FindObjectOfType<Canvas>();
        inventoryUI = canvas.transform.Find("BackgroundPanel/InventoryUI").gameObject;
    }

    public void SetItemOn(string gameObjectName)
    {
        if (canvas != null)
        {
            Debug.Log("this is SetItemOn");
            Transform cubeTransform = inventoryUI.transform.Find(gameObjectName);
            Debug.Log(gameObjectName);
            if (inventoryUI == null)
            {
                Debug.Log("InventoryUI is null");
                return;
            }

            if (cubeTransform != null)
            {
                cube = cubeTransform.GetComponent<Image>();
                if (cube != null)
                {
                    cube.enabled = true;
                }
                else
                {
                    Debug.LogWarning("Cube component is missing!");
                }
            }
            else
            {
                Debug.LogWarning(gameObjectName + " not found in InventoryUI!");
            }
        }
        else
        {
            Debug.LogWarning("Canvas is missing!");
        }
    }   

    public void SetItemOff(string gameObjectName)
    {
        if (canvas != null)
        {
            Debug.Log("this is SetItemOff");
            Transform cubeTransform = inventoryUI.transform.Find(gameObjectName);
            Debug.Log(gameObjectName);
            if (inventoryUI == null)
            {
                Debug.Log("InventoryUI is null");
                return;
            }

            if (cubeTransform != null)
            {
                cube = cubeTransform.GetComponent<Image>();
                if (cube != null)
                {
                    cube.enabled = false;
                }
                else
                {
                    Debug.LogWarning("Cube component is missing!");
                }
            }
            else
            {
                Debug.LogWarning(gameObjectName + " not found in InventoryUI!");
            }
        }
        else
        {
            Debug.LogWarning("Canvas is missing!");
        }
    }   
}
