using System.Collections;
using Fusion;
using UnityEngine;

public class SelectCharacterManager : NetworkBehaviour, IStateAuthorityChanged
{
    #region Singleton Pattern
    
    private static SelectCharacterManager instance = null;
    
    /// <summary>
    /// UIManager 싱글톤 구현
    /// </summary>
    public static SelectCharacterManager Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }
    
    #endregion
    
    [Networked, OnChangedRender(nameof(OnPlayerReady))] 
    public bool _fatherPlayerReady   { get; set; } = false;
    [Networked, OnChangedRender(nameof(OnPlayerReady))] 
    public bool _daughterPlayerReady { get; set; } = false;

    //두 플레이어가 준비 상태에 들어가면 레디 버튼 비활성화 시키게 함
    // private bool isGameStarted = false;
    
    public override void Spawned()
    {
        // Runner = NetworkRunner.GetRunnerForGameObject(this.gameObject);
        // Debug.Log("SelectCharacterManager Spawned");
        // 싱글톤
        if (instance == null)
        {
            instance = this;
            Runner.MakeDontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Runner.DestroySingleton<SelectCharacterManager>();
            Runner.Despawn(Object);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);

        FindObjectOfType<CharacterSelectUIController>()?.RemoveAllListeners();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void SwapCharacterRpc()
    {
        Debug.Log("SwapCharacterRpc");
        
        GameManager.Instance.SwapPlayerInfoRpc();

        _fatherPlayerReady   = false;
        _daughterPlayerReady = false;
        
        ApplyInfoToUIRpc();
    }
    

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ReadyRpc(RpcInfo info = default)
    {
        if(GameManager.Instance.GameReady) return;

        PlayerRef playerRef = info.Source;
        Debug.Log("ReadyRpc: " + playerRef);

        if(playerRef == PlayerRef.None) {
            Debug.Log("GameManager - ReadyRpc : PlayerRef가 None입니다. / LocalPlayer로 대체됩니다.");
            playerRef = Runner.LocalPlayer;
        }

        if (playerRef == GameManager.Instance.FatherPlayerRef)
            _fatherPlayerReady = !_fatherPlayerReady;
        else if (playerRef == GameManager.Instance.DaughterPlayerRef)
            _daughterPlayerReady = !_daughterPlayerReady;
        else
            Debug.LogError("GameManager - ReadyRpc : 존재하지 않는 PlayerRef 입니다.");

        ApplyInfoToUIRpc();
    }
    
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void ApplyInfoToUIRpc(bool enableReadyButton = true)
    {
        CharacterSelectUIController characterSelectUIController = UIManager.Instance.CharacterSelectUIController;
        
        characterSelectUIController.SetPlayerName();
        characterSelectUIController.CharacterSwapButton.enabled = enableReadyButton;
        characterSelectUIController.ReadyButton.enabled = enableReadyButton;

        UIManager.Instance.MainMenuUIController.UpdateRoomUIRpc();
    }
    
    
    void OnPlayerReady()
    {
        if (_fatherPlayerReady && _daughterPlayerReady)
        {
            GameManager.Instance.ChangeGameReadyRpc(true);
            
            GoToStageSelectRpc();
            //여기에 스테이지 선택창으로 넘어가게 하기
            
            
            //_startGameCoroutine = StartCoroutine(StartGameWithDelay(5));
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void GoToStageSelectRpc() {
        //characterSelectUIController 비활성화
        CharacterSelectUIController characterSelectUIController = UIManager.Instance.CharacterSelectUIController;
        //characterSelectUIController.ShowCanvas(false);
        StartCoroutine(characterSelectUIController.FadeCanvas(false));

        //StageSelectUI 활성화
        SaveManager.Instance.StageSelectUIInitialize();
    }
    
    public void StateAuthorityChanged()
    {
        bool leftIsFather = !(GetComponent<NetworkObject>().StateAuthority == GameManager.Instance.FatherPlayerRef);
        
        if (leftIsFather)
            _fatherPlayerReady = false;
        else
            _daughterPlayerReady = false;

        if (UIManager.Instance.MainMenuUIController != null)
            UIManager.Instance.MainMenuUIController.UpdateRoomUIRpc();
    }
}
