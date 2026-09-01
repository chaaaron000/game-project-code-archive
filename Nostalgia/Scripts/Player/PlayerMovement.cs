using System;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections;
using Unity.VisualScripting;
using ExitGames.Client.Photon.StructWrapping;
using Newtonsoft.Json.Linq;
using Nostal.Player;
using UnityEngine.SceneManagement;
using Steamworks;
using Nostal;

/// <summary>
/// TODO:
/// - CanMove 구현
/// </summary>

public class PlayerMovement : NetworkBehaviour
{
    private const float DEFAULT_WALK_SPEED = 3f;
    private const float DEFAULT_RUN_FORCE = 3f;

    public Camera Camera = null;
    [Networked] public Quaternion CameraRotation { get; set; } = Quaternion.identity;
    public Transform CameraOffset = null;

    public bool OnDirt = false;
    public bool CanMove = false;
    public bool isHidden = false;
    public bool JumpPressed { get; private set; } = false;
    public bool IsMoving { get; private set; } = false;
    public bool IsRunning { get; private set; } = false;

    public float WalkSpeed = 4.5f;
    public float RunForce = 1.4f;
    public float JumpForce = 5f;

    /// <summary>
    /// 입력이 없을 때 얼마나 천천히 멈출지
    /// - 값이 높으면 빨리 멈춤
    /// </summary>
    public float BrakingForce = 5f;
    public float GravityValue = 9.81f;

    private float _moveRight = 0f;
    private float _moveForward = 0f;
    private float _currentSpeed = 1f;
    private Vector3 _previousVelocity = default;

    private const float _runStamina = 0.25f;
    private const float _jumpStamina = 0.5f;

    public float MouseSensitivity = 10f;

    private Player _player = null;
    private CharacterController _characterController = null;
    private FirstPersonCamera firstPersonCamera = null;

    //사망 시 player Object를 안 보이도록 텔포시킬 위치
    private Vector3 deathPosition = new Vector3(0, -1000, 0);
    //플레이어가 텔포되는 위치와 발동되는 flag
    private bool _teleportFlag = false;
    private bool _isTeleporting = false;
    private Vector3 _teleportPosition = new Vector3(0, 0, 0);
    private Quaternion _teleportRotation = Quaternion.identity;
    public Vector3 _spawnPosition = new Vector3(0, 0, 0);

    public Animator _animator;
    private bool _spawnTrigger = false;
    public string _runningClip = "runningHallway";
    public string _walkingClip = "walkingHallway";

    private bool m_bIsBoosting = false;
    private float m_boostRate;

    public override void Spawned()
    {
        if (SceneManager.GetActiveScene().name == "TutorialS")
        {
            OnDirt = true;
            _runningClip = "runningDirt";
            _walkingClip = "walkingDirt";
        }

        _player = GetComponent<Player>();
        _characterController = GetComponent<CharacterController>();
        _characterController.enabled = true;

        if (HasStateAuthority == false)
        {
            return;
        }

        UIManager.Instance.EnablePlayerUI();

        Camera = Camera.main;
        if (Camera != null && Camera.TryGetComponent(out firstPersonCamera))
        {
            firstPersonCamera.Init();
            firstPersonCamera.Target = transform;
            firstPersonCamera.Offset = CameraOffset.localPosition;
        }

        _spawnTrigger = true;

        _currentSpeed = WalkSpeed;
    }

    public override void FixedUpdateNetwork()
    {
        // 아직 스폰이 끝나지 않았을 때 동작 간섭 방지
        if (!_spawnTrigger)
        {
            return;
        }
        
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasStateAuthority == false)
        {
            return;
        }
        
        if (_teleportFlag)
        {
            Teleport(_teleportPosition, _teleportRotation);
            return;
        }

        if (_isTeleporting)
        {
            return;
        }
        
        Move();
        Rotate();

        // 애니메이션 파라미터 설정
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (IsRunning && IsMoving && _player.Stamina > 0f)
        {
            _player.ReduceStamina(_runStamina);
            if (_player.Stamina <= 0)
            {
                IsRunning = false;
                _currentSpeed = WalkSpeed;
                SoundManager.Instance.SFX_loop_Stop_rpc(_runningClip, gameObject.GetComponent<NetworkObject>());
                SoundManager.Instance.SFX_loop_Play_rpc(_walkingClip, gameObject.GetComponent<NetworkObject>());
            }
        }
    }

    private void Move()
    {
        // 사망 시 등 CanMove가 false일 때 이동 불가
        if (CanMove == false)
        {
            return;
        }

        // 움직임 관성으로 인한 문제 예방 차원
        if (_isTeleporting)
        {
            _previousVelocity = Vector3.zero;
            return;
        }

        // Cabinet 안에 들어가 있으면 이동 금지
        if (isHidden)
        {
            return;
        }

        float speed = DEFAULT_WALK_SPEED;

        if (IsRunning)
        {
            speed *= DEFAULT_RUN_FORCE;
        }

        if (m_bIsBoosting)
        {
            speed *= m_boostRate;
        }

        float deltaTime = Runner.DeltaTime;
        Vector3 velocity = ((transform.right * _moveRight) + (transform.forward * _moveForward)) * speed;
        Vector3 moveVelocity = Vector3.Lerp(_previousVelocity, velocity, BrakingForce * deltaTime);

        if (_characterController.isGrounded)
        {
            moveVelocity.y = -1f;
        }
        else
        {
            moveVelocity.y = _previousVelocity.y - GravityValue * deltaTime;
        }

        _characterController.Move(moveVelocity * deltaTime);
        _previousVelocity = moveVelocity;
    }

    /// <summary>
    /// 캐릭터 오브젝트의 회전을 수행하는 메소드
    /// FixedUpdateNetwork에서 직접 마우스 움직임을 입력 받으면 매우 끊김
    /// 따라서 FirstPersonCamera.cs에서 회전한 것을 싱크하는 방법으로 구현
    /// </summary>
    private void Rotate()
    {
        // Cabinet 안에 들어가 있으면 이동 금지
        if (isHidden)
        {
            return;
        }
        
        transform.rotation = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
    }

    /// <summary>
    /// Input System의 WASD 버튼이 눌렸을 때 호출
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        _moveRight = input.x;
        _moveForward = input.y;

        if (_moveRight != 0 || _moveForward != 0)
        {
            IsMoving = true;
            if (IsRunning && !isHidden)
            {
                SoundManager.Instance.SFX_loop_Play_rpc(_runningClip, gameObject.GetComponent<NetworkObject>());
                SoundManager.Instance.SFX_loop_Stop_rpc(_walkingClip, gameObject.GetComponent<NetworkObject>());
            }
            else if (!isHidden)
            {
                SoundManager.Instance.SFX_loop_Play_rpc(_walkingClip, gameObject.GetComponent<NetworkObject>());
                SoundManager.Instance.SFX_loop_Stop_rpc(_runningClip, gameObject.GetComponent<NetworkObject>());
            }
        }
        else
        {
            IsMoving = false;
            SoundManager.Instance.SFX_loop_Stop_rpc(_runningClip, gameObject.GetComponent<NetworkObject>());
            SoundManager.Instance.SFX_loop_Stop_rpc(_walkingClip, gameObject.GetComponent<NetworkObject>());
        }
    }
    /// <summary>
    /// Input System의 Run 버튼이 눌렸을 때 호출
    /// - 홀드 방식
    /// </summary>
    public void OnRun(InputAction.CallbackContext context)
    {
        if (_player.Stamina - _runStamina <= 0f)
        {
            return;
        }

        if (context.ReadValueAsButton() == true)
        {
            IsRunning = true;
            // _currentSpeed = WalkSpeed * RunForce;
            if (IsMoving && !isHidden)
            {
                SoundManager.Instance.SFX_loop_Play_rpc(_runningClip, gameObject.GetComponent<NetworkObject>());
                SoundManager.Instance.SFX_loop_Stop_rpc(_walkingClip, gameObject.GetComponent<NetworkObject>());
            }
        }
        else
        {
            IsRunning = false;
            // _currentSpeed = WalkSpeed;
            if (IsMoving && !isHidden)
            {
                SoundManager.Instance.SFX_loop_Play_rpc(_walkingClip, gameObject.GetComponent<NetworkObject>());
                SoundManager.Instance.SFX_loop_Stop_rpc(_runningClip, gameObject.GetComponent<NetworkObject>());
            }
        }
    }

    public void Death()
    {
        CanMove = false;

        NetworkObject targetPlayer = GameManager.Instance.GetOtherPlayer();
        _teleportFlag = true;
        _teleportPosition = deathPosition;
        firstPersonCamera.Target = targetPlayer.transform;
        firstPersonCamera.Offset = targetPlayer.GetComponent<PlayerMovement>().CameraOffset.localPosition;
        firstPersonCamera.SetDeathFlag(true);

        //gameObject가 father이면 daughter의 시점에서 컬링마스크 설정
        gameObject.GetComponent<PlayerModelController>()
                  .ChangeCullingMask(gameObject.TryGetComponent(out Father _) == false);

        //플레이어 눕혀놓은거 해제
        SetAnimatorRpc("isDying", false);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ClearRpc(bool isOtherClear)
    {
        CanMove = false;

        //클리어 시 심장 소리 제거
        SoundManager.Instance.SFX_Stop("heartbeat");

        if (isOtherClear == true)
        {
            return;
        }

        NetworkObject targetPlayer = GameManager.Instance.GetOtherPlayer();
        _teleportFlag = true;
        _teleportPosition = deathPosition;
        firstPersonCamera.Target = targetPlayer.transform;
        firstPersonCamera.Offset = targetPlayer.GetComponent<PlayerMovement>().CameraOffset.localPosition;
        firstPersonCamera.SetClearSpectate(true);
    }

    public IEnumerator Revive(Vector3 alterPosition)
    {
        NetworkObject targetPlayer = GameManager.Instance.GetLocalPlayer();
        // Debug.Log("Revive alterPos: " + alterPosition);
        _teleportPosition = alterPosition;
        _teleportFlag = true;
        _teleportRotation = Quaternion.identity;
        
        IsRunning = false;

        // 부활하고 이펙트가 거의 끝나는 시간까지 대기
        yield return new WaitForSeconds(10.0f);
        
        // 페이드 아웃 앤 인
        UIManager.Instance.FadeView.FadeOutAndIn(1.0f, 1.0f, 1.0f);
        yield return new WaitForSeconds(1.0f);

        UIManager.Instance.PlayerUIController.Show();
        _player.PlayerInventory.inventoryUI.Show();

        // flashlight 킬 수 있게 변경 
        _player._playerFlashlight.ChangeFlashlightEnable(true);

        // 카메라 다시 자기 플레이어를 가리키도록 수정
        firstPersonCamera.Target = targetPlayer.transform;
        firstPersonCamera.Offset = targetPlayer.GetComponent<PlayerMovement>().CameraOffset.localPosition;
        firstPersonCamera.SetDeathFlag(false);

        // gameObject가 father이면 daughter의 시점에서 컬링마스크 설정
        gameObject.GetComponent<PlayerModelController>()
                  .ChangeCullingMask(gameObject.TryGetComponent(out Father _) != false);

        isHidden = false;
        CanMove = true;
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        _isTeleporting = true;
        
        gameObject.GetComponent<NetworkTransform>().Teleport(position: position, rotation: rotation);
        _teleportFlag = false;
        
        StartCoroutine(WaitForTeleport());
    }

    public IEnumerator WaitForTeleport()
    {
        yield return new WaitForSeconds(0.1f);
        _isTeleporting = false;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void TeleportRpc(NetworkObject obj)
    {
        _teleportPosition = obj.transform.position;
        _teleportRotation = obj.transform.rotation;

        _teleportFlag = true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void TeleportRpc(Vector3 position = default, Quaternion rotation = default)
    {
        _teleportPosition = position;
        _teleportRotation = rotation;
        _teleportFlag = true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void TeleportByItemRPC(Vector3 position = default, Quaternion rotation = default)
    {
        _player.PlayerEffects.PlayEffectRPC(PlayerEffectType.Teleport);
        SoundManager.Instance.SFX_Play_rpc(
            "usingTeleport",
            gameObject.transform.root.GetComponent<NetworkObject>());

        StartCoroutine(TeleportAfterDelayCoroutine(1f, position, rotation));
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SendCameraRotationRpc()
    {
        CameraRotation = firstPersonCamera.transform.rotation;
    }

    public void EnterCabinet(Vector3 inPosition, Quaternion inRotation)
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        
        _teleportFlag = true;
        _teleportPosition = inPosition;
        _teleportRotation = inRotation;
        IsRunning = false;
        isHidden = true;
        firstPersonCamera.transform.rotation = inRotation;

        SoundManager.Instance.SFX_loop_Stop_rpc(_walkingClip, gameObject.GetComponent<NetworkObject>());
        SoundManager.Instance.SFX_loop_Stop_rpc(_runningClip, gameObject.GetComponent<NetworkObject>());
    }

    public void ExitCabinet(Vector3 outPosition, Quaternion outRotation)
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        
        _teleportFlag = true;
        _teleportPosition = outPosition;
        _teleportRotation = outRotation;
        firstPersonCamera.transform.rotation = outRotation;
        isHidden = false;
    }

    private Coroutine m_boostCoroutine;
    public void BoostSpeed(float boostRate, float boostDuration)
    {
        m_boostRate = boostRate;
        
        if (m_bIsBoosting)
        {
            StopCoroutine(m_boostCoroutine);
        }

        m_boostCoroutine = StartCoroutine(BoostSpeedCoroutine(boostRate, boostDuration));
    }

    private IEnumerator BoostSpeedCoroutine(float boostRate, float boostDuration)
    {
        m_bIsBoosting = true;

        yield return new WaitForSeconds(boostDuration);

        m_bIsBoosting = false;
    }

    /// <summary>
    /// 캐릭터의 이동 방향 및 속도에 따라 애니메이션을 업데이트하는 메소드
    /// </summary>
    private void UpdateAnimation()
    {
        SetAnimatorRpc("isMovingForward", _moveForward > 0 && !isHidden);
        SetAnimatorRpc("isRunning", _moveForward > 0 && IsRunning && !isHidden);

        SetAnimatorRpc("isMovingBackward", _moveForward < 0 && !isHidden);
        SetAnimatorRpc("isMovingRight", _moveRight > 0 && _moveForward == 0 && !isHidden);
        SetAnimatorRpc("isMovingLeft", _moveRight < 0 && _moveForward == 0 && !isHidden);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetAnimatorRpc(string name, bool value)
    {
        _animator.SetBool(name, value);
    }

    public void Fall()
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        
        _teleportPosition = GameManager.Instance.GetSpawnPosition();
        _teleportFlag = true;
    }

    public IEnumerator Jumpscare(Jumpscare jumpscare) {
        //사망 애니메이션 재생
        DeathAnimationRpc();

        //충혈효과 제거
        _player.SetBloodUI(false);

        //점프스케어 사운드 재생
        for (int i = 0; i < jumpscare.audioSource.Length; i++)
        {
            jumpscare.audioSource[i].Play();
        }

        //애니메이션 재생 및 카메라 audio listener 변경
        if (Camera == null)
        {
            Camera = Camera.main;
        }
        
        Camera.GetComponent<AudioListener>().enabled = false;
        for (int i = 0; i < jumpscare.animator.Length; i++)
        {
            jumpscare.animator[i].SetTrigger("Jumpscare");
        }
        jumpscare.camera.enabled = true;
        jumpscare.camera.GetComponent<AudioListener>().enabled = true;
        Camera.enabled = false;
        UIManager.Instance.PlayerUIController.Hide();
        _player.PlayerInventory.inventoryUI.Hide();

        // UI Interaction을 숨김
        if (HasStateAuthority)
        {
            GetComponent<PlayerInteraction>().isUICanShow = false;
        }

        // DeathTimerUIController가 있으면 숨김
        if (UIManager.Instance.DeathTimerUIController != null)
        {
            UIManager.Instance.DeathTimerUIController.Hide();
        }

        //플레이어가 일기장 UI를 키고 있을 경우 꺼둔 후 DiarySystem을 숨김
        if (GameManager.Instance.DiarySystem != null) 
        {
            if (GameManager.Instance.DiarySystem.bIsDiaryEnabled)
            {
                GameManager.Instance.DiarySystem.ToggleDiaryMode();
            }
            
            GameManager.Instance.DiarySystem.gameObject.SetActive(false);
        }

        CameraShake cameraShake = jumpscare.camera.GetComponent<CameraShake>();
        cameraShake.StartShake(1.5f);

        yield return new WaitForSeconds(1.4f);

        cameraShake.StartMoveForward();

        yield return new WaitForSeconds(0.05f);

        // 소리 원상복구
        // SoundManager.Instance.setSFX_volume(originVolume);

        // //5초 대기, 이후 카메라 등 원상 복구
        // UIManager.Instance.PlayerUIController.Show();
        if (GameManager.Instance.DiarySystem != null)
        {
            GameManager.Instance.DiarySystem.gameObject.SetActive(true);
        }
        
        for (int i=0; i<jumpscare.animator.Length; i++) 
        {
            jumpscare.animator[i].SetTrigger("JumpscareEnd");
        }

        if (HasStateAuthority)
        {
            GetComponent<PlayerInteraction>().isUICanShow = true;
        }

        //화면 페이드아웃
        UIManager.Instance.FadeView.FadeOut(0.01f);

        //까만화면인 상태로 페이드아웃 끝나면 화면전환하게끔 대기
        yield return new WaitForSeconds(0.1f);

        Camera.enabled = true;
        jumpscare.camera.enabled = false;
        jumpscare.camera.GetComponent<AudioListener>().enabled = false;
        Camera.GetComponent<AudioListener>().enabled = true;

        //까만화면인 상태로 2초 대기
        yield return new WaitForSeconds(2f);

        GameplayEventManager.OnJumpscareEndedRPC(Runner, Runner.LocalPlayer);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SetCanMoveRpc(bool value)
    {
        CanMove = value;
    }

    public void StopMovingSound()
    {
        SoundManager.Instance.ResetSoundObjectRpc(gameObject.GetComponent<NetworkObject>());
    }

    public void SetDirt(bool value)
    {
        OnDirt = value;
        
        if (value == true)
        {
            _runningClip = "runningDirt";
            _walkingClip = "walkingDirt";
        }
        else
        {
            _runningClip = "runningHallway";
            _walkingClip = "walkingHallway";
        }
        
        StopMovingSound();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void InitializePositionRpc()
    {
        _spawnPosition = GameManager.Instance.GetSpawnPosition();

        Vector3 teleportPosition;
        if (gameObject.TryGetComponent<Father>(out Father temp) == true)
        {
            teleportPosition = _spawnPosition;
        }
        else if (gameObject.TryGetComponent<Daughter>(out Daughter temp2) == true)
        {
            teleportPosition = new Vector3(_spawnPosition.x, _spawnPosition.y, _spawnPosition.z + 1);
        }
        else
        {
            teleportPosition = _spawnPosition;
        }

        Teleport(teleportPosition, Quaternion.identity);
        // Debug.Log("InitializePositionRpc, Teleport Complete");
    }

    private IEnumerator TeleportAfterDelayCoroutine(float delaySeconds, Vector3 position, Quaternion rotation)
    {
        yield return new WaitForSeconds(delaySeconds);
        TeleportRpc(position, rotation);
    }
    

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void DeathAnimationRpc() 
    {
        _animator.Play("mixamo_com", 0, 0f);
    }
}