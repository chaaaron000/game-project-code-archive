using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public void ShowChaseImage()
    {
        UIManager.Instance.PlayerUIController.ShowChaseImage();
    }
    public void HideChaseImage()
    {
        UIManager.Instance.PlayerUIController.HideChaseImage();
    }

    public void SetStamina(float stamina)
    {
        UIManager.Instance.PlayerUIController.SetStamina(stamina);
    }
}
