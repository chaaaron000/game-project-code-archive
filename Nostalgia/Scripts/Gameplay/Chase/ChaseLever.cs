using System.Collections;
using System.Collections.Generic;
using Nostal.Interfaces;
using UnityEngine;
using Fusion;
using JetBrains.Annotations;
using Nostal;

public class ChaseLever : NetworkBehaviour, IInteractable
{
    [SerializeField] private bool isFatherLever;
    [Networked] public bool IsLeverPulled { get; private set; } = false;
    
    [Header("Door")]
    [SerializeField] private ChaseMapDoor m_door;
    
    [Header("Collider")]
    [SerializeField] private Collider m_leverCollider;
    
    [Header("Interact")]
    [SerializeField] private InteractPromptData[] m_interactPromptData;

    public Material pillarMaterial;
    private Coroutine m_pullCoroutine = null;

    public override void Spawned() 
    {
        //collider = GetComponent<Collider>();
        pillarMaterial.SetFloat("_Emission", 1.0f);

        GameplayEventManager.ChaseMapReset += Reset;
    }

    public void OnInteract(NetworkObject netObject)
    {
        if (( isFatherLever &&  GameManager.Instance.IsLocalPlayerFather) || 
            (!isFatherLever && !GameManager.Instance.IsLocalPlayerFather))
        {
            m_pullCoroutine = StartCoroutine(PullCoroutine());
        }
        else
        {
            Debug.Log("레버를 당길 수 없습니다.");
        }
    }

    public InteractPromptData GetInteractPromptData()
    {
        return m_interactPromptData[0];
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void OpenDoorRpc()
    {
        if (m_pullCoroutine != null)
        {
            StopCoroutine(m_pullCoroutine);
            m_pullCoroutine = null;
        }
        
        m_leverCollider.enabled = false;

        //일정 시간 이후 다시 emission을 꺼두기
        StartCoroutine(OpenDoorCoroutine());
    }
    
    public void Reset()
    {
        IsLeverPulled = false;
        
        pillarMaterial.SetFloat("_Emission", 1.0f);
        m_leverCollider.enabled = true;

        if (m_pullCoroutine != null)
        {
            StopCoroutine(m_pullCoroutine);
            m_pullCoroutine = null;
        }
    }
    
    private IEnumerator PullCoroutine()
    {
        SetIsLeverPulledRPC(true);
        m_leverCollider.enabled = false;

        float time = 0f;
        float duration = 2.0f;
        while (time < duration)
        {
            time += 0.1f;
            float t = Mathf.Clamp01(time / duration); // 0 ~ 1

            float emission = Mathf.Lerp(0.1f, 1.0f, t); // 점점 증가
            SetMaterialRpc(emission);

            yield return new WaitForSeconds(0.1f);
        }
        
        m_leverCollider.enabled = true;
        SetMaterialRpc(1.0f);
        
        SetIsLeverPulledRPC(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void SetMaterialRpc(float value) 
    {
        pillarMaterial.SetFloat("_Emission", value);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void SetIsLeverPulledRPC(bool value)
    {
        IsLeverPulled = value;
        m_door.TryOpenDoor();
    }

    private IEnumerator OpenDoorCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        pillarMaterial.SetFloat("_Emission", 0.1f);

        yield return new WaitForSeconds(7.0f);
        pillarMaterial.SetFloat("_Emission", 1.0f);
    }
}
