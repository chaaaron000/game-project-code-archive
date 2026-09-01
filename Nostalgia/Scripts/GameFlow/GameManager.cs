using Fusion;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using ExitGames.Client.Photon.StructWrapping;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Fusion.Sockets;
using System;
using System.Threading.Tasks;
using Nostal;
using Nostal.Network;

/*
 * StartGame 로직
     * GameSceneManager의 LoadScene 호출
     * GameSceneManager가 씬 로딩이 끝나면 OnSceneLoadComplete를 Invoke -> void OnSceneLoadCompleted가 실행
     * 만약 씬 인덱스가 0이고 게임이 시작 됐으면(게임이 종료되서 메인 화면으로 돌아가면)
         * true  -> RunnerLeaveGame 호출
         * false -> 스테이지 초기화
     * MapCreator 등등 스테이지가 초기화가 되면 OnReadyLevel를 Invoke -> Fade Out 진행
 */

public class GameManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft, IStateAuthorityChanged  
{
    #region Singleton Pattern
    
    private static GameManager instance = null;
    
    /// <summary>
    /// UIManager 싱글톤 구현
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }
    
    #endregion

    [SerializeField] private bool debugMode = false;
    
    public bool DebugMode => debugMode;

    private NetworkRunner m_runner => NetworkManager.Instance.Runner;

    public GameObject SelectCharacterManagerPrefab = null;
    [SerializeField] private GameSceneManager _gameSceneManager = null;
    
    [Networked] public bool RoomCreatorJoined { get; private set; } = false;
    [Networked] public bool GameStarted { get; private set; } = false;
    [Networked] public bool GameReady { get; private set; } = false;
    
    [Networked] public PlayerRef FatherPlayerRef   { get; set; }
    [Networked] public PlayerRef DaughterPlayerRef { get; set; }
    
    [Networked] public string FatherPlayerName   { get; set; } = "";
    [Networked] public string DaughterPlayerName { get; set; } = "";
    bool _duringJoined = false;

    [Networked] public NetworkObject FatherNetworkObject { get; set; }  = null;
    [Networked] public NetworkObject DaughterNetworkObject { get; set; } = null;

    public PlayerSpawner PlayerSpawner = null;

    //누군가가 죽었는지를 판단하는 bool 변수, true인 상태에서 한번 더 죽으면 게임 종료, 부활 시 false로 변경
    [Networked] public bool _deathFlag {get; set;} = false;
    //누군가가 클리어했는지 판단하는 bool 변수, true인 상태에서 반대쪽이 바로 죽으면 게임 종료
    [Networked] public bool _clearFlag {get; set;} = false;
    private Coroutine _deathTimerCoroutine = null;
    public DiarySystem DiarySystem;
    public ExitDoor _exitDoor;
    public MapCreator _mapCreator;
    [Networked] private Vector3 _spawnPosition {get; set;} = new Vector3(106,15,32);

    [Networked] public int FatherPlayerRefId { get; set; } = -1;
    [Networked] public int DaughterPlayerRefId { get; set; } = -1;
    [SerializeField] RenderPipelineAsset _renderPipelineAsset;

    public bool IsServer {
        get {
            if(HasStateAuthority) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    public int scenePlayerCnt = 0;

    [Header("Death Timer")] 
    public UnityAction<float> OnDeathTimerUpdated;
    
    public UnityAction OnGameOver;
    public static UnityAction OnReadyLevel;
    
    [SerializeField] private float deathTime;
    public float DeathTime
    {
        get => deathTime;
        private set
        {
            deathTime = value;
            OnDeathTimerUpdated?.Invoke(deathTime);
        }
    }

    public bool IsLocalPlayerFather => (m_runner.LocalPlayer == FatherPlayerRef);

    public override void Spawned()
    {
        // Debug.Log("GameManager Spawned");
        //Runner = NetworkRunner.GetRunnerForGameObject(this.gameObject);
        PlayerSpawner = GetComponent<PlayerSpawner>();
        _gameSceneManager = FindObjectOfType<GameSceneManager>();
        
        // 싱글톤
        if (instance == null)
        {
            instance = this;
            m_runner.MakeDontDestroyOnLoad(this.gameObject);
        }
        else
        {
            m_runner.DestroySingleton<GameManager>();
            m_runner.Despawn(gameObject.GetComponent<NetworkObject>());
            return;
        }

        GameSceneManager.OnSceneLoadComplete += OnSceneLoadCompleted;

        PlayerRef player = m_runner.LocalPlayer;

        SetJoinRpc(true);
        // 아직 들어온 사람이 없으면
        if (!RoomCreatorJoined) RoomCreatorJoined = true;
        
        // 어떤 캐릭터를 골랐는지 확인하는 변수
        bool selectFather = false; // true: father, false: daughter
        
        // SelectCharacterManager 받아오는 구문
        if (FindObjectOfType<SelectCharacterManager>() == null)
            m_runner.Spawn(SelectCharacterManagerPrefab);
        else
            FindObjectOfType<SelectCharacterManager>().Spawned();

        // Debug.Log($"{DaughterPlayerRef} {FatherPlayerRef}");

        // 현재 딸 캐릭터가 선택이 안 되있으면 false, 즉 딸을 선택
        selectFather = !(DaughterPlayerRef == PlayerRef.None);
        
        GetSteamNameRpc(player, selectFather);
        SetPlayerRpc(player, selectFather);

        AddCharacterSelectUIButtonListenerRpc(player);
        SelectCharacterManager.Instance.ApplyInfoToUIRpc();
        SetJoinRpc(false);

        if (HasStateAuthority)
            InitializeStageRpc();
    }

    private void OnDestroy()
    {
        GameSceneManager.OnSceneLoadComplete -= OnSceneLoadCompleted;
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SetJoinRpc(bool value) {
        _duringJoined = value;
    }

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("Player Joined");
        Debug.Log(player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        Debug.Log("Player Left");

        if (GameStarted)
        {
            Debug.Log("게임을 종료합니다.");
            BackToMainMenuRpc();
            return;
        }
        
        ChangeGameReadyRpc(false);

        bool isFather;
        
        if (player != PlayerRef.None)
            isFather = player == FatherPlayerRef;
        else
            isFather = !(GetComponent<NetworkObject>().StateAuthority == FatherPlayerRef);

        Debug.Log($"PlayerLeft isFather : {isFather}");
        
        // PlayerRef 초기화
        ResetPlayerRpc(isFather);
        
        // 닉네임 공백 설정
        SetPlayerNameRpc("", isFather);
        UIManager.Instance.CharacterSelectUIController.SetPlayerName();
        
        // 레디 해제
        if (isFather)
            SelectCharacterManager.Instance._fatherPlayerReady = false;
        else
            SelectCharacterManager.Instance._daughterPlayerReady = false;
        
        UIManager.Instance.MainMenuUIController.UpdateRoomUIRpc();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SetPlayerRpc(PlayerRef player, bool isFather)
    {
        // Debug.Log("SetPlayerRpc: " + player + isFather);
        if(isFather == true)
        {
            FatherPlayerRef = player;
            FatherPlayerRefId = player.PlayerId;
        }
        else
        {
            DaughterPlayerRef = player;
            DaughterPlayerRefId = player.PlayerId;
        }
        SelectCharacterManager.Instance.ApplyInfoToUIRpc();
        
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ResetPlayerRpc(bool isFather)
    {
        if(isFather)
        {
            FatherPlayerRef = PlayerRef.None;
            FatherPlayerRefId = -1;
        }
        else
        {
            DaughterPlayerRef = PlayerRef.None;
            DaughterPlayerRefId = -1;
        }
        
        // SelectCharacterManager.Instance.ApplyInfoToUIRpc();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void GetSteamNameRpc([RpcTarget] PlayerRef player, bool isFather)
    {
        if(!SteamManager.Initialized)
        {
            Debug.Log("SteamManager is not initialized");
            SetPlayerNameRpc("NoName", isFather);
            return;
        }
        
        //returns my steam name(id)
        string name = SteamFriends.GetPersonaName();
        SetPlayerNameRpc(name, isFather);
        return;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SetPlayerNameRpc(string name, bool isFather)
    {
        if (isFather)
        {
            FatherPlayerName = name;
        }
        else
        {
            DaughterPlayerName = name;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void SwapPlayerInfoRpc()
    {
        Debug.Log("SwapPlayerInfoRpc");
        // 사람 들어오는 도중 및 권한이 없으면 안 바뀌도록
        if(_duringJoined == true && HasStateAuthority == false) return;
        (DaughterPlayerRef,  FatherPlayerRef)  = (FatherPlayerRef,  DaughterPlayerRef);
        (DaughterPlayerName, FatherPlayerName) = (FatherPlayerName, DaughterPlayerName);
    }

    public void Clear(NostalgiaGameLevel nextScene)
    {
        if (HasStateAuthority)
        {
            GameplayEventManager.OnGameOverRPC(m_runner, true);
        }
        
        // 다음 씬 시작
        StartGame(nextScene);
    }

    public void StartGame(NostalgiaGameLevel scene)
    {
        GameStarted = true;
        retryCount = 0;

        ResetUIRpc();
        
        if (!m_runner.IsSceneAuthority) return;

        if (_gameSceneManager == null)
            _gameSceneManager = FindObjectOfType<GameSceneManager>();

        _gameSceneManager.LoadScene(scene);
    }
 
    private bool retryFatherFlag = false;
    private bool retryDaughterFlag = false;
    [Networked, OnChangedRender(nameof(OnRetryCountChanged))] public int retryCount {get; set;} = 0;
    public static event UnityAction<int> OnRetryCountChangedEvent;

    private void OnRetryCountChanged()
    {
        OnRetryCountChangedEvent?.Invoke(retryCount);
    }

    public void TryRetry() {
        Debug.Log("TryRetry called by LocalPlayer: " + Runner.LocalPlayer);
        RetryRpc(Runner.LocalPlayer);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RetryRpc(PlayerRef player) {
        if(player == FatherPlayerRef && retryFatherFlag == false) {
            Debug.Log("RetryRpc: Father Player Ref");
            retryFatherFlag = true;
            retryCount++;
        }
        else if(player == DaughterPlayerRef && retryDaughterFlag == false) {
            Debug.Log("RetryRpc: Daugther Player Ref");
            retryDaughterFlag = true;
            retryCount++;
        }

        if(retryCount == 2) {
            RetryGame();
        }
    }

    /// <summary>
    /// 게임 재시도를 위한 메소드입니다.
    /// RetryGameCoroutine를 실행합니다.
    /// </summary>
    public void RetryGame()
    {
        retryDaughterFlag = false;
        retryFatherFlag = false;
        retryCount = 0;

        if (SceneManager.GetActiveScene().name == "TutorialS") {
            // 튜토리얼 씬에서는 게임 재시도 시 씬을 리셋하지 않음
            ResetUIRpc();
            FindObjectOfType<TutorialManager>().ResetRpc();
            InitializeStageRpc();
            Debug.Log("RetryGame: Tutorial Scene, Reset UI and TutorialManager");
        }
        else if (SceneManager.GetActiveScene().name == "main_chaseScene") {
            // 메인 추격 씬에서는 게임 재시도 시 씬을 리셋하지 않음
            ResetUIRpc();
            StartCoroutine(ChaseMapManager.Instance.ResetChase(null));
        }
        else {
            ResetUIRpc();
            _gameSceneManager.ReloadCurrentScene();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ResetUIRpc() {
        UIManager.Instance.IsGameOver = false;
        UIManager.Instance.Clear();
    }

    // [Rpc(RpcSources.All, RpcTargets.All)]
    // public void MakeLoadingScreenRpc() {
    //     UIManager.Instance.EnableLoadingUI();
    // }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RemoveLoadingScreenRpc()
    {
        OnReadyLevel?.Invoke();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void BackToMainMenuRpc()
    {
        StartCoroutine(BackToMainMenuCoroutine());
    }

    private IEnumerator BackToMainMenuCoroutine()
    {
        yield return new WaitUntil(() => m_runner.IsSceneAuthority && _gameSceneManager.HasStateAuthority);
        _gameSceneManager.LoadScene(NostalgiaGameLevel.MainMenu);
    }
    
    private IEnumerator InitializeStageCoroutine()
    {
        if (!HasStateAuthority)
        {
            yield break;
        }
        
        string activeSceneName = SceneManager.GetActiveScene().name;

        // flag 초기화
        _deathFlag = false;
        _clearFlag = false;

        // 게임 씬인지 파악하고 열어야함, 튜토리얼씬, 엔딩 씬에서는 실행 x
        if (activeSceneName != "TutorialS" && activeSceneName != "ending_test" && activeSceneName != "main_chaseScene")
        {
            _mapCreator = FindObjectOfType<MapCreator>();
            // StartCoroutine(_mapCreator.CreateMap());
            // _mapCreator.SetActiveTilesRpc();
            //여기에 원래 urphidelity 넘기기
            ChangeRPARpc(false);
        }
        //튜토리얼 씬
        else if (activeSceneName == "TutorialS"){
            _spawnPosition = new Vector3(106,15,32);
            PlayerSpawner.PlayerSpawnRpc(FatherPlayerRef, new Vector3(107,15,32));
            PlayerSpawner.PlayerSpawnRpc(DaughterPlayerRef, new Vector3(106,15,32));
            ChangeRPARpc(true);
        }
        //엔딩 씬
        else if (activeSceneName == "ending_test") {
            //PlayerSpawner.PlayerSpawnRpc(FatherPlayerRef, new Vector3(0,10,0));
            //PlayerSpawner.PlayerSpawnRpc(DaughterPlayerRef, new Vector3(1,10,0));
            //필요 기능

            yield return new WaitForSeconds(3f);
            // 로딩창 가림막 끄는 rpc 쏘기
            RemoveLoadingScreenRpc();

            if (HasStateAuthority)
                InitializeStageRpc();
            
            yield break;
        }
        else if (activeSceneName == "main_chaseScene"){
            _spawnPosition = new Vector3(-27.5f,7.4f,0f);
            PlayerSpawner.PlayerSpawnRpc(FatherPlayerRef, new Vector3(-27.5f,7.4f,0f));
            PlayerSpawner.PlayerSpawnRpc(DaughterPlayerRef, new Vector3(-28.5f,7.4f,0f));
        }

        yield return StartCoroutine(CheckSceneLoading(activeSceneName));

        FatherNetworkObject.GetComponent<PlayerMovement>().SetCanMoveRpc(true);
        DaughterNetworkObject.GetComponent<PlayerMovement>().SetCanMoveRpc(true);

        // 시작 위치로 한번 더 텔포 시키기
        InitializePositionRpc();
        // 로딩창 가림막 끄는 rpc 쏘기
        RemoveLoadingScreenRpc();
        
        if (HasStateAuthority)
            InitializeStageRpc();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void InitializeStageRpc() {
        // Stage 초기화
        DiarySystem = FindObjectOfType<DiarySystem>();
        _exitDoor = FindObjectOfType<ExitDoor>();
        if(_mapCreator == null) {
            _mapCreator = FindObjectOfType<MapCreator>();
        }
        Debug.Log("InitializeStageRpc : _deathFlag = " + _deathFlag);
        _deathFlag = false;
    }

    public string GetSessionName()
    {
        if(m_runner.SessionInfo.IsValid)
            return m_runner.SessionInfo.Name;
        else
        {
            Debug.LogError("Can't found Session");
            return null;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ChangeRPARpc(bool isTutorial)
    {
        QualitySettings.renderPipeline = isTutorial ? null : _renderPipelineAsset;
    }

    /// <summary>
    /// <para>캐릭터 선택 UI 생성을 위한 Rpc 메소드</para>
    /// - RPC를 호출하여 플레이어마다 UI를 로컬로 생성하도록 함
    /// </summary>
    /// <param name="targetPlayer">PlayerRef, 캐릭터 선택 UI를 생성할 플레이어</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void EnableCharacterSelectUIRpc([RpcTarget] PlayerRef targetPlayer)
    {
        UIManager.Instance.EnableCharacterSelectUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void AddCharacterSelectUIButtonListenerRpc([RpcTarget] PlayerRef targetPlayer)
    {
        UIManager.Instance.CharacterSelectUIController.AddButtonListener(HasStateAuthority);
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SetNetworkObjectRpc(bool isFather, NetworkObject networkObject)
    {
        Debug.Log("SetNetworkObject: " + isFather + " " + networkObject);
        if (isFather)
        {
            FatherNetworkObject = networkObject;
        }
        else
        {
            DaughterNetworkObject = networkObject;
        }
    }

    //LocalPlayer의 PlayerObject를 반환해주는 함수
    public NetworkObject GetLocalPlayer()
    {
        PlayerRef player = m_runner.LocalPlayer;
        if (player == FatherPlayerRef)
        {
            return FatherNetworkObject;
        }
        else if (player == DaughterPlayerRef)
        {
            return DaughterNetworkObject;
        }
        else {
            return null;
        }
    }

    //LocalPlayer가 아닌 상대방의 PlayerObject를 반환해주는 함수
    public NetworkObject GetOtherPlayer()
    {
        PlayerRef player = m_runner.LocalPlayer;
        // Debug.Log("LocalPlayer: " + player);
        // Debug.Log("fatherPlayer: " + FatherPlayerRef);
        // Debug.Log("father same: " + (FatherPlayerRef == player));
        // Debug.Log("daughterPlayer: " + DaughterPlayerRef);
        // Debug.Log("daughter same: " + (DaughterPlayerRef == player));
        if (player == FatherPlayerRef)
        {
            return DaughterNetworkObject;
        }
        else if (player == DaughterPlayerRef)
        {
            return FatherNetworkObject;
        }
        else {
            return null;
        }
    }

    //NetworkObject를 받아서 상대방의 NetworkObject를 반환해주는 함수
    public NetworkObject GetOtherPlayer(NetworkObject playerObject)
    {
        if (playerObject == FatherNetworkObject)
        {
            return DaughterNetworkObject;
        }
        else if (playerObject == DaughterNetworkObject)
        {
            return FatherNetworkObject;
        }
        else {
            return null;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DeathEventRpc() {
        // 이미 한 플레이어가 죽은 상태 / 탈출 상태면 게임 종료
        if(_clearFlag) {
            GameOverRpc();
            return;
        }
        if(_deathFlag)
        {
            Debug.Log("DeathEventRpc: _deathFlag : " + _deathFlag);
            StopDeathTimerRpc();
            GameOverRpc();
            return;
        }
        
        _deathFlag = true;
        StartDeathTimerRpc();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ClearEventRpc() {
        _clearFlag = true;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void StartDeathTimerRpc() {
        // 모든 플레이어에게 Death Timer UI를 활성화
        UIManager.Instance.EnableDeathTimerUI();
        this.StartDeathTimer();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void StopDeathTimerRpc()
    {
        if (_deathTimerCoroutine != null)
        {
            StopCoroutine(_deathTimerCoroutine);
            _deathTimerCoroutine = null;
        }

        if (m_pauseDeathTimerCoroutine != null)
        {
            StopCoroutine(m_pauseDeathTimerCoroutine);
            m_pauseDeathTimerCoroutine = null;

            GameplayEventManager.OnStoperItemEndedRPC(m_runner);
        }

        UIManager.Instance.DisableDeathTimerUI();
    }

    // 게임 오버 rpc 중복 실행되는 현상 해결하기 (flag로?)
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void GameOverRpc() {
        // 게임 종료
        // Debug.Log("Game Over");
        if (UIManager.Instance.IsUIOn())
            UIManager.Instance.Clear();

        Player playerObject = GetLocalPlayer().GetComponent<Player>();
        playerObject.PlayerInventory.inventoryUI.Hide();

        UIManager.Instance.IsGameOver = true;
        StopDeathTimerRpc();
        OnGameOver?.Invoke();

        if (m_runner.IsSharedModeMasterClient)
        {
            GameplayEventManager.OnGameOverRPC(m_runner, false);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ReviveEventRpc() {
        _deathFlag = false;

        if (HasStateAuthority)
            StopDeathTimerRpc();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SetSpawnPositionRpc(Vector3 pos) {
        _spawnPosition = pos;
    }

    public Vector3 GetSpawnPosition() {
        Debug.Log("GetSpawnPosition "+  _spawnPosition);
        return _spawnPosition;
    }

    public Jumpscare GetJumpscare(int index) 
    {
        if (SceneManager.GetActiveScene().name == "TutorialS") 
        {
            // 튜토리얼 씬에서는 점프스케어가 없음
            return FindObjectOfType<TutorialManager>().GetJumpscare();
        }
        
        return _mapCreator.GetJumpscare(index);
    }   
    
    public void StateAuthorityChanged()
    {
        Debug.Log($"StateAuthorityChanged()\nHasStateAuthority : {GetComponent<NetworkObject>().StateAuthority}");
        PlayerLeft(PlayerRef.None);
    }

    private Coroutine m_pauseDeathTimerCoroutine;
    private float m_currentDeathTime;
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void PauseDeathTimerRpc(float pauseDuration)
    {
		// DeathTimer가 작동 중이지 않음
        if (_deathTimerCoroutine == null)
        {
            return;
        }

        if(HasStateAuthority)
            GameplayEventManager.OnStoperItemStartedRPC(m_runner);

        if (m_pauseDeathTimerCoroutine != null)
        {
            StopCoroutine(m_pauseDeathTimerCoroutine);
            m_pauseDeathTimerCoroutine = null;
        }
        // 현재 DeathTime을 저장
        m_currentDeathTime = DeathTime;

        m_pauseDeathTimerCoroutine = StartCoroutine(PauseDeathTimerCoroutine(pauseDuration));
    }

    private IEnumerator PauseDeathTimerCoroutine(float pauseDuration)
    {
        float pauseTimer = 0f;
        while (pauseTimer < pauseDuration)
        {
            DeathTime = m_currentDeathTime;
            pauseTimer += Time.deltaTime;
            yield return null;
        }

        if(HasStateAuthority)
            GameplayEventManager.OnStoperItemEndedRPC(m_runner);
        m_pauseDeathTimerCoroutine = null;
    }
    
    /// <summary>
    /// 데스 타이머 코루틴을 실행하는 메소드입니다.
    /// </summary>
    private void StartDeathTimer()
    {
        _deathTimerCoroutine = StartCoroutine(StartDeathTimerCoroutine());
    }
    
    private IEnumerator StartDeathTimerCoroutine() {
        DeathTime = 180.0f;
        
        // _deathFlag가 false로 바꾸면 죽은 플레이어가 부활한 것으로 간주, 타이머 중지
        while (DeathTime > 0 && this._deathFlag) 
        {
            DeathTime -= Time.deltaTime;
            yield return null;
        }
        
        // 시간이 다 되었을 때
        if (DeathTime <= 0) 
        {
            this.GameOverRpc();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ChangeGameReadyRpc(bool value) {
        GameReady = value;
    }

    private IEnumerator CheckSceneLoading(string activeSceneName)
    {
        int cnt = 0;
        while(true){
            if (cnt <= 10) {
                cnt++;
                yield return new WaitForSeconds(1.0f);
                continue;
            }
            
            if (cnt > 60) {
                Debug.LogError("60초 동안 씬이 로딩되지 않았습니다. 재로딩합니다.", this);
                _gameSceneManager.ReloadCurrentScene();
            }
            
            if (activeSceneName == "TutorialS") {
                if(FatherNetworkObject == null || DaughterNetworkObject == null) {
                    cnt++;
                    yield return new WaitForSeconds(1.0f); 
                }
                else {
                    FindObjectOfType<TutorialManager>().ShowTutorialTextRpc(0);
                    break;
                }
            }
            else if (activeSceneName == "main_chaseScene") {
                if(FatherNetworkObject == null || DaughterNetworkObject == null) {
                    cnt++;
                    yield return new WaitForSeconds(1.0f); 
                }
                else {
                    break;
                }
            }
            else {
                if (FatherNetworkObject == null || DaughterNetworkObject == null || _mapCreator.bMapCreated == false) {
                    cnt++;
                    yield return new WaitForSeconds(1.0f); 
                }
                else {
                    break;
                }
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void InitializePositionRpc() {
        FatherNetworkObject.GetComponent<PlayerMovement>().InitializePositionRpc();
        DaughterNetworkObject.GetComponent<PlayerMovement>().InitializePositionRpc();
    }
    
    private void OnSceneLoadCompleted(int sceneIndex)
    {
        // 메인 메뉴로 돌아갔을 때 RunnerLeaveGame 호출
        if (sceneIndex == 0 && GameStarted)
        {
            Task leave = NetworkManager.Instance.RunnerLeaveGame();
            return;
        }

        StartCoroutine(InitializeStageCoroutine());
        SoundManager.Instance.ResetSoundObjectRpc(SoundManager.Instance.GetComponent<NetworkObject>());
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SpawnNetworkObjectRpc(NetworkPrefabRef prefabRef, Vector3 position = default,
                                      Quaternion    rotation = default)
    {
        this.m_runner.Spawn(prefabRef, position, rotation);
    }
}
