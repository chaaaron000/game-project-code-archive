using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using System.Collections;
using Nostal;
using System.Linq;
using Nostal.Player;
using Nostal.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class Player : NetworkBehaviour
{
    private const float MAX_STAMINA = 100f;
    private const float STAMINA_REGEN_RATE  = 20f;
    private const float STAMINA_REGEN_DELAY = 1.5f;
    private const float HEALTH_REGEN_RATE = 10f;
    private const float HEALTH_REGEN_DELAY = 5f;
    
    [Networked, OnChangedRender(nameof(HealthChanged))] 
    public float Health  { get; private set; } = 100f;
    private Coroutine _healthRecoveryCoroutine = null;
    
    [Networked, OnChangedRender(nameof(StaminaChanged))] 
    public float Stamina { get; private set; } = 100f;
    private Coroutine _staminaRecoveryCoroutine = null;
    
    [Networked] public bool _deathFlag {get; set;}= false;
    public PlayerFlashlight _playerFlashlight;

    private PlayerInput    _playerInput    = null;
    private PlayerMovement _playerMovement = null;
    private PlayerUI       _playerUI = null;

    public PlayerMovement Movement => _playerMovement;
    
    //cabinet에 숨은 상태 or 제단 안전지대인지 판별하는 변수
    [Networked] public bool isHidden {get; set;} = false;
    //공격당하고 있는지 판별하는 변수
    [Networked] public bool isAttack {get; set;} = false;
    public GameObject _cabinet;
    [Networked, OnChangedRender(nameof(OnChaseCntChanged))] public int chaseCnt {get; private set;}= 0;

    public BloodUIEffect bloodUIEffect;

    public GameObject bloodUIPanel;
    [SerializeField] public GameObject angryPanel;
    private Coroutine _heartBeatCoroutine = null;

    private PlayerInventory playerInventory;
    public PlayerInventory PlayerInventory {
        get {
            if (playerInventory == null)
            {
                playerInventory = GetComponent<PlayerInventory>();
            }
            return playerInventory;
        }
        set {
            
        }
    }

    [SerializeField] private PlayerEffects m_playerEffects;
    public PlayerEffects PlayerEffects => m_playerEffects;
    
    private void Awake()
    {
        _playerInput    = GetComponent<PlayerInput>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerUI       = GetComponent<PlayerUI>();
        
        _playerInput.enabled = false;
        GameManager.Instance.OnGameOver += OnGameOver;
    }

    public override void Spawned()
    {
        CursorController.SetEnableCursor(false);
        
        if (!HasInputAuthority)
        {
            return;
        } 

        if (gameObject.TryGetComponent(out Father temp) == false)
        {
            GameManager.Instance.SetNetworkObjectRpc(false, Object); 
        }
        else
        {
            GameManager.Instance.SetNetworkObjectRpc(true, Object); 
        }

        isHidden = false;
        isAttack = false;
        _playerInput.enabled = true;

        _heartBeatCoroutine = StartCoroutine(DetectMob());
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false) return;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(float damage, int mobID)
    {
        if(Health <= 0f) return;
        float afterDealDamage = Health - damage;
        Health = afterDealDamage >= 0f ? afterDealDamage : 0f;

        if (_healthRecoveryCoroutine != null)
        {
            StopCoroutine(_healthRecoveryCoroutine);
            _healthRecoveryCoroutine = null;
        }
        
        _healthRecoveryCoroutine = StartCoroutine(RecoverHealth());

        if(Health == 0f){
            StartCoroutine(Death(mobID)); 
        }
    }

    public IEnumerator Jumpscare(int mobID) {
        isAttack = true;

        Jumpscare jumpscare;

        switch(mobID) {
            //angry
            case 0:
                jumpscare = GameManager.Instance.GetJumpscare(0);
                UIManager.Instance.FadeView.SetColor(new Color(0.4f, 0f, 0f));
                UIManager.Instance.FadeView.FadeOut();
                
                //사망 애니메이션 재생
                _playerMovement.DeathAnimationRpc();

                //UI Interaction을 숨김
                GetComponent<PlayerInteraction>().isUICanShow = false;

                // DeathTimerUIController가 있으면 숨김
                if(UIManager.Instance.DeathTimerUIController != null)
                    UIManager.Instance.DeathTimerUIController.Hide();

                //까만화면인 상태로 2초 대기
                yield return new WaitForSeconds(2f);

                UIManager.Instance.FadeView.SetColor(Color.black);
                //까만화면인 상태로 2초 대기
                yield return new WaitForSeconds(2f);

                //화면 페이드 인
                UIManager.Instance.FadeView.FadeIn(1f);

                HideBloodUI();
                UIManager.Instance.PlayerUIController.Hide();
                PlayerInventory.inventoryUI.Hide();
                yield break;
            //expressionless
            case 1:
                jumpscare = GameManager.Instance.GetJumpscare(1);
                break;
            //sad
            case 2:
                jumpscare = GameManager.Instance.GetJumpscare(2);
                break;
            //smile
            case 3:
                jumpscare = GameManager.Instance.GetJumpscare(3);
                break;
            //female
            case 4:
                jumpscare = GameManager.Instance.GetJumpscare(4);
                break;
            default:
                jumpscare = GameManager.Instance.GetJumpscare(0);
                break;
        }
        
        Coroutine coroutine = _playerMovement.StartCoroutine(_playerMovement.Jumpscare(jumpscare));
        //사망시 충혈효과 켜져있으면 끄기
        HideBloodUI();
        yield return coroutine;

        //화면 페이드 인
        UIManager.Instance.FadeView.FadeIn(2f);

        isAttack = false;
    }

    private void HealthChanged()
    {
        if (!HasStateAuthority) 
        { 
            return; 
        }
        
        UpdateBloodUI();
    }

    /// <summary>
    /// Player의 스테미나를 사용하는 메소드
    /// </summary>
    /// <param name="use">Player의 스테미나를 얼만큼 쓸지</param>
    public void ReduceStamina(float use)
    {
        float afterReduceStamina = Stamina - use;
        Stamina = afterReduceStamina >= 0f ? afterReduceStamina : 0f;

        if (_staminaRecoveryCoroutine != null)
        {
            StopCoroutine(_staminaRecoveryCoroutine);
            _staminaRecoveryCoroutine = null;
        }
        
        _staminaRecoveryCoroutine = StartCoroutine(RecoverStamina());
    }

    private IEnumerator RecoverStamina()
    {
        // 회복 딜레이
        yield return new WaitForSeconds(STAMINA_REGEN_DELAY);

        while (Stamina < 100f)
        {
            Stamina += STAMINA_REGEN_RATE * Time.deltaTime;
            Stamina = Mathf.Min(Stamina, 100f);
            
            // 다음 프레임까지 대기
            yield return null;
        }
    }

    private IEnumerator RecoverHealth()
    {
        // 회복 딜레이
        yield return new WaitForSeconds(HEALTH_REGEN_DELAY);

        while (Health < 100)
        {
            Health += HEALTH_REGEN_RATE * Time.deltaTime;
            Health = Mathf.Min(Health, 100);

            // 다음 프레임까지 대기
            yield return null;
        }
    }

    void StaminaChanged()
    {
        if (HasStateAuthority == false) return;
        _playerUI.SetStamina(Stamina);
    }

    public IEnumerator Death(int mobID) {
        if (HasStateAuthority == false) yield break;
        //중복 죽음 방지
        if (_deathFlag == true) yield break;

        _deathFlag = true;
        Stamina = 0f;
        Health = 0f;

        //stamina 회복 코루틴 중지
        if(_staminaRecoveryCoroutine != null)
        {
            StopCoroutine(_staminaRecoveryCoroutine);
            _staminaRecoveryCoroutine = null;
        }
        //health 회복 코루틴 중지
        if(_healthRecoveryCoroutine != null)
        {
            StopCoroutine(_healthRecoveryCoroutine);
            _healthRecoveryCoroutine = null;
        }
        //심장소리 코루틴 중지
        StopCoroutine(_heartBeatCoroutine);
        SoundManager.Instance.SFX_loop_Stop("heartbeat");
        _heartBeatCoroutine = null;

        //flashlight 켜져있으면 끄고, 죽었을때 상호작용 안되도록
        _playerFlashlight.ChangeFlashlightEnable(false);

        //사망시 충혈효과 켜져있으면 끄기
        HideBloodUI();
        JumpscareSendRpc(mobID);
        yield return StartCoroutine(Jumpscare(mobID));

        _playerMovement.Death();

        GameManager.Instance.DeathEventRpc();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void JumpscareSendRpc(int mobID) {
        //클라쪽에만 실행
        if(HasStateAuthority) return;

        if(GameManager.Instance._deathFlag == true) {
            StartCoroutine(Jumpscare(mobID));
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ReviveRpc(Vector3 alterPosition) {
        if (HasStateAuthority == false) return;
        
        _deathFlag = false;
        Health = 100f;
        Stamina = 100f;
        SetBloodUI(true);
        StartCoroutine(_playerMovement.Revive(alterPosition));
        
        //심장 소리 코루틴 다시 재생
        _heartBeatCoroutine = StartCoroutine(DetectMob());

        GameplayEventManager.OnPlayerRevivedRPC(Runner, Runner.LocalPlayer);
        GameManager.Instance.ReviveEventRpc();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void EnterCabinetRpc(Vector3 inPosition, Quaternion inRotation, NetworkObject cabinet) {
        // if (HasStateAuthority == false) return;
        _playerMovement.EnterCabinet(inPosition, inRotation);

        if (cabinet.gameObject.TryGetComponent(out Cabinet cab))
        {
            SetCabinetRPC(cab);
        }

        //_cabinet = cabinet.gameObject;

        //flashlight 켜져있으면 끄고, 캐비넷 안에서 상호작용 안되도록
        _playerFlashlight.ChangeFlashlightEnable(false);
        isHidden = true;
        
        // 캐비넷에 숨은 상태일 때는 카메라 회전 불가
        if (Camera.main.TryGetComponent<FirstPersonCamera>(out FirstPersonCamera fpc))
            fpc.LockCameraRotate(true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ExitCabinetRpc(Vector3 outPosition, Quaternion outRotation) {
        // if (HasStateAuthority == false) return;
        _playerMovement.ExitCabinet(outPosition, outRotation);
        _cabinet = null;
        //flashlight 상호작용 되도록
        _playerFlashlight.ChangeFlashlightEnable(true);
        isHidden = false;
        
        if (Camera.main.TryGetComponent<FirstPersonCamera>(out FirstPersonCamera fpc))
            fpc.LockCameraRotate(false);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void EnterSafeZoneRpc() {
        isHidden = true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ExitSafeZoneRpc() {
        isHidden = false;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ChasedRpc() 
    {
        SoundManager.Instance.SFX_Play("chased");
        
        chaseCnt++;
        if (chaseCnt == 1)
        {
            _playerUI.ShowChaseImage();  // 눈알 표시
            GameplayEventManager.OnPlayerChaseStartedRPC(Runner, Runner.LocalPlayer);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void StopChasedRpc() 
    {
        chaseCnt--;
        if (chaseCnt == 0)
        {
            _playerUI.HideChaseImage();  // 눈알 표시 끄기
            GameplayEventManager.OnPlayerChaseEndedRPC(Runner, Runner.LocalPlayer);
        }

        if (chaseCnt < 0)
        {
            chaseCnt = 0;
        }
    }

    public void OnChaseCntChanged()
    {
        if(!HasStateAuthority) return;
        int otherChaseCnt = GetOtherChaseCnt();

        // 둘 다 0이면 gasp 소리 재생
        if (chaseCnt == 0 && otherChaseCnt == 0)
        {
            if (gameObject.TryGetComponent<Father>(out Father temp))
            {
                SoundManager.Instance.SFX_Play("gaspFather");
            }
            else if (gameObject.TryGetComponent<Daughter>(out Daughter temp2))
            {
                SoundManager.Instance.SFX_Play("gaspDaughter");
            }
        }
    }

    public int GetChaseCnt() {
        return chaseCnt;
    }

    public int GetOtherChaseCnt() {
        if(gameObject.TryGetComponent<Father>(out Father temp) == true)
        {
            return GameManager.Instance.DaughterNetworkObject.GetComponent<Player>().GetChaseCnt();
        }
        else if(gameObject.TryGetComponent<Daughter>(out Daughter temp2) == true)
        {
            return GameManager.Instance.FatherNetworkObject.GetComponent<Player>().GetChaseCnt();
        }
        else return 0;
    }

    public void OnGameOver() {
        if (HasStateAuthority == false) return;

        SoundManager.Instance.SFX_Stop("chased");
        chaseCnt = 0;
    }

    public void RefillStamina()
    {
        Stamina = MAX_STAMINA;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RefillStaminaRpc()
    {
        Stamina = MAX_STAMINA;
    }

    void UpdateBloodUI()
    {
        if(bloodUIPanel == null) return;

        int[] healthThresholds = { 90, 70, 50, 30, 20 };

        Debug.Log("UpdateBloodUI called. Current Health: " + Health);
        for (int i = 0; i < bloodUIEffect.images.Length; i++)
        {
            bool shouldShow = Health <= healthThresholds[i];
            bool isActive = bloodUIEffect.images[i].gameObject.activeSelf;

            if (shouldShow && !isActive)
            {
                bloodUIEffect.ShowImage(i);
                Debug.Log("Showing blood UI effect for index: " + i + ", Health: " + Health);
            }
            else if (!shouldShow && isActive)
            {
                bloodUIEffect.HideImage(i);
                Debug.Log("Hiding blood UI effect for index: " + i + ", Health: " + Health);
            }
        }
    }

    public void HideBloodUI(){
        Debug.Log("충혈효과 제거");
        if(bloodUIPanel != null) {
            for(int i = 0; i < 5; i++){
                bloodUIEffect.images[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetBloodUI(bool isbool){
        if(bloodUIPanel != null)
            bloodUIPanel.SetActive(isbool);
    }

    public bool CanUseItem() {
        if(Movement.isHidden || _deathFlag) return false;
        else return true;
    }

    public IEnumerator DetectMob() {
        while(true) {
            Collider[] collider = Physics.OverlapSphere(gameObject.transform.position, 20f, LayerMask.GetMask("Mob"));
            // Debug.Log("DetectMob : " + collider.Length);
            int cnt = collider.Length;
            for(int i=0; i<collider.Length; i++) {
                if(CheckDistanceY(collider[i].gameObject) == true) {
                    cnt--;
                }
            }
            if(cnt > 0) {
                SoundManager.Instance.SFX_loop_Play("heartbeat");
            }
            else {
                SoundManager.Instance.SFX_loop_Stop("heartbeat");
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public bool CheckDistanceY(GameObject gameObject) {
        float distance = math.abs(gameObject.transform.position.y - transform.position.y);
        if (distance > 3.0f) {
            return true;
        }
        else return false;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ClearInventoryRpc() {
        PlayerInventory.ClearInventory();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void SetCabinetRPC(Cabinet cabinetObject)
    {
        _cabinet = cabinetObject.gameObject;
    }
}
