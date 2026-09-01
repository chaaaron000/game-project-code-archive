using System.Collections;
using System.Collections.Generic;
using _Scripts.Interfaces;
using UnityEngine;
using Fusion;
using UnityEngine.Events;
using Nostal;

public class ChaseMapManager : NetworkBehaviour
{
    #region Singleton Pattern
    
    private static ChaseMapManager instance = null;
    
    /// <summary>
    /// UIManager 싱글톤 구현
    /// </summary>
    public static ChaseMapManager Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }

    #endregion

    private bool isFatherLeverPulled = false;
    private bool isDaughterLeverPulled = false;

    [SerializeField] private GameObject StartDoor;
    [SerializeField] private Transform restartPos;
    [SerializeField] private NetworkObject mobPrefab;
    private NetworkObject mob;
    [SerializeField] private Transform mobPos;
    private bool isChaseStarted = false;
    [SerializeField] private Transform[] mobTarget;
    [SerializeField] private Transform[] wrongPos;

    [SerializeField] private Jumpscare jumpscare;
    private bool isJumpscareEnd = false;

    [SerializeField] private ChaseFinalObject finalObject;
    [SerializeField] private Transform clearPos;
    
    private List<IResettable> m_resettableObjects = new List<IResettable>();
    public List<GameObject> m_debug = new List<GameObject>();

    public UnityEvent OnChaseReset;
    public UnityEvent OnChaseClear;

    public AudioSource chaseBGM_Audiosource;

    public override void Spawned()
    {
        // 싱글톤
        if (instance == null)
        {
            instance = this;
            Debug.Log("ChaseMapManager Instance Created");
            Runner.MakeDontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Runner.DestroySingleton<GameManager>();
            Runner.Despawn(gameObject.GetComponent<NetworkObject>());
            return;
        }

        // 인터페이스를 구현하는 컴포넌트를 가진 게임오브젝트들 가져오기
        MonoBehaviour[] allComponents = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour component in allComponents)
        {
            if (component is not IResettable resettable)
            {
                continue;
            }

            if (!m_debug.Contains(component.gameObject)) // 중복 방지
            {
                m_resettableObjects.Add(resettable);
                m_debug.Add(component.gameObject);
            }
        }

        GameplayEventManager.JumpscareEnded += JumpscareEnd;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        
        GameplayEventManager.JumpscareEnded -= JumpscareEnd;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void PullLeverRpc(bool isFather, bool isPulled)
    {
        if (isFather)
        {
            if(isPulled) {
                isFatherLeverPulled = true;
            }
            else {
                isFatherLeverPulled = false;
            }
        }
        else
        {
            if(isPulled) {
                isDaughterLeverPulled = true;
            }
            else {
                isDaughterLeverPulled = false;
            }
        }
        
        Debug.Log("isFatherLeverPulled: " + isFatherLeverPulled + "/ isDaughterLeverPulled: " + isDaughterLeverPulled);

        if(isDaughterLeverPulled && isFatherLeverPulled) {
            //둘다 레버를 당겼을 때
            StartChase();
        }
    }

    //레버를 건드려서 맵을 추격신을 시작할 때
    public void StartChase()
    {
        chaseBGM_Audiosource.Play();

        if (!HasStateAuthority || isChaseStarted)
        {
            return;
        }
        isChaseStarted = true;


        StartCoroutine(StartChaseC());
    }

    IEnumerator StartChaseC()
    {
        yield return new WaitForSeconds(2f);

        //몹 소환
        mob = Runner.Spawn(mobPrefab, mobPos.position, mobPos.rotation, Runner.LocalPlayer, OnBeforeSpawned);
        mob.GetComponent<ChaseMob>().StartChase();
    }

    public void OnBeforeSpawned(NetworkRunner runner, NetworkObject networkObject)
    {
        //몹 소환
        networkObject.GetComponent<ChaseMob>().target = mobTarget;
        networkObject.GetComponent<ChaseMob>().wrongWay = wrongPos;
    }

    // 사망 이후 조건들 초기화
    public IEnumerator ResetChase(NetworkObject player)
    {
        OnChaseReset?.Invoke();
        GameplayEventManager.OnChaseMapResetRPC(Runner);
        
        if (player == GameManager.Instance.GetLocalPlayer()) 
        {
            StartCoroutine(player.GetComponent<PlayerMovement>().Jumpscare(jumpscare));
        }

        while (!isJumpscareEnd && player != null) 
        {
            yield return null;
        }

        ShowUIRpc();

        if (player != GameManager.Instance.GetLocalPlayer()) 
        {
            UIManager.Instance.FadeView.FadeOut(2);
        }

        if (!HasStateAuthority)
        {
            yield break;
        }
        
        isChaseStarted = false;

        yield return new WaitForSeconds(2);

        // 움직이는 물체들 초기화, 아이템 초기화
        ResetObjectsRPC();
        
        //몹 알고리즘 초기화
        if(mob != null) {
            mob.GetComponent<ChaseMob>().agent.enabled = false;
            Runner.Despawn(mob);
        }
        isJumpscareEnd = false;

        //플레이어 관련 초기화들
        Player father = GameManager.Instance.FatherNetworkObject.GetComponent<Player>();
        Player daughter = GameManager.Instance.DaughterNetworkObject.GetComponent<Player>();

        //스테미너 초기화, 인벤토리 초기화
        father.ClearInventoryRpc();
        daughter.ClearInventoryRpc();
        father.RefillStaminaRpc();
        daughter.RefillStaminaRpc();

        //플레이어 위치 초기화
        father.gameObject.GetComponent<PlayerMovement>().TeleportRpc(restartPos.position);
        daughter.gameObject.GetComponent<PlayerMovement>().TeleportRpc(restartPos.position);

        FadeOutInRpc(false);
    }

    public void JumpscareEnd(PlayerRef playerRef) {
        if (!HasStateAuthority) return;
        Debug.Log("JumpscareEnd Event Called");
        isJumpscareEnd = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ShowUIRpc() {
        UIManager.Instance.PlayerUIController.Show();
        GameManager.Instance.GetLocalPlayer().GetComponent<Player>().PlayerInventory.inventoryUI.Show();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void FadeOutInRpc(bool isOut) {
        if(isOut) {
            UIManager.Instance.FadeView.FadeOut(2);
        }
        else {
            UIManager.Instance.FadeView.FadeIn(2);
        }
    }

    public void SetMobWrongWayTrigger(bool flag)
    {
        if (!HasStateAuthority) return;
        Debug.Log("Manager: SetMobWrongWayTrigger: " + flag);
        mob.GetComponent<ChaseMob>().SetWrongWayTrigger(flag);
    }

    public void Clear() 
    {
        if (HasStateAuthority)
        {
            GameplayEventManager.OnChaseMapClearRPC(Runner);
            InvokeChaseClearRPC();
            GameplayEventManager.OnGameOverRPC(Runner, true);
        }
        
        Player father = GameManager.Instance.FatherNetworkObject.GetComponent<Player>();
        Player daughter = GameManager.Instance.DaughterNetworkObject.GetComponent<Player>();

        father.gameObject.GetComponent<PlayerMovement>().TeleportByItemRPC(clearPos.position, clearPos.rotation);
        daughter.gameObject.GetComponent<PlayerMovement>().TeleportByItemRPC(clearPos.position, clearPos.rotation);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void ResetObjectsRPC()
    {
        foreach (IResettable resettable in m_resettableObjects)
        {
            resettable.Reset();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void InvokeChaseResetRPC()
    {
        OnChaseReset?.Invoke();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void InvokeChaseClearRPC()
    {
        OnChaseClear?.Invoke();
    }
}
