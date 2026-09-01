using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using Nostal.Interfaces;
using Unity.Mathematics;

public class DiaryObject : NetworkBehaviour, IInteractable
{
    private bool despawnTrigger = false;
    public bool isTriggered = false;
    public Material material;
    public Renderer mesh_renderer_cover;
    public Renderer mesh_renderer_pile;
    public Renderer mesh_renderer_uvula;

    public ParticleSystem butterfly;
    [SerializeField] private GameObject lightObject;
    
    [Header("Interactable Prompt Data")] 
    [SerializeField] private InteractPromptData m_interactPromptData;

    private Camera m_camera;

    public override void Spawned()
    {
        m_camera = Camera.main;
        
        mesh_renderer_cover.material.SetFloat("_desolve", 0f);  //머터리얼 초기화
        mesh_renderer_pile.material.SetFloat("_desolve", 0f);  //머터리얼 초기화
        mesh_renderer_uvula.material.SetFloat("_desolve", 0f);  //머터리얼 초기화

        //딸이면 일기장 소리 재생
        StartCoroutine(PlayDiarySoundToDaughter());
        //아빠면 조명 키기
        StartCoroutine(LightOnToFather());
    }

    public IEnumerator PlayDiarySoundToDaughter() 
    {
        while(GameManager.Instance == null) {
            yield return null;
        }
        while(GameManager.Instance.GetLocalPlayer() == null || GameManager.Instance.DaughterNetworkObject == null) {
            yield return null;
        }
        if(GameManager.Instance.GetLocalPlayer() == GameManager.Instance.DaughterNetworkObject) {
            Debug.Log("딸 일기장 소리 재생");
            SoundManager.Instance.SFX_loop_Play("diarySound", this.gameObject, 40);
        }

        StartCoroutine(CheckDiarySoundY());
    }

    public IEnumerator CheckDiarySoundY() {
        while(true) {
            bool isOtherFloor = CheckDistanceY();
            if(isOtherFloor) {
                SoundManager.Instance.SFX_Set_Volume("diarySound", this.gameObject, 0.0f);
            }
            else {
                SoundManager.Instance.SFX_Set_Volume("diarySound", this.gameObject, 1.0f);
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    private bool CheckDistanceY() 
    {
        if (!m_camera)
        {
            m_camera = Camera.main;

            if (!m_camera)
            {
                return false;
            }
        }
        
        float distance = math.abs(m_camera.transform.position.y - transform.position.y);
        return distance > 3.0f;
    }

    protected IEnumerator LightOnToFather() 
    {
        while(GameManager.Instance == null) {
            yield return null;
        }
        while(GameManager.Instance.GetLocalPlayer() == null || GameManager.Instance.FatherNetworkObject == null) {
            yield return null;
        }
        if(GameManager.Instance.GetLocalPlayer() == GameManager.Instance.FatherNetworkObject) {
            Debug.Log("아빠 일기장 조명 켜기");
            lightObject.SetActive(true);
        }
    }

    public virtual void OnInteract(NetworkObject playerObject)
    {
        //중복 획득
        if (despawnTrigger || isTriggered) return;

        SetTriggeredRpc();

        //Debug.Log("이거 되고 있냐?");
        GameManager.Instance.DiarySystem.GetDiaryRpc();

        //딸이면 일기장 소리 중지
        SoundManager.Instance.SFX_loop_Stop_rpc("diarySound", this.gameObject.GetComponent<NetworkObject>());
        SoundManager.Instance.SFX_Play_rpc("diaryGet", this.gameObject.GetComponent<NetworkObject>(), 20);

        StartCoroutine(diaryDesolve());
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetTriggeredRpc() 
    {
        isTriggered = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void MT_SetFloatRpc(string variable, float value) 
    {
        mesh_renderer_cover.material.SetFloat(variable, value);
        mesh_renderer_pile.material.SetFloat(variable, value);
        mesh_renderer_uvula.material.SetFloat(variable, value);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void ParticleEffectRpc() 
    {
        isTriggered = true;
        butterfly.Play();
    }

    public InteractPromptData GetInteractPromptData()
    {
        return m_interactPromptData;
    }

    private IEnumerator diaryDesolve(){
        StartCoroutine(cat_desolve_Efffect(0f, 1f, 3f));
        ParticleEffectRpc();
        yield return new WaitForSeconds(4f);

        //이펙트 2초후 디스폰트리거 트루
        Debug.Log("일기장 이펙트 코루틴 종료");
        MT_SetFloatRpc("_desolve", 0f);
        GameManager.Instance.DiarySystem.DestroyDiaryRpc(Object);
    }

    private IEnumerator cat_desolve_Efffect(float startValue, float endValue, float duration) // 1에서 0으로 변화하는 데 걸리는 시간) //애니메이션으로 할라 했는데 파라미터를 못찾겠어서 그냥 스크립트로 구현
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration); 
            MT_SetFloatRpc("_desolve", newValue);
            yield return null;
        }

        // 최종적으로 0으로 설정
        MT_SetFloatRpc("_desolve", endValue);
    }
}
