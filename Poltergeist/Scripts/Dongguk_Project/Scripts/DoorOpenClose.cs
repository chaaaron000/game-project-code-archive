using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenClose : MonoBehaviour
{
    private Animator myAnimator;
    private Animator additionalAnimator;
    public bool objectOpen;
    public bool objectOpenAdditional;
    public GameObject animateAdditional;
    private bool hasAdditional = false;
    float myNormalizedTime;

    public enum DoorType
    {
        CantOpen,
        KeyDoor,
        QuizDoor,
        BasementDoor,
        InfiniteLoopEntrance
    }
    public DoorType doorType;

    private ItemDatabase itemDatabase;

    public bool needKey;
    public bool doorLocked = true;
    public GameObject crossline;
    private CrosslineCollider crosslineCollider;

    KeyFrame sphKeyFrame;
    KeyFrame primKeyFrame;
    KeyFrame CubeKeyFrame;

    // Open or close animator state in start depending on selection.
    // Additional object with animator. For example another door when double doors. 
    void Start()
    {
       
        // If there is no animator in the gameobject itself, get the parent animator.
        myAnimator = GetComponent<Animator>();
        if (myAnimator == null)
        {
            myAnimator = GetComponentInParent<Animator>();
        }
        

        if (objectOpen == true)
        {
            myAnimator.Play("Open", 0, 1.0f);
        }
        if (animateAdditional != null)
            if (animateAdditional.GetComponent<DoorOpenClose>())
            {
                additionalAnimator = animateAdditional.GetComponent<Animator>();
                hasAdditional = true;
                objectOpenAdditional = animateAdditional.GetComponent<DoorOpenClose>().objectOpen;
            }
        else
            {
                hasAdditional = false;
            }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("playerObject is not assigned.");
            return;
        }

        itemDatabase = playerObject.GetComponent<ItemDatabase>();

        if (doorType == DoorType.BasementDoor || doorType == DoorType.InfiniteLoopEntrance)
        {
            crosslineCollider = crossline.GetComponent<CrosslineCollider>();
        }

        if (doorType == DoorType.InfiniteLoopEntrance)
        {
            sphKeyFrame = GameObject.Find("Sphere_KeyFrame").GetComponent<KeyFrame>();
            primKeyFrame = GameObject.Find("Prism_KeyFrame").GetComponent<KeyFrame>();
            CubeKeyFrame = GameObject.Find("Cube_KeyFrame").GetComponent<KeyFrame>();
        }

    }

    // Player clicks object. Method called from SimplePlayerUse script.

    public void ObjectClicked()
    {
        if (doorLocked)
        {
            switch (doorType)
            {
                case DoorType.CantOpen:
                    Debug.Log("열 수 없는 문입니다.");
                    return;

                case DoorType.KeyDoor:
                    Debug.Log("키로 여는 문입니다.");
                    if (!CheckForItem("Key"))
                    {
                        Debug.Log("키가 없습니다.");
                        return;
                    }

                    doorLocked = false;
                    itemDatabase.DeleteData("Key");
                    break;

                case DoorType.QuizDoor:
                    Debug.Log("퀴즈로 여는 문입니다.");
                    break;

                case DoorType.BasementDoor:
                    // Debug.Log("지하실 문입니다.");

                    // Crossline을 한번 지나가면 더 이상 열리지 않게 하는 조건
                    if (crosslineCollider.CheckIsCrossed())
                    {
                        // Debug.Log("더 이상 열리지 않습니다.");
                        return;
                    }

                    // 키가 필요한 문 일때 키를 체크한다
                    if (needKey)
                    {
                        if (CheckForItem("Key"))
                        {
                            doorLocked = false;
                            itemDatabase.DeleteData("Key");
                            break;
                        }
                        else
                        {
                            // Debug.Log("방에서 키를 찾으세요");
                            return;
                        }
                    }

                    break;

                case DoorType.InfiniteLoopEntrance:

                    if (crosslineCollider.CheckIsCrossed())
                    {
                        // Debug.Log("더 이상 열리지 않습니다.");
                        return;
                    }

                    if (sphKeyFrame.FilledState() && primKeyFrame.FilledState() && CubeKeyFrame.FilledState())
                    {
                        Debug.Log("키를 다 찾았습니다");
                        break;
                    }
                    else 
                    {
                        Debug.Log("키가 없습니다.");
                        return;
                    }


                default:
                    Debug.Log("올바르지 않은 타입입니다.");;
                    break;
            }
        }

        myNormalizedTime = myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (hasAdditional == false)
        {
            if (myNormalizedTime >= 1.0)
            {
                if (objectOpen == true)
                {   
                    myAnimator.Play("Close", 0, 0.0f);
                    objectOpen = false;
                }

                else
                {
                    
                    myAnimator.Play("Open", 0, 0.0f);
                    objectOpen = true;
                }
            }
        }

        if (hasAdditional == true && myNormalizedTime >= 1.0)
        {
            if (objectOpen == true)
            {
                myAnimator.Play("Close", 0, 0.0f);
                objectOpen = false;
                animateAdditional.GetComponent<DoorOpenClose>().objectOpenAdditional = false;

                if (objectOpenAdditional == true)
                {
                    additionalAnimator.Play("Close", 0, 0.0f);
                    objectOpenAdditional = false;
                    animateAdditional.GetComponent<DoorOpenClose>().objectOpen = false;
                }

            }

            else
            {
                myAnimator.Play("Open", 0, 0.0f);
                objectOpen = true;
                animateAdditional.GetComponent<DoorOpenClose>().objectOpenAdditional = true;

                if (objectOpenAdditional == false)
                {
                    additionalAnimator.Play("Open", 0, 0.0f);
                    objectOpenAdditional = true;
                    animateAdditional.GetComponent<DoorOpenClose>().objectOpen = true;

                }

            }

        }

    }

    bool CheckForItem(string item)
    {
        Debug.Log(itemDatabase.item);
        foreach(string obj in itemDatabase.item) {
            if(obj == item) {
                return true;
            }
        }   
        return false;
    }

}
