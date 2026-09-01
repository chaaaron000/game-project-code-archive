using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nostal.Network
{
    public class NetworkManager : Util.Singleton<NetworkManager>, INetworkRunnerCallbacks
    {
        public NetworkRunner Runner { get; private set; }
        public bool LobbyJoined { get; private set; }

        [SerializeField] private string m_lobbyID;
        [SerializeField] private NetworkRunner NetworkRunnerPrefab;
        [SerializeField] private List<NetworkPrefabRef> m_networkedMangers;

        private NetworkSceneManagerDefault m_sceneManager;
        private bool m_bIsRunnerValid;
        private List<SessionInfo> m_sessionList;

        private GameObject m_sessionNotFoundUI;
        private const float SESSION_NOT_FOUND_OFF = 3f;
        private float m_sessionNotFoundTimer;
        private FusionAppSettings appSettings;
        private string region = null; 
        //public TMP_Dropdown RegionDropdown;
        public ServerSelect ServerSelect;

        //로비에 접속시 sessionList를 받았는지 검사할 용도
        private bool _isSessionUpdated = false;
        
        protected override void Awake()
        {
            base.Awake();

            m_bIsRunnerValid = false;
            LobbyJoined = false;

            m_sessionNotFoundUI = GameObject.Find("NotFoundMessage");
            m_sessionNotFoundUI.SetActive(false);
            m_sessionNotFoundTimer = float.MaxValue;

            // GameObject serverdropdownObject = GameObject.Find("RegionDropdown");
            // ServerSelect = serverdropdownObject.GetComponent<ServerSelect>();
        }

        public void RefreshServerSelect(ServerSelect serverSelect = null) {
            ServerSelect = serverSelect;

            StartCoroutine(InitBestRegionCo());
        }

        private IEnumerator InitBestRegionCo()
        {
            var task = ServerSelect.FindBestRegionAsync();
            while (!task.IsCompleted) yield return null;

            LobbyJoined = task.Status == TaskStatus.RanToCompletion && task.Result;
        }

        private void Update()
        {
            if (!m_bIsRunnerValid)
            {
                DestroyNetworkRunner();
                InitNetworkRunner();
            }

            if (m_sessionNotFoundUI == null)
            {
                m_sessionNotFoundUI = GameObject.Find("NotFoundMessage");
            }

            if (m_sessionNotFoundTimer < SESSION_NOT_FOUND_OFF)
            {
                m_sessionNotFoundUI?.SetActive(true);
                m_sessionNotFoundTimer += Time.deltaTime;
                UIManager.Instance.MainMenuUIController.JoinPanelButton.interactable = true;
            }
            else
            {
                m_sessionNotFoundUI?.SetActive(false);
            }
        }

        public async Task RunnerCreateGame() 
        {
            //현재 지역에 따라 lobby 들어가기 (이미 들어가있으면 나가는 기능 추가 필요)
            await JoinLobby();
            
            // 2) 중복 없는 이름 뽑기(현재 스냅샷 기준)
            string sessionName = RandomSessionNameGenerator.GenerateRandomRoomName();
            while(FindGameSession(sessionName)) 
            {
                sessionName = RandomSessionNameGenerator.GenerateRandomRoomName();
            }

            Task<bool> clientTask = StartGame(sessionName);
            await clientTask;

            if (!clientTask.Result)
            {
                Debug.LogError("CreateGame - FAILED", this);
                return;
            }

            SpawnNetworkedManagers();
            UIManager.Instance.MainMenuUIController.SwitchToCamera2();
            
            // Vivox 채널을 sessionName을 사용해 참가
            VivoxManager.Instance.JoinPositionalChannelAsync(sessionName);
        }
        
        public async Task RunnerJoinGame(string sessionName) 
        {
            //현재 지역에 따라 lobby 들어가기 (이미 들어가있으면 나가는 기능 추가 필요)
            await JoinLobby();

            // 세션 리스트가 첫 1회 업데이트 될 때까지 대기
            while(!_isSessionUpdated) 
            {
                await Task.Delay(100); // 100ms 대기
            }

            // 세션 존재를 일정 시간 재시도하며 확인 (리스트는 점진적으로 채워질 수 있음)
            if (!FindGameSession(sessionName)) {
                m_sessionNotFoundTimer = 0f;
                return;
            }
            
            Task<bool> clientTask = StartGame(sessionName);
            await clientTask;

            if (!clientTask.Result)
            {
                Debug.LogError("JoinGame - FAILED", this);
                return;
            }
            
            // NetworkedManagerSpawner.SpawnNetworkedManager(true);
            UIManager.Instance.MainMenuUIController.SwitchToCamera2();
            
            // Vivox 채널을 sessionName을 사용해 참가
            VivoxManager.Instance.JoinPositionalChannelAsync(sessionName);
        }
        
        public async Task RunnerLeaveGame()
        {
            Debug.Log("RunnerLeaveGame");
            await Runner.Shutdown(true, ShutdownReason.Ok, false);
        }

        private void DestroyNetworkRunner()
        {
            if (Runner != null)
            {
                Runner.RemoveCallbacks(this);
                Destroy(Runner.gameObject);
                Runner = null;
            }
        }

        private void InitNetworkRunner()
        {
            Runner = Instantiate(NetworkRunnerPrefab);
            DontDestroyOnLoad(Runner);

            Runner.name = "NetworkRunner";
            Runner.AddCallbacks(this);

            m_bIsRunnerValid = true;
            
            //처음 시작할 때는 지역을 가장 최선의 지역으로 초기화
            if(!LobbyJoined)
                StartCoroutine(InitBestRegionCo());
        }
        
        private async Task JoinLobby()
        {
            //sessionUpdate 되었는지 초기화
            _isSessionUpdated = false;

            Debug.Log("Region JoinLobby : " + region);
            if(region != null) {
                appSettings = BuildCustomAppSetting(region);
                Debug.Log("Setting region to: " + region);
                Debug.Log("appsettings: " + appSettings.FixedRegion);
            }
            else
            {
                appSettings = BuildCustomAppSetting(PhotonAppSettings.Global.AppSettings.FixedRegion);
            }
        
            // lobbyId를 통해 SessionLobby에 참가
            StartGameResult result = await Runner.JoinSessionLobby(SessionLobby.Shared, m_lobbyID, customAppSettings:appSettings);
        
            // 정상적으로 참가되었는지 확인
            if (result.Ok)
            {
                Debug.Log($"JoinLobby - SUCCESS: {m_lobbyID}", this);
                //LobbyJoined = true;
            }
            else 
            {
                Debug.LogError($"JoinLobby - FAILED: {result.ErrorMessage}", this);
            }
        }
        
        private bool FindGameSession(string sessionName)
        {
            if(m_sessionList == null) {
                return false;
            }

            return m_sessionList.Any(session => session.Name == sessionName);
        }

        private async Task<bool> StartGame(string sessionName)
        {
            Runner.ProvideInput = true;

            if (!m_sceneManager)
            {
                m_sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();
            }
            
            StartGameResult result = await Runner.StartGame(new StartGameArgs
            {
                Address = NetAddress.Any(),
                ConnectionToken = SteamTicketManager.Instance.GetToken(), 
                GameMode = GameMode.Shared,
                IsOpen = true,
                IsVisible = true,
                // CustomLobbyName = lobbyId,
                Scene = SceneRef.FromIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex), 
                SceneManager = m_sceneManager,
                SessionName = sessionName,
                CustomPhotonAppSettings = appSettings,
            });

            if (result.Ok)
            {
                Debug.Log("StartGame - SUCCESS", this);
                return true;
            }
            else
            {
                Debug.LogError($"StartGame - FAILED: {result.ShutdownReason}", this);
                return false;
            }
        }

        private void SpawnNetworkedManagers()
        {
            foreach (NetworkPrefabRef manager in m_networkedMangers)
            {
                Runner.Spawn(manager);
            }
        }

        public void SetRegion(string region) {
            this.region = region;

            if(m_bIsRunnerValid) {
                m_bIsRunnerValid = false; // Reset the runner validity
            }
        }

        private FusionAppSettings BuildCustomAppSetting(string region, string customAppID = null, string appVersion = "1.0.0") {

            var appSettings = PhotonAppSettings.Global.AppSettings.GetCopy();;

            appSettings.UseNameServer = true;
            appSettings.AppVersion = appVersion;

            if (string.IsNullOrEmpty(customAppID) == false) {
                appSettings.AppIdFusion = customAppID;
            }

            if (string.IsNullOrEmpty(region) == false) {
                appSettings.FixedRegion = region.ToLower();

                //m_bIsRunnerValid = false; // Reset the runner validity
            }
            else
            {
                appSettings.FixedRegion = PhotonAppSettings.Global.AppSettings.FixedRegion; 
            }

            // If the Region is set to China (CN),
            // the Name Server will be automatically changed to the right one
            // appSettings.Server = "ns.photonengine.cn";

            return appSettings;
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            runner.SetMasterClient(runner.LocalPlayer);

            foreach (NetworkBehaviour networkBehaviour in runner.GetAllBehaviours<NetworkBehaviour>())
            {
                networkBehaviour?.Object.RequestStateAuthority();
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Task vivoxChannelLeave = VivoxManager.Instance.LeaveAllChannelAsync();
            
            m_bIsRunnerValid = false;
            //LobbyJoined = false;
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            if (runner.SessionInfo.IsValid)
            {
                UIManager.Instance.CharacterSelectUIController.SetSessionName(runner.SessionInfo.Name);
            }
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            m_sessionList = sessionList;

            string sessionNames = sessionList.Aggregate("",
                (current,
                 session) => current + (session.Name + '\n'));

            Debug.Log("Sessions: \n" + sessionNames, this);

            //sessionUpdate 되었다고 표시
            _isSessionUpdated = true;
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            if(SceneManager.GetActiveScene().name == "MainMenu") {
                RefreshServerSelect();
            }
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }
    }
}