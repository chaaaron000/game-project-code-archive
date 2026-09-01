using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomSimplePlayerUse : MonoBehaviour
{
    public GameObject mainCamera;
    private GameObject objectClicked;
    public GameObject flashlight;
    public KeyCode OpenCloseGetItem;
    public KeyCode Flashlight;

    private ItemDatabase itemDatabase;

    void Start()
    {
        itemDatabase = gameObject.GetComponent<ItemDatabase>();
    }

    void Update()
    {
        if (Input.GetKeyDown(OpenCloseGetItem)) // Open and close action
            {
                RaycastCheck();
            }

        if (Input.GetKeyDown(Flashlight)) // Toggle flashlight
        {
            if (itemDatabase.hasItem("FlashLight"))
            {
                if (flashlight.activeSelf )
                    flashlight.SetActive(false);
                else
                    flashlight.SetActive(true);
            }
            else
            {
                Debug.Log("손전등이 없습니다.");
            }
        }
    }
    
    void RaycastCheck()
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, 2.3f))
        {
            if (hit.collider.gameObject.GetComponent<SimpleOpenClose>())
            {
                
                //Debug.Log("Object with SimpleOpenClose script found");
                hit.collider.gameObject.BroadcastMessage("ObjectClicked");
            }
            // else
            // {
            //    Debug.Log("Object doesn't have script SimpleOpenClose attached");

            // }

            if (hit.collider.gameObject.GetComponent<DoorOpenClose>())
            {
                Debug.Log("Object with DoorOpenClose script found");
                hit.collider.gameObject.BroadcastMessage("ObjectClicked");
            }

            if (hit.collider.gameObject.GetComponent<LockController>())
            {
                Debug.Log("Object with LockController script found");
                Debug.Log(hit.collider.gameObject.name);
                hit.collider.gameObject.BroadcastMessage("ObjectClicked");
            }
            if (hit.collider.gameObject.GetComponent<WatchController>())
            {
                Debug.Log("Object with WatchController script found");
                hit.collider.gameObject.BroadcastMessage("WatchClicked");
            }
            
            if (hit.collider.gameObject.GetComponent<SimpleGetItem>()){
                hit.collider.gameObject.BroadcastMessage("ObjectClicked");
            }

            if (hit.collider.gameObject.GetComponent<KeyFrame>())
            {
                string keyName = hit.collider.gameObject.GetComponent<KeyFrame>().ReturnKeyName();
                if (itemDatabase.hasItem(keyName))
                {
                    hit.collider.gameObject.GetComponent<KeyFrame>().ObjectClicked();
                    itemDatabase.DeleteData(keyName);
                }
            }

           // Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
           // Debug.Log("Did Hit");
        }
        else
        {
         // Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
         //   Debug.Log("Did not Hit");


        }

    }

}
