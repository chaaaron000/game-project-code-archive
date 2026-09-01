using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using Cinemachine;
using Fusion;
using Nostal.Network;
using Nostal.Util;

public class MainMenuUIController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainUIPanel;
    public GameObject joinSessionPanel;
    public GameObject DaughterDescriptionPanel;
    public GameObject FatherDescriptionPanel;

    [Header("Input Fields")]
    public TMP_InputField joinSessionNameInputField;
    // [Header("Texts")]
    // public TextMeshProUGUI sessionNameText;
    public CanvasGroup buttonCanvasGroup; // CanvasGroup을 연결
    public CanvasGroup mainUICanvasGroup;
    public CanvasGroup LobbyUICanvasGroup;
    public Camera LobbyUICamera;

    public float fadeDuration = 1.0f; // 페이드 지속 시간

    //촛불과 체크 이미지
    public Animator candleDaughterAnimator;
    public Animator candleFatherAnimator;
    public GameObject fatherReady;
    public GameObject daughterReady;

    public CinemachineVirtualCamera camera1;
    public CinemachineVirtualCamera camera2;

    public Button CreateGameButton;
    public Button JoinGameButton;
    public Button SettingsButton;
    public Button LeaveSessionButton;  // 얘는 LobbyUI Camera에 있는데 귀찮에서 여기서 관리
    public Button JoinPanelButton;

    [Header("Title UI Image")]

    [SerializeField]
    private Image m_titleImage;

    //[SerializeField]
    //private Material titleMaterialKR; // 셰이더가 적용된 머티리얼을 참조

    //[SerializeField]
    //private Material titleMaterialEN;

    private Material m_currentTitleMaterialInstance;
    private float m_currentTitleAlpha;
    private static readonly int m_titleAlphaPropID = Shader.PropertyToID("_AlphaThreshold");
    
    private bool isReady = false;

    private void OnEnable()
    {
        CursorController.SetEnableCursor(false);

        m_currentTitleMaterialInstance = m_titleImage.material;
        m_titleImage.material = m_currentTitleMaterialInstance;
    }

    public void Start()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        StartCoroutine(TitleEffect(1f, 0.01f, 2f));

        //networkRunnerHandler.OnJoinLobby();

        CreateGameButton = buttonCanvasGroup.transform.GetChild(0).GetComponent<Button>();
        JoinGameButton = buttonCanvasGroup.transform.GetChild(1).GetComponent<Button>();
        SettingsButton = buttonCanvasGroup.transform.GetChild(2).GetComponent<Button>();
        
        if (LeaveSessionButton == null)
            Debug.LogError("LeaveButton null");
        
        SettingsButton.onClick.AddListener(OnSettingsButtonClicked);
        LeaveSessionButton.onClick.AddListener(OnLeaveButtonClicked);

        // SoundManager.Instance.BGM_Play("startSceneBGM");
        
        GameButtonActive(false);
        LobbyUICamera.enabled = false;
        //LobbyUI.SetActive(false);
    }

    private void Update()
    {
        if (m_titleImage.material != m_currentTitleMaterialInstance)
        {
            m_currentTitleMaterialInstance = m_titleImage.material;
            m_titleImage.material = m_currentTitleMaterialInstance;
        }
            
        m_currentTitleMaterialInstance.SetFloat(m_titleAlphaPropID, m_currentTitleAlpha);

        if (VivoxManager.Instance.VivoxReady && NetworkManager.Instance.LobbyJoined && !isReady) 
        {
            isReady = true;
            GameButtonActive(true);
            StartCoroutine(FadeInOut(buttonCanvasGroup, 0f, 1f));
        }
    }

    private IEnumerator FadeInOut(CanvasGroup canvasGroup, float startAlpha, float endAlpha)
    {
        Debug.Log("어디서 fadeinout 불렸는지 확인");
        float elapsedTime = 0f;

        // 시작 알파값 설정
        canvasGroup.alpha = startAlpha;

        // 지정된 시간 동안 알파값을 변경
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        // 최종 알파값 설정
        canvasGroup.alpha = endAlpha;
    }

    /// <summary>
    /// 게임 타이틀을 나타나고 없어지게 하는 함수
    /// </summary>
    /// <param name="startValue">시작 알파 값</param>
    /// <param name="endValue">목표 알파 값</param>
    /// <param name="duration">시작 값에서부터 목표 값까지의 시간</param>
    /// <returns></returns>
    private IEnumerator TitleEffect(float startValue, float endValue, float duration)    
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);

            m_currentTitleAlpha = newValue;
            yield return null;
        }

        // 최종적으로 0으로 설정
        m_currentTitleAlpha = endValue;
    }

    private IEnumerator cameraChange(CanvasGroup fadeOutUI, CanvasGroup fadeInUI)
    {
        // 타이틀 사라지게
        if (fadeOutUI == mainUICanvasGroup) StartCoroutine(TitleEffect(0f, 1f, 1f));
        // 페이드 아웃 UI 알파값을 1에서 0으로 줄임
        yield return StartCoroutine(FadeInOut(fadeOutUI, 1f, 0f));
        
        // 페이드 아웃이 끝난 후 UI를 비활성화
        fadeOutUI.interactable = false;
        fadeOutUI.blocksRaycasts = false;

        yield return new WaitForSeconds(4.0f);

        // 페이드 인할 UI를 활성화
        fadeInUI.interactable = true;
        fadeInUI.blocksRaycasts = true;

        // 타이틀 다시 보이게
        if (fadeInUI == mainUICanvasGroup) StartCoroutine(TitleEffect(1f, 0.01f, 2f));
        // 페이드 인 UI 알파값을 0에서 1로 증가시킴
        yield return StartCoroutine(FadeInOut(fadeInUI, 0f, 1f));
    }

    public void EnableJoinPanel()
    {
        GameButtonActive(false);
        JoinPanelButton.interactable = true;
        joinSessionPanel.SetActive(true);
    }

    public void DisableJoinPanel()
    {
        GameButtonActive(true);
        joinSessionNameInputField.text = "";
        JoinPanelButton.interactable = false;
        joinSessionPanel.SetActive(false);
    }

    /// <summary>
    /// 캐릭터 선택 화면에서 메인 메뉴 화면으로
    /// </summary>
    public void SwitchToCamera1()
    {
        camera1.enabled = true;
        camera2.enabled = false;
        LobbyUICamera.enabled = false;
        UIManager.Instance.CharacterSelectUIController.ShowCanvas(false);
        //LobbyUI.SetActive(false);
        LobbyUICanvasGroup.alpha = 1f;
        StartCoroutine(cameraChange(LobbyUICanvasGroup, mainUICanvasGroup));
        GameButtonActive(true);
    }

    /// <summary>
    /// 메인 메뉴 화면에서 캐릭터 선택 화면으로
    /// </summary>
    public void SwitchToCamera2()
    {
        // camera1을 비활성화하고, camera2를 활성화
        // Debug.Log("SwitchToCamera2");
        
        camera2.enabled = true;
        camera1.enabled = false;
        LobbyUICamera.enabled = true;
        UIManager.Instance.CharacterSelectUIController.ShowCanvas(true);
        //LobbyUI.SetActive(true);
        LobbyUICanvasGroup.alpha = 0f;
        StartCoroutine(cameraChange(mainUICanvasGroup, LobbyUICanvasGroup));

        //UIManager.Instance.CharacterSelectUIPrefab.SetActive(true);
    }

    public void HideAllPanels()
    {
        mainUIPanel.SetActive(false);
        joinSessionPanel.SetActive(false);
    }

    //촛불과 체크 이미지를 갱신하는 함수 (UIManager에서 StateAuthority만 호출)
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void UpdateRoomUIRpc()
    {
        bool isFatherReady = SelectCharacterManager.Instance._fatherPlayerReady;
        bool isDaughterReady = SelectCharacterManager.Instance._daughterPlayerReady;
        bool isFatherSelected = GameManager.Instance.FatherPlayerRef != PlayerRef.None;
        bool isDaughterSelected = GameManager.Instance.DaughterPlayerRef != PlayerRef.None;
        // Debug.Log($"Ready) Father: {isFatherReady}, Daughter: {isDaughterReady}");
        // Debug.Log($"Selected) Father: {isFatherSelected}, Daughter: {isDaughterSelected}");

        //촛불 이미지 갱신
        candleFatherAnimator.SetBool("isSelected", isFatherReady);
        candleDaughterAnimator.SetBool("isSelected", isDaughterReady);

        //체크 이미지 갱신
        fatherReady.SetActive(isFatherReady);
        daughterReady.SetActive(isDaughterReady);

        //설명 이미지 갱신
        FatherDescriptionPanel.SetActive(GameManager.Instance.IsLocalPlayerFather);
        DaughterDescriptionPanel.SetActive(!GameManager.Instance.IsLocalPlayerFather);
    }

    public void OnJoinGameClicked()
    {
        CreateGameButton.interactable = false;
        JoinGameButton.interactable = false;
        JoinPanelButton.interactable = false;

        Task joinGame = NetworkManager.Instance.RunnerJoinGame(joinSessionNameInputField.text);
    }

    public void OnCreateGameClicked()
    {
        CreateGameButton.interactable = false;
        JoinGameButton.interactable = false;

        Task createGame = NetworkManager.Instance.RunnerCreateGame();
    }
    
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnSettingsButtonClicked()
    {
        UIManager.Instance.Push("SettingUIController");
    }

    void OnLeaveButtonClicked()
    {
        SwitchToCamera1();

        Task leaveGame = NetworkManager.Instance.RunnerLeaveGame();
    }

    /// <summary>
    /// 게임 시작 버튼과 게임 참가 버튼의 활성 & 비활성화를 설정하는 함수
    /// </summary>
    /// <param name="value">true : 버튼 활성화, false : 버튼 비활성화</param>
    private void GameButtonActive(bool value)
    {
        CreateGameButton.interactable = value;
        JoinGameButton.interactable = value;
        
    }
}
