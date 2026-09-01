using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.AI;
using UnityEngine.Localization;
using UnityEngine.LowLevel;
using _Scripts.UI.Diary;
using Nostal;
using Nostal.Network;
using Nostal.Util;

public class TutorialManager : NetworkBehaviour
{
    private NetworkRunner m_runner => NetworkManager.Instance.Runner;
    
    [SerializeField] private LocalizedString[] m_tutorialDescriptions;
    [SerializeField] private LocalizedString[] m_tutorialGoalDescriptions;
    private bool[] isTextShowed = new bool[12];
    private bool[] isGoalTextShowed = new bool[12];

    [SerializeField] public TutorialChild child;
    //[SerializeField] public MapCreator mapCreator;
    
    //튜토리얼 스테이지 관련 
    [SerializeField] public NetworkObject diaryObjectPrefab;
    private NetworkObject diaryObjectInstance = null;
    [SerializeField] public Transform diarySpawnPosition;
    [SerializeField] public NetworkObject mobObjectPrefab;
    private NetworkObject mobObjectInstance = null;
    [SerializeField] public Transform mobSpawnPosition;
    [SerializeField] public NetworkObject alterObjectPrefab;
    private NetworkObject alterObjectInstance = null;
    [SerializeField] public Transform alterSpawnPosition;
    [Networked] public bool isStageStarted {get; set;} = false;
    [SerializeField] public NetworkObject diarySystemPrefab;
    [SerializeField] private NetworkObject diarySystemInstance;
    [SerializeField] private LevelDiaryContentSO levelDiaryContentSO;
    [SerializeField] public NetworkObject exitDoorPrefab;
    [SerializeField] public Transform exitDoorSpawnPosition;
    private NetworkObject exitDoorInstance = null;

    //플레이어 리셋 포인트
    [SerializeField] public Transform playerResetPosition;
    
    //TutorialStageDoor 문 닫기 위함
    [SerializeField] private TutorialStageDoor tutorialStageDoor;

    //GameOverUI 끄기 위함
    [SerializeField] private GameOverUIController gameOverUIController;
    [SerializeField] private Jumpscare jumpscare;

    public override void Spawned() 
    {
        UIManager.Instance.EnableTutorialUI();
        UIManager.Instance.TutorialUIController.ShowGoalText(m_tutorialGoalDescriptions[0]); // Show the first goal text

        GameManager.Instance.DiarySystem = diarySystemInstance.GetComponent<DiarySystem>();

        if (!HasStateAuthority) 
        {
            return;
        }

        alterObjectInstance = m_runner.Spawn(alterObjectPrefab,
            alterSpawnPosition.position,
            alterSpawnPosition.rotation,
            m_runner.LocalPlayer
        );

        GameManager.Instance.OnGameOver += HideUIRpc;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Debug.Log("Despawned Called");
        
        GameManager.Instance.OnGameOver -= HideUIRpc;
        base.Despawned(runner, hasState);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void HideUIRpc() {
        UIManager.Instance.TutorialUIController.Hide();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ShowUIRpc() {
        UIManager.Instance.TutorialUIController.Show();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ShowTutorialTextRpc(int index) {
        if (isTextShowed[index])
        {
            return;
        }
        SetTutorialTextShowedRpc(index);

        UIManager.Instance.TutorialUIController.ShowText(m_tutorialDescriptions[index]);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ShowTutorialGoalTextRpc(int index) {
        if (isGoalTextShowed[index])
        {
            return;
        }
        SetGoalTextShowedRpc(index);

        UIManager.Instance.TutorialUIController.ShowGoalText(m_tutorialGoalDescriptions[index]);
        SoundManager.Instance.SFX_Play_rpc("tutorialGoalUI");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetTutorialTextShowedRpc(int index) {
        if (index < 0 || index >= isTextShowed.Length) {
            Debug.LogError("Index out of bounds for isTextShowed array: " + index);
            return;
        }
        isTextShowed[index] = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetGoalTextShowedRpc(int index) {
        if (index < 0 || index >= isGoalTextShowed.Length) {
            Debug.LogError("Index out of bounds for isTextShowed array: " + index);
            return;
        }
        isGoalTextShowed[index] = true;
    }

    //이름으로는 진짜 접근하기 싫은디 없앨 게 너무 많다
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void DestroyUIRpc() {
        GameObject diary = GameObject.Find("DiaryCanvas(Clone)");
        if (diary != null) Destroy(diary);

        GameObject score = GameObject.Find("ScoreCanvas(Clone)");
        if (score != null) Destroy(score);

        GameObject playerUI = GameObject.Find("Player UI(Clone)");
        if (playerUI != null) Destroy(playerUI);
        
        GameObject deathUI = GameObject.Find("DeathTimerUI(Clone)");
        if (deathUI != null) Destroy(deathUI);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ResetRpc() {
        GameplayEventManager.OnTutorialMapResetRPC(m_runner);
        
        //각종 UI 없앰
        DestroyUIRpc();

        //diary와 mob 초기화
        if(diaryObjectInstance != null) {
            m_runner.Despawn(diaryObjectInstance);
        }
        if(mobObjectInstance != null) {
            m_runner.Despawn(mobObjectInstance);
        }

        tutorialStageDoor.ResetDoor();
        HideGameOverUIRpc(); // 게임 오버 UI 숨김
        ShowUIRpc();

        // 커서 비활성화
        ResetCameraRpc();

        StartCoroutine(ResetSequence());
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void ResetCameraRpc() {
        CursorController.SetEnableCursor(false);
        if (Camera.main.TryGetComponent<FirstPersonCamera>(out FirstPersonCamera fpc))
        {
            fpc.LockCameraRotate(false);
        }
    }

    public IEnumerator ResetPlayer() {
        //플레이어 위치 초기화
        ResetPlayerRpc();

        //플레이어가 사라질 때까지 대기
        while(GameManager.Instance.FatherNetworkObject != null || GameManager.Instance.DaughterNetworkObject != null) {
            Debug.Log("Waiting for players to despawn... " + GameManager.Instance.FatherNetworkObject + " / " + GameManager.Instance.DaughterNetworkObject);
            yield return null;
        }
        
        GameManager.Instance.PlayerSpawner.PlayerSpawnRpc(GameManager.Instance.FatherPlayerRef, playerResetPosition.position);
        GameManager.Instance.PlayerSpawner.PlayerSpawnRpc(GameManager.Instance.DaughterPlayerRef, playerResetPosition.position);

        //플레이어가 다시 스폰할 때까지 대기
        while(GameManager.Instance.FatherNetworkObject == null || GameManager.Instance.DaughterNetworkObject == null) {
            Debug.Log("Waiting for players to spawn... " + GameManager.Instance.FatherNetworkObject + " / " + GameManager.Instance.DaughterNetworkObject);
            yield return null;
        }
        
        SetDirtSoundRpc();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void SetDirtSoundRpc() {
        GameManager.Instance.GetLocalPlayer().GetComponent<PlayerMovement>().SetDirt(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void ResetPlayerRpc() {
        if(m_runner.LocalPlayer == GameManager.Instance.FatherPlayerRef) {
            m_runner.Despawn(GameManager.Instance.FatherNetworkObject);
        } else if(m_runner.LocalPlayer == GameManager.Instance.DaughterPlayerRef) {
            m_runner.Despawn(GameManager.Instance.DaughterNetworkObject);
        }        
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void HideGameOverUIRpc() {
        gameOverUIController.Hide();
    }

    public void OnBeforeDiarySystemSpawned(NetworkRunner runner, NetworkObject obj) {
        obj.GetComponent<DiarySystem>().SetLevelDiaryContentSO(levelDiaryContentSO);
    }

    public void OnBeforeExitDoorSpawned(NetworkRunner runner, NetworkObject obj) {
        obj.GetComponent<ExitDoor>().nextScene = NostalgiaGameLevel.LevelOne;      
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void StartStageRpc() {
        if(isStageStarted) return;
        isStageStarted = true;

        if(GameManager.Instance.DiarySystem.collectDiaryNum == 0) {
            GameManager.Instance.DiarySystem.GetDiaryRpc();
        }
        
        diaryObjectInstance = m_runner.Spawn(
            diaryObjectPrefab,
            diarySpawnPosition.position,
            diarySpawnPosition.rotation,
            m_runner.LocalPlayer
        );
        mobObjectInstance = m_runner.Spawn(
            mobObjectPrefab,
            mobSpawnPosition.position,
            mobSpawnPosition.rotation,
            m_runner.LocalPlayer
        );
        exitDoorInstance = m_runner.Spawn(
            exitDoorPrefab,
            exitDoorSpawnPosition.position,
            exitDoorSpawnPosition.rotation,
            m_runner.LocalPlayer,
            OnBeforeExitDoorSpawned
        );
    }

    private IEnumerator ResetSequence()
    {
        // 제단 제거 및 Spawn
        if(alterObjectInstance != null) {
            m_runner.Despawn(alterObjectInstance);
            alterObjectInstance = null;
        }
        yield return new WaitForSeconds(0.05f);

        alterObjectInstance = m_runner.Spawn(
            alterObjectPrefab,
            alterSpawnPosition.position,
            alterSpawnPosition.rotation,
            m_runner.LocalPlayer
        );
        yield return new WaitForSeconds(0.05f);

        // diarySystem 초기화
        if(diarySystemInstance != null) {
            m_runner.Despawn(diarySystemInstance);
        }
        diarySystemInstance = m_runner.Spawn(
            diarySystemPrefab,
            Vector3.zero,
            Quaternion.identity,
            m_runner.LocalPlayer,
            OnBeforeDiarySystemSpawned
        );
        yield return new WaitForSeconds(0.05f);

        // 탈출구 제거
        if(exitDoorInstance != null) {
            m_runner.Despawn(exitDoorInstance);
        }

        // 플레이어 리셋
        StartCoroutine(ResetPlayer());

        isStageStarted = false;
    }

    public Jumpscare GetJumpscare() {
        return jumpscare;
    }
}
