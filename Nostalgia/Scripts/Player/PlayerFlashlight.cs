using Fusion;
using UnityEngine;
using System;
using System.Collections;

public class PlayerFlashlight : NetworkBehaviour
{
    public GameObject Flashlight = null;

    public Animator animator;

    public Transform RightHand;

    private int layerIndex_flashLight;
    public bool _flashlightEnable = true;
    private PlayerInventory playerInventory;

    [Networked, OnChangedRender(nameof(FlashlightChanged))]
    public bool FlashlightToggle
    {
        get;
        set;
    } = false;

    void Awake()
    {
        Flashlight.SetActive(false);
        layerIndex_flashLight = animator.GetLayerIndex("flashLight Layer");
        playerInventory = GetComponentInParent<PlayerInventory>();
    }

    public void PressFlashlightButton()
    {
        if(_flashlightEnable == false) return;
        // Debug.Log("PressFlashlightButton: " + FlashlightToggle);
        FlashlightToggle = !FlashlightToggle;
    }

    public void ChangeFlashlightEnable(bool value) {
        _flashlightEnable = value;
        //이미 flashlight가 켜져있는 상태에서 flashlightenable를 false로 바꾸면 flashlight를 끔
        if(FlashlightToggle == true && value == false) {
            FlashlightToggle = false;
        }   
    }

    void FlashlightChanged()
    {
        Flashlight.SetActive(FlashlightToggle);
        SetAnimatorRpc("isLantern",FlashlightToggle);

        if (HasStateAuthority)
        {
            playerInventory.OnFlashlightChange(FlashlightToggle);
        }
    }

    private void OnAnimatorIK(int _layerIndex) 
    {
        if (_layerIndex != layerIndex_flashLight)
        {
            return;
        }

        if (FlashlightToggle)
        {
            // FlashlightToggle이 true일 때 IK 적용
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKPosition(AvatarIKGoal.RightHand, RightHand.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, RightHand.rotation);
        }
        else
        {
            // FlashlightToggle이 false일 때 IK Weight를 0으로 설정하여 IK 비활성화
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetAnimatorRpc(string name, bool value) {
        animator.SetBool(name, value);
    }
}
