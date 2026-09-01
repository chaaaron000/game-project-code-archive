using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using ExitGames.Client.Photon.StructWrapping;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Fusion.Photon.Realtime;


/*
 * TODO:
 * 1. 로비 조인 실패 시 재시도 기능 추가
 */


public class Runner : MonoBehaviour, INetworkRunnerCallbacks  
{
    public static NetworkRunner runner;
    public List<SessionInfo> _sessionList;
    public string lobbyId = "OurLobbyId";
    public bool RunnerReady = false;

    public NetworkedManagerSpawner NetworkedManagerSpawner;
    public NetworkRunnerHandler NetworkRunnerHandler;
    public UnityEvent OnJoinLobby;
    public UnityEvent OnLeaveLobby;


    private void Awake()
    {
        //NetworkRunner를 찾아서 할당
        runner = gameObject.GetComponent<NetworkRunner>();

        if(runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }

        
    }

    

    private void OnEnable()
    {
        // 버튼 재활성화를 위해 이벤트 연결
        MainMenuUIController mainMenuUIController = FindObjectOfType<MainMenuUIController>();
        // OnJoinLobby.AddListener(() => mainMenuUIController.TryGameButtonInteractable());
        // OnLeaveLobby.AddListener(() => mainMenuUIController.TryGameButtonInteractable());
        // OnLeaveLobby.AddListener(() => VivoxManager.Instance.LeaveAllChannelAsync());
        // OnLeaveLobby.AddListener(() => UIManager.Instance.CharacterSelectUIController.RemoveAllListeners());

        NetworkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        NetworkedManagerSpawner = GetComponent<NetworkedManagerSpawner>();
        // NetworkedManagerSpawner.SpawnNetworkedManager(false);
        
        //var lobbyTask = JoinLobby();
    }

    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
        
        if(sceneManager == null)
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        return sceneManager;
    } 


    protected async Task<bool> InitializeNetworkRunner(
            NetworkRunner runner, 
            GameMode gameMode, 
            string sessionName, 
            byte[] connectionToken, 
            NetAddress address, 
            SceneRef scene, 
            Action<NetworkRunner> initialized, 
            bool isServer
        )
    {
        var SceneManager = GetSceneManager(runner);

        runner.ProvideInput = true;
        var result = await runner.StartGame(new StartGameArgs{
            GameMode = gameMode,
            IsVisible = true,
            IsOpen = true,
            Address = address,
            //CustomLobbyName = lobbyId,
            ConnectionToken = connectionToken,
            SessionName = sessionName,
            Scene = scene,
            SceneManager = SceneManager,

            
        });

        if(result.Ok){
            Debug.Log("Game started successfully");
            return true;
        }
        else {
            Debug.LogError($"Failed to start game: {result.ShutdownReason}");
            // OnLeaveLobby?.Invoke();
            return false;
        }
    }

    private async Task JoinLobby()
    {
        Debug.Log("Joining Lobby");

        //lobbyId를 통해 SessionLobby에 참가
        var result = await runner.JoinSessionLobby(SessionLobby.Shared, lobbyId);
        
        //정상적으로 참가되었는지 확인
        if(!result.Ok)
        {
            Debug.LogError($"Failed to join lobby: {lobbyId}");
        }
        else 
        {
            Debug.Log($"Joined lobby: {lobbyId}");
            RunnerReady = true;
            
            // 게임 시작, 게임 참가 버튼 활성화를 위한 UnityEvent 호출
            OnJoinLobby?.Invoke();
        }
    }

    public async void RunnerCreateGame() {
        string sessionName = RandomSessionNameGenerator.GenerateRandomRoomName();
        //sessionName이 중복되지 않도록 생성
        while(FindGameSession(sessionName) == true)
        {
            sessionName = RandomSessionNameGenerator.GenerateRandomRoomName();
        }

        var clientTask = InitializeNetworkRunner(
                runner, 
                GameMode.Shared, 
                sessionName, 
                SteamTicketManager.Instance.GetToken(), 
                NetAddress.Any(), 
                SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex), 
                null, 
                true
            );
        await clientTask;

        if(clientTask.Result == true)
        {
            NetworkedManagerSpawner.SpawnNetworkedManager(true);
            UIManager.Instance.MainMenuUIController.SwitchToCamera2();
            
            // Vivox 채널을 sessionName을 사용해 참가
            VivoxManager.Instance.JoinPositionalChannelAsync(sessionName);
        }
        else {
            Debug.LogError("Failed to create game");
        }
    }

    public async void RunnerJoinGame(string sessionName) {
        //sessionName이 존재하는지 확인 후 있으면 접속, 없으면 로그 메시지 출력
        if(FindGameSession(sessionName) == true)
        {
            
        }
        else {
            Debug.Log($"Session {sessionName} is not found.");
            return;
        }

        var clientTask = InitializeNetworkRunner(
                runner, 
                GameMode.Shared, 
                sessionName, 
                SteamTicketManager.Instance.GetToken(), 
                NetAddress.Any(), 
                SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex), 
                null, 
                false
            );
        await clientTask;

        if(clientTask.Result == true)
        {
            // NetworkedManagerSpawner.SpawnNetworkedManager(true);
            UIManager.Instance.MainMenuUIController.SwitchToCamera2();
            
            // Vivox 채널을 sessionName을 사용해 참가
            VivoxManager.Instance.JoinPositionalChannelAsync(sessionName);
        }
        else {
            Debug.LogError("Failed to join game");
        }
        
    }

    public async void RunnerLeaveGame()
    {
        Debug.Log("RunnerLeaveGame");
        var task = runner.Shutdown(true, ShutdownReason.Ok, false);
        await task;
    }

    public bool FindGameSession(string sessionName)
    {
        //_sessionList가 0개라면
        if(_sessionList == null)
        {
            return false;
        }

        //_sessionList를 순회하며 sessionName과 같은 이름의 session이 있는지 확인
        foreach(var session in _sessionList)
        {
            if(session.Name == sessionName)
            {
                return true;
            }
        }
        return false;
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Session list updated.");

        _sessionList = sessionList;
        Debug.Log($"Session count: {sessionList.Count}");
        foreach(var session in sessionList)
        {
            Debug.Log($"Session: {session.Name}");
        }
    }
    
    public void OnDestroy()
    {
        Debug.Log("OnDestroy");
        
        if (NetworkRunnerHandler == null)
            NetworkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        
        NetworkRunnerHandler?.ScheduleRunnerCreation();
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
        Debug.Log($"OnPlayerLeft {player.PlayerId}");
        runner.SetMasterClient(runner.LocalPlayer);
        Debug.Log("PlayerRef.MasterClient.PlayerId = " + PlayerRef.MasterClient.PlayerId);

        Debug.Log("OnPlayerLeft RequestStateAuthority Start");
        foreach (NetworkBehaviour networkBehaviour in runner.GetAllBehaviours<NetworkBehaviour>())
        {
            if (networkBehaviour == null)
            {
                continue;
            }
            
            // Debug.Log(networkBehaviour.name);
            networkBehaviour.Object.RequestStateAuthority();
        }
        Debug.Log("OnPlayerLeft RequestStateAuthority End");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W)) data.direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) data.direction += Vector3.back;
        if (Input.GetKey(KeyCode.A)) data.direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) data.direction += Vector3.right;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("OnShutdown");
        RunnerReady = false;
        
        // 세션 나가기 이벤트 호출
        OnLeaveLobby?.Invoke();
        // NetworkRunnerHandler.MakeNetworkRunner();
        
        // 세션이 재접속 됐을 때 사라진 매니저 다시 생성
        // NetworkedManagerSpawner.SpawnNetworkedManager();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");
        
        // 세션에 접속할 때마다 세션 이름 변경
        if (runner.SessionInfo.IsValid)
            UIManager.Instance.CharacterSelectUIController.SetSessionName(runner.SessionInfo.Name);
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
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("OnDisconnectedFromServer");
    }

    public struct NetworkInputData : INetworkInput // OnInput()을 위한 인터페이스입니다.
    {
        public const byte MOUSEBUTTON1 = 0x01;
        public const byte MOUSEBUTTON2 = 0x02;
        public byte buttons;
        public Vector3 direction;
    }
}
