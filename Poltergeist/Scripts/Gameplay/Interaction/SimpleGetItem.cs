using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGetItem : MonoBehaviour
{
    private ItemDatabase itemDatabase;
   

    void Start()
    {
        
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("playerObject is not assigned.");
            return;
        }
        itemDatabase = playerObject.GetComponent<ItemDatabase>();
    }

    void ObjectClicked()
    {   
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase is not assigned.");
            return;
        }
        Debug.Log(gameObject.name);
        
        
        
        itemDatabase.AddData(gameObject.name);

        
        Destroy(gameObject);
        
    }
}

