using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyFrame : MonoBehaviour
{
    public GameObject key;

    bool isFilled = false;

    // Start is called before the first frame update
    void Start()
    {
        SetKeyActive(false);
    }

    void SetKeyActive(bool state)
    {
        key.SetActive(state);
    }

    public void ObjectClicked()
    {
        if (!isFilled)
        {
            SetKeyActive(true);
            isFilled = true;
        }
    }

    public string ReturnKeyName()
    {
        return key.name;
    }

    public bool FilledState()
    {
        return isFilled;
    }
}
