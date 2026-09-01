using System;
using System.Collections.Generic;
using System.Linq;
using Nostal.Util;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro;
using Unity.Mathematics;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Singleton Pattern

    private static UIManager instance = null;

    /// <summary>
    /// UIManager 싱글톤 구현
    /// </summary>
    public static UIManager Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }

    #endregion

    public Camera UICamera;
    [Header("UI Prefabs")]
    public GameObject CharacterSelectUIPrefab;
    public GameObject PlayerUIPrefab;
    public GameObject DeathTimerUIPrefab;
    public GameObject MainMenuUIPrefab;
    public GameObject TutorialUIPrefab;
    public GameObject LoadingPrefab;
    public GameObject ExitBlockUIPrefab;
    public GameObject StageSelectUIPrefab;
    public GameObject CantAddItemUIPrefab;

    public GameObject PauseMenu;
    public GameObject SettingUI;
    [SerializeField] private GameObject fadeCanvas;


    private MainMenuUIController mainMenuUIController;
    public MainMenuUIController MainMenuUIController {
        get {
            if(mainMenuUIController == null) {
                mainMenuUIController = FindObjectOfType<MainMenuUIController>();
            }
            return mainMenuUIController;
        }
        set => mainMenuUIController = value;
    }
    private CharacterSelectUIController characterSelectUIController;
    public CharacterSelectUIController CharacterSelectUIController{
        get {
            if(characterSelectUIController == null) {
                characterSelectUIController = FindObjectOfType<CharacterSelectUIController>();
            }
            return characterSelectUIController;
        }
        set => characterSelectUIController = value;
    }
    private TutorialUIController tutorialUIController;
    public TutorialUIController TutorialUIController{
        get {
            if(tutorialUIController == null) {
                tutorialUIController = FindObjectOfType<TutorialUIController>();
            }
            return tutorialUIController;
        }
        set => tutorialUIController = value;
    }
    private StageSelectUIController stageSelectUIController;
    public StageSelectUIController StageSelectUIController {
        get {
            if(stageSelectUIController == null) {
                stageSelectUIController = FindObjectOfType<StageSelectUIController>();
            }
            return stageSelectUIController;
        }
    }
    
    [Header("UI Controllers")]
    public PlayerUIController PlayerUIController;
    public DeathTimerUIController DeathTimerUIController;
    public LoadingUIController LoadingUIController;
    public ExitBlockUIController ExitBlockUIController;
    public CantAddItemUIController CantAddItemUIController;
    
    public FadeView FadeView;

    private Stack<GameObject> uiStack = new Stack<GameObject>();

    public int UIStackCount => uiStack.Count;
    
    public bool PauseMenuActive {get; set;}= false;
    public bool IsGameOver {get; set;}= false;

    void Awake()
    {
        // 싱글톤
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Initialize()
    {
        CreateUICamera();
        FadeView = Instantiate(fadeCanvas).GetComponent<FadeView>();
        DontDestroyOnLoad(FadeView.gameObject);
    }
    
    private void LateUpdate()
    {
        // TODO: 게임 중일 때만 가능하도록 조건 추가 필요
        if (SceneManager.GetActiveScene().buildIndex != 0 && Input.GetKeyDown(KeyCode.Escape) &&
            !IsGameOver)
        {
            if (uiStack.Count == 0)
                Push("PauseMenuController");
            else
                Pop();
        }
    }

    void CreateUICamera()
    {
        // 새 카메라 오브젝트 생성
        GameObject cameraObject = new GameObject("UI Camera");
        UICamera = cameraObject.AddComponent<Camera>();
        
        // 카메라가 다른 카메라에 의해 렌더링된 내용을 유지하면서 추가로 렌더링하게 함
        UICamera.clearFlags = CameraClearFlags.Depth;
        
        // UI 레이어만 렌더링하도록 Culling Mask를 설정
        UICamera.cullingMask = LayerMask.GetMask("UI");
        
        // 카메라 속성 설정
        UICamera.orthographic = true;
        
        UniversalAdditionalCameraData cameraData = UICamera.GetUniversalAdditionalCameraData();
        cameraData.renderType = CameraRenderType.Overlay;
        cameraData.renderPostProcessing = false;

        // 카메라 위치 설정
        UICamera.transform.position = new Vector3(0f, 0f, 10000f);
        UICamera.transform.LookAt(Vector3.zero);
        
        // 싱글톤화
        DontDestroyOnLoad(UICamera.gameObject);
    }

    GameObject InstantiateUIPrefab(GameObject uiPrefab)
    {
        GameObject uiObject = Instantiate(uiPrefab);
        
        // UICamera의 자식으로 설정
        uiObject.transform.SetParent(UICamera.transform);
        uiObject.transform.SetSiblingIndex(UICamera.transform.childCount - 1);
        Debug.Log(UICamera.transform.childCount - 1);
        
        // Canvas의 Render Camera를 UICamera로 설정
        uiObject.GetComponent<Canvas>().worldCamera = UICamera;

        return uiObject;
    }

    private void OnEnable()
    {
        Debug.Log("UIManager OnEnable");
        
        if (CharacterSelectUIController == null)
            CharacterSelectUIController = FindObjectOfType<CharacterSelectUIController>();
    }


    public void EnableCharacterSelectUI()
    {

    }

    public void EnablePlayerUI()
    {
        var playerUI = Instantiate(PlayerUIPrefab);
        Debug.Log("EnablePlayerUI: " + playerUI);
        PlayerUIController = playerUI.GetComponent<PlayerUIController>();
    }

    public void EnableDeathTimerUI()
    {
        var deathTimerUI = Instantiate(DeathTimerUIPrefab);
        DeathTimerUIController = deathTimerUI.GetComponent<DeathTimerUIController>();
    }

    public void DisableDeathTimerUI()
    {
        if(DeathTimerUIController != null) {
            Destroy(DeathTimerUIController.gameObject);
            DeathTimerUIController = null;
        } 
    }

    public void EnableExitBlockUI()
    {
        var exitBlockUI = Instantiate(ExitBlockUIPrefab);
        ExitBlockUIController = exitBlockUI.GetComponent<ExitBlockUIController>();
    }

    public void DisableExitBlockUI()
    {
        Destroy(ExitBlockUIController.gameObject);
        ExitBlockUIController = null;
    }

    public void EnableCantAddItemUI()
    {
        var cantAddItemUI = Instantiate(CantAddItemUIPrefab);
        CantAddItemUIController = cantAddItemUI.GetComponent<CantAddItemUIController>();
    }

    public void DisableCantAddItemUI()
    {
        Destroy(CantAddItemUIController.gameObject);
        CantAddItemUIController = null;
    }

    public void EnableMainMenuUI()
    {

    }

    public void DisableMainMenuUI()
    {
        Destroy(MainMenuUIController.gameObject);
        MainMenuUIController = null;
    }

    public void EnableTutorialUI()
    {
        var tutorialUI = Instantiate(TutorialUIPrefab);
        TutorialUIController = tutorialUI.GetComponent<TutorialUIController>();
        Debug.Log("EnableTutorialUI");
    }

    public void DisableTutorialUI()
    {
        Destroy(TutorialUIController.gameObject);
        TutorialUIController = null;
    }

    public void EnableLoadingUI()
    {
        var loadingUI = Instantiate(LoadingPrefab);
        LoadingUIController = loadingUI.GetComponent<LoadingUIController>();
        Debug.Log("EnableLoadingUI");
    }

    public void DisableLoadingUI()
    {
        if(LoadingUIController == null) {
            LoadingUIController = FindObjectOfType<LoadingUIController>();
            if(LoadingUIController == null) {
                Debug.LogError("LoadingUIController is null");
                return;
            }
        }

        Destroy(LoadingUIController.gameObject);
        LoadingUIController = null;
        Debug.Log("DisableLoadingUI");
    }


    /// <summary>
    /// Stack에 Push하는 함수
    /// </summary>
    /// <param name="uiName">Push하고 싶은 UIController 인터페이스의 이름: PauseMenuController, SettingUIController</param>
    /// <returns>Push한 UIController를 반환. 올바르지 않은 이름이면 null 반환</returns>
    public UIController Push(string uiName)
    {
        GameObject uiObject;
        UIController uiController;

        if (uiStack.Count != 0)
            uiStack.Peek().GetComponent<UIController>().Hide();
        
        switch (uiName)
        {
            case "PauseMenuController":
                uiObject = InstantiateUIPrefab(PauseMenu);
                break;
            
            case "SettingUIController":
                uiObject = InstantiateUIPrefab(SettingUI);
                break;
            
            default:
                Debug.LogError($"UIManager, public UIController Push({uiName}) : {uiName}는 잘못된 이름입니다.");
                uiObject = null;
                
                break;
        }

        Debug.Log("uI Push: " + uiObject);

        if (uiObject != null)
        {
            uiController = uiObject.GetComponent<UIController>();
            uiController.Show();
            uiStack.Push(uiObject);
        }
        else
        {
            uiController = null;
        }
        
        Debug.Log("uI stacks Count: " + uiStack.Count);
        return uiController;
    }

    public void Pop()
    {
        if (uiStack.Count == 0)
        {
            Debug.LogError("UIManager, public UIController Pop(): Stack에 UI가 없습니다.");
            return;
        }

        GameObject popUI = uiStack.Pop();
        if (popUI.TryGetComponent(out UIController uiController)) {
            Debug.Log(popUI.name);
            uiController.Hide();
        }
        
        Destroy(popUI);
        
        if (uiStack.Count > 0)
            uiStack.Peek().GetComponent<UIController>().Show();
    }

    public void Clear()
    {
        while(uiStack.Count > 0)
        {
            GameObject popUI = uiStack.Pop();
            if (popUI.TryGetComponent(out UIController uiController)) {
                Debug.Log(popUI.name);
                uiController.Hide();
            }
            
            Destroy(popUI);
        }
    }

    public bool IsUIOn()
    {
        return uiStack.Count > 0;
    }

    public void SetCameraLock(bool value) 
    {
        CursorController.SetEnableCursor(value);
        FindObjectOfType<FirstPersonCamera>()?.LockCameraRotate(value);
    }
}
