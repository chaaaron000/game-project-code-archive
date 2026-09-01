using Nostal.Settings;
using UnityEngine;

namespace Nostal.Player
{
    public class CheatCamera : MonoBehaviour
    {
        [SerializeField] private Camera m_fpsCamera;
        [SerializeField] private GamePlaySettingsSO m_gamePlaySettingsSO;
        [SerializeField] private float m_normalMoveSpeed = 5f;
        [SerializeField] private float m_scrollSpeedSensitivity = 5f;

        private Camera m_cheatCamera;
        
        private float m_currentMoveSpeed;
        private float m_fastMoveSpeedMultiplier = 2f;
        
        private float m_rotationX;
        private float m_rotationY;

        private void Start()
        {
            m_cheatCamera = GetComponent<Camera>();
            m_cheatCamera.enabled = false;
            
            m_currentMoveSpeed = m_normalMoveSpeed;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                m_fpsCamera.enabled = !m_fpsCamera.enabled;
                m_cheatCamera.enabled = !m_cheatCamera.enabled;
            }
            
            Rotation();
            Movement();
            HandleSpeedAdjustment();
        }

        private void Rotation()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            m_rotationX += mouseX * m_gamePlaySettingsSO.MouseSensitivity;
            m_rotationY -= mouseY * m_gamePlaySettingsSO.MouseSensitivity;
            m_rotationY    = Mathf.Clamp(m_rotationY, -90f, 90f);

            transform.localEulerAngles = new Vector3(m_rotationY, m_rotationX, 0);
        }

        private void Movement()
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal"); // A, D 또는 좌우 화살표
            float verticalInput = Input.GetAxisRaw("Vertical");     // W, S 또는 위아래 화살표

            // 이동 속도 결정 (Shift 키로 빠르게)
            float actualSpeed = m_currentMoveSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                actualSpeed *= m_fastMoveSpeedMultiplier;
            }

            Vector3 forward = transform.forward * verticalInput;
            Vector3 right = transform.right     * horizontalInput;
            Vector3 moveDirection = (forward + right).normalized; // 대각선 이동 시 속도 보정

            // 위/아래 이동 (Q, E 키)
            if (Input.GetKey(KeyCode.E))
            {
                transform.Translate(Vector3.up * (actualSpeed * Time.deltaTime), Space.World);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Translate(Vector3.down * (actualSpeed * Time.deltaTime), Space.World);
            }

            transform.Translate(moveDirection * (actualSpeed * Time.deltaTime), Space.World);
        }
        
        private void HandleSpeedAdjustment()
        {
            // 마우스 휠로 이동 속도 조절
            float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheelInput != 0f)
            {
                // currentMoveSpeed를 직접 조절하거나, normalMoveSpeed를 조절할 수 있음
                // 여기서는 normalMoveSpeed를 조절하고 currentMoveSpeed는 이를 따르도록 함
                m_normalMoveSpeed += scrollWheelInput * m_scrollSpeedSensitivity;
                m_normalMoveSpeed = Mathf.Max(0.1f, m_normalMoveSpeed); // 최소 속도 제한
                m_currentMoveSpeed = m_normalMoveSpeed;                 // 현재 속도에도 반영 (Shift 안눌렀을 때 기준)
                // Debug.Log($"치트 카메라 이동 속도 변경됨: {m_normalMoveSpeed:F1}");
            }
        }
    }
}