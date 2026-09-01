using System;
using UnityEngine;
using Fusion;
using Nostal.Settings;
using Nostal.Util;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private GamePlaySettingsSO m_gamePlaySettingsSO;
    
    public Transform Target = null;
    public Vector3 Offset = default;
    public PlayerMovement _otherPlayerMovement = null;
    private bool _deathFlag = false;
    private const float _maxXRotation = 50f;

    private float _verticalRotation   = 0f;
    private float _horizontalRotation = 0f;
    public Player localPlayer = null;

    public LayerMask CollisionMask; // 충돌 레이어 설정 (벽 등)
    public float SmoothSpeed = 10f;
    private bool rotationLock = false;
    private int rotationLockCount = 0;

    private bool _clearSpectate = false;  // 탈출구로 나가고 관전 상태인지 확인

    void Start() {
        Init();
    }

    public void Init() {
        CollisionMask = LayerMask.GetMask("Obstacle");
        //CursorController.SetEnableCursor(true);

        rotationLock = false;
        _deathFlag = false;
        _otherPlayerMovement = null;
        localPlayer = null;
        _clearSpectate = false;
    }

    void LateUpdate()
    {
        if (Target == null) return;
        
        if (_otherPlayerMovement == null) {
            if(GameManager.Instance.GetOtherPlayer() != null) 
                _otherPlayerMovement = GameManager.Instance.GetOtherPlayer().GetComponent<PlayerMovement>();
            // else 
            //     Debug.Log("Other Player is null");
        }
        
        if (localPlayer == null)
        {
            NetworkObject localPlayerObject = GameManager.Instance.GetLocalPlayer();
            if (localPlayerObject == null) return;
            if (!localPlayerObject.TryGetComponent(out localPlayer)) return;
        }

        var forward = transform.forward;
        var offset  = ( new Vector3(forward.x, 0f, forward.z) * Offset.z ) + ( Vector3.up * Offset.y );
        transform.position = Target.position + offset;
        
        if (_deathFlag || _clearSpectate) {
            _otherPlayerMovement.SendCameraRotationRpc();
            Quaternion smoothRotation = Quaternion.Slerp(
                transform.rotation,                   // 현재 회전
                _otherPlayerMovement.CameraRotation,  // 목표 회전
                0.1f                                // 보간 속도
            );
            Vector3 euler = smoothRotation.eulerAngles;
            euler.z = 0f;
            smoothRotation = Quaternion.Euler(euler);
            transform.rotation = smoothRotation;
            return;
        }

        if (rotationLock)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _verticalRotation   -= mouseY * m_gamePlaySettingsSO.MouseSensitivity;
        _horizontalRotation += mouseX * m_gamePlaySettingsSO.MouseSensitivity;
        _verticalRotation    = Mathf.Clamp(_verticalRotation, -_maxXRotation, _maxXRotation);

        transform.rotation   = Quaternion.Euler(_verticalRotation, _horizontalRotation, 0);
    }

    public void SetDeathFlag(bool flag)
    {
        _deathFlag = flag;
    }

    public void LockCameraRotate(bool value)
    {
        // 두 가지 이상의 회전을 막는 상황에서 풀리는 경우 방지를 위한 카운트
        // ex) 캐비넷 안에서 esc를 껏다 키면 카메라 락이 해제 되는 것과 같은 경우
        rotationLockCount += (value ? 1 : -1);
        rotationLockCount = Mathf.Max(0, rotationLockCount);
        rotationLock = (rotationLockCount > 0 ? true : false);
        Debug.Log("LockCameraRotate: " + value + ", rotationLockCount: " + rotationLockCount);
    }

    public void SetClearSpectate(bool value)
    {
        _clearSpectate = value;
    }
}