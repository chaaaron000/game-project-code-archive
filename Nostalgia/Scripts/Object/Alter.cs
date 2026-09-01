using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.VFX;
using UnityEngine.UIElements;
using Nostal.Interfaces;

public class Alter : NetworkBehaviour, IInteractable
{
    public Transform alterPosition;

    public VisualEffect vfx;  // VFX Graph가 연결된 오브젝트

    public GameObject catObject;

    [Header("alter Material")]
    public Material base_material;
    public Material cat_material;

    [Header("girl Material")]
    public Material girl_outfit_material;
    public Material girl_face_material;
    public Material girl_hair_material;

    [Header("Man Material")]
    public Material Man_outfit_material;
    public Material Man_face_material;
    public Material Man_hair_material;
    
    [Header("Interactable Prompt Data")] 
    [SerializeField] private InteractPromptData m_interactPromptData;

    //디버깅 용, spawn할때 출력
    public override void Spawned()
    {
        Debug.Log($"{gameObject.name} Spawned on {Runner.LocalPlayer} (HasStateAuthority: {HasStateAuthority})");
    }

    public void OnInteract(NetworkObject playerObject)
    {
        if(GameManager.Instance._deathFlag == false) return;
        else {
            Debug.Log("Interacted with Alter");
            DisableColliderRpc();
            Player targetPlayer = GameManager.Instance.GetOtherPlayer(playerObject).GetComponent<Player>();
            
            SoundManager.Instance.SFX_Play_rpc("usingAltar");
            if(GameManager.Instance.GetOtherPlayer(playerObject).gameObject.TryGetComponent<Father>(out Father temp) == true){
                //제단을 상호작용한 플레이어가 딸일 경우
                targetPlayer.ReviveRpc(alterPosition.position);
                StartCoroutine(Man_respawnEffect());
            }
            else{
                //제단을 상호작용한 플레이어가 아빠일 경우
                targetPlayer.ReviveRpc(alterPosition.position);
                StartCoroutine(girl_respawnEffect());
            }
        }   
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void DisableColliderRpc() {
        gameObject.GetComponent<Collider>().enabled = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void MT_SetFloatRpc(int materialIndex, string variable, float value) 
    {
        Material targetMaterial = GetMaterialByIndex(materialIndex);
        if (targetMaterial != null)
        {
            targetMaterial.SetFloat(variable, value);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetCatActiveRpc() 
    {
        catObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public InteractPromptData GetInteractPromptData()
    {
        return m_interactPromptData;
    }

    private Material GetMaterialByIndex(int index)
    {
        switch (index)
        {
            case 0: return girl_outfit_material;
            case 1: return girl_face_material;
            case 2: return girl_hair_material;
            case 3: return Man_outfit_material;
            case 4: return Man_face_material;
            case 5: return Man_hair_material;
            case 6: return base_material;
            case 7: return cat_material;
            default: return null;
        }
    }

    //딸 머터리얼 변수 세팅
    private void SetGirlMaterial(){
        MT_SetFloatRpc(0,"_desolve", 1f);
        MT_SetFloatRpc(1,"_desolve", 1f);
        MT_SetFloatRpc(2,"_desolve", 1f);
    }

    //아빠 머터리얼 변수 세팅
    private void SetManMaterial(){
        MT_SetFloatRpc(3,"_desolve", 1f);
        MT_SetFloatRpc(4,"_desolve", 1f);
        MT_SetFloatRpc(5,"_desolve", 1f);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void VfxEventRpc(string eventName)
    {
        Debug.Log("alterVFX Start");
        vfx.SendEvent(eventName);
    }

    //딸 부활 이펙트 코루틴
    private IEnumerator girl_respawnEffect(){
        SetGirlMaterial();          //딸 머터리얼 변수 초기화

        StartCoroutine(alter_glow_Efffect(1f, 0.01f, 4f));  //제단 글로우 이펙트
        yield return new WaitForSeconds(5f);             

        VfxEventRpc("OnPlay");                            //파티클 이펙트 시작
        StartCoroutine(cat_desolve_Efffect(0f, 1f, 3f));    //고양이 디졸브 이펙트 시작
        yield return new WaitForSeconds(3f);

        StartCoroutine(girl_spawn_Efffect(1f, 0f, 3f));     //딸 부활 이펙트 시작
        VfxEventRpc("OnStop");                            //파티클 이펙트 종료

        StartCoroutine(alter_glow_Efffect(0.01f, 1f, 2f));  //제단 글로우 이펙트 종료
        yield return new WaitForSeconds(3f);

        MT_SetFloatRpc(7, "_desolve", 0f);       //고양이 다시 생성
        SetCatActiveRpc();                         //현재 이 제단의 고양이만 비활성화
    }

    //아빠 부활 이펙트 코루틴
    private IEnumerator Man_respawnEffect(){
        SetManMaterial();

        StartCoroutine(alter_glow_Efffect(1f, 0.01f, 4f));
        yield return new WaitForSeconds(5f);

        VfxEventRpc("OnPlay");
        StartCoroutine(cat_desolve_Efffect(0f, 1f, 3f));
        yield return new WaitForSeconds(3f);

        StartCoroutine(Man_spawn_Efffect(1f, 0f, 3f));
        VfxEventRpc("OnStop");

        StartCoroutine(alter_glow_Efffect(0.01f, 1f, 2f));
        yield return new WaitForSeconds(3f);

        MT_SetFloatRpc(7, "_desolve", 0f);       
        SetCatActiveRpc();                       
    }



    private IEnumerator alter_glow_Efffect(float startValue, float endValue, float duration) // 1에서 0으로 변화하는 데 걸리는 시간) //애니메이션으로 할라 했는데 파라미터를 못찾겠어서 그냥 스크립트로 구현
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration); 
            MT_SetFloatRpc(6,"_Emission", newValue);
            yield return null;
        }

        // 최종적으로 0으로 설정
        MT_SetFloatRpc(6, "_Emission", endValue);
    }

    private IEnumerator cat_desolve_Efffect(float startValue, float endValue, float duration) // 1에서 0으로 변화하는 데 걸리는 시간) //애니메이션으로 할라 했는데 파라미터를 못찾겠어서 그냥 스크립트로 구현
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration); 
            MT_SetFloatRpc(7, "_desolve", newValue);
            yield return null;
        }

        // 최종적으로 0으로 설정
        MT_SetFloatRpc(7, "_desolve", endValue);
    }

    private IEnumerator girl_spawn_Efffect(float startValue, float endValue, float duration) // 1에서 0으로 변화하는 데 걸리는 시간) //애니메이션으로 할라 했는데 파라미터를 못찾겠어서 그냥 스크립트로 구현
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration); 
            MT_SetFloatRpc(0, "_desolve", newValue);
            MT_SetFloatRpc(1, "_desolve", newValue);
            MT_SetFloatRpc(2, "_desolve", newValue);
            yield return null;
        }

        // 최종적으로 0으로 설정
        MT_SetFloatRpc(0, "_desolve", endValue);
        MT_SetFloatRpc(1, "_desolve", endValue);
        MT_SetFloatRpc(2, "_desolve", endValue);
    }

    private IEnumerator Man_spawn_Efffect(float startValue, float endValue, float duration) // 1에서 0으로 변화하는 데 걸리는 시간) //애니메이션으로 할라 했는데 파라미터를 못찾겠어서 그냥 스크립트로 구현
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration); 
            MT_SetFloatRpc(3, "_desolve", newValue);
            MT_SetFloatRpc(4, "_desolve", newValue);
            MT_SetFloatRpc(5, "_desolve", newValue);
            yield return null;
        }

        // 최종적으로 0으로 설정
        MT_SetFloatRpc(3, "_desolve", endValue);
        MT_SetFloatRpc(4, "_desolve", endValue);
        MT_SetFloatRpc(5, "_desolve", endValue);
    }
}
