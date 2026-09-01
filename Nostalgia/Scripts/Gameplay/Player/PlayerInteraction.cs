using System;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using Nostal.Interfaces;
using UnityEngine.Localization;
using UnityEngine.UI;

public class PlayerInteraction : NetworkBehaviour
{
    private Camera m_camera;
    private PlayerMovement _playerMovement = null;
    private Player _player = null;
    private CapsuleCollider _runningCollider = null;
    public float maxDistance = 3f;
    public GameObject _interactUI;
    private float _UItime = 0.15f;
    private bool _UIFlag = false;
    //점프스케어 등 특정 상황에서 UI를 띄우지 않기 위한 변수
    public bool isUICanShow = true;

    public TextMeshProUGUI _UIText;

    public GameObject UIcanvase;

    [SerializeField] private Image m_interactKeyImage;
    
    [Header("Use Item")]
    [SerializeField] private PlayerInventory m_playerInventory;
    [SerializeField] private LocalizedString m_useItemString;
    [SerializeField] private Sprite m_useItemKeyIcon;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _player = GetComponent<Player>();
        _runningCollider = gameObject.transform.GetChild(3).GetComponent<CapsuleCollider>();
        _runningCollider.enabled = false;
        m_camera = Camera.main;
    }

    private void Start()
    {
        // 로컬이 아니면 인터렉션 컴포넌트, UI캔버스 삭제 
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        while (true)
        {
            NetworkObject obj = GameManager.Instance.GetLocalPlayer();
            if (obj == null)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            if (obj.gameObject.name != gameObject.name)
            {
                Destroy(UIcanvase);
                Destroy(gameObject.GetComponent<PlayerInteraction>());
            }

            break;
        }
    }

    public void Update() 
    {
        //화면 중앙에서 부터 ray를 쏴서 부딪힌 물체가 InteractableObject인지 확인
        Ray ray = m_camera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        
        if (m_playerInventory.IsItemSelected)
        {
            _UItime = 0.15f;
            if (!_UIFlag && isUICanShow)
            {
                _interactUI.SetActive(true);
                _UIFlag = true;
            }
            
            _UIText.text = m_useItemString.GetLocalizedString();
            m_interactKeyImage.sprite = m_useItemKeyIcon;
        }
        else if (Physics.Raycast(ray, out RaycastHit hit, maxDistance) && hit.collider.gameObject.CompareTag("Interactable"))
        {
            _UItime = 0.15f;
            
            if (!_UIFlag && isUICanShow)
            {
                _interactUI.SetActive(true);
                _UIFlag = true;
            }

            IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
            if (interactable == null)
            {
                Debug.LogError("인터렉션 오류", hit.collider.gameObject);
                return;
            }
            
            InteractPromptData interactPromptData = interactable.GetInteractPromptData();

            _UIText.text = interactPromptData.PromptText.GetLocalizedString();
            m_interactKeyImage.sprite = interactPromptData.KeySprite;
        }
        else if (_UItime < 0) 
        {
            _UIFlag = false;
            _interactUI.SetActive(false);
        }

        _UItime -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // 얘는 어차피 멀티플레이 싱크가 필요가 없어 보임
        _runningCollider.enabled = _playerMovement.IsRunning;
    }

    public void OnInteract(InputAction.CallbackContext context) 
    {
        if (context.phase != InputActionPhase.Performed)
        {
            return;
        }
        // Debug.Log("Interact button pressed");
        
        // 현재 캐비넷에 숨어있는 상태면 나가는 기능만 작동
        if (_player.isHidden && _player._cabinet != null) 
        {
            _player._cabinet.GetComponent<Cabinet>().OnInteract(this.GetComponent<NetworkObject>());
            return;
        }

        //화면 중앙에서 부터 ray를 쏴서 부딪힌 물체가 InteractableObject인지 확인
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = m_camera.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            GameObject hitGameObject = hit.collider.gameObject;
            if (!hitGameObject.CompareTag("Interactable"))
            {
                return;
            }
            
            // IInteractable를 상속받은 클래스라면 그 오브젝트의 OnInteract 함수를 호출
            IInteractable interactable = hitGameObject.GetComponent<IInteractable>();
            interactable?.OnInteract(this.GetComponent<NetworkObject>());
        }
    }

    /// <summary>
    /// Input System의 tab 버튼이 눌렸을 때 호출
    /// </summary>
    public void OnDiary(InputAction.CallbackContext context)
    {
        // UIStack에 UI가 있거나 모은 일기장이 없으면 X
        if (UIManager.Instance.UIStackCount > 0 || GameManager.Instance.DiarySystem.collectDiaryNum < 1)
        {
            return;
        }
        
        GameManager.Instance.DiarySystem.ToggleDiaryMode();
    }

}
