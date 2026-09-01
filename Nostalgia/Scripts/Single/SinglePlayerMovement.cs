using UnityEngine;
using UnityEngine.InputSystem;

namespace Nostal.Single
{
    public class SinglePlayerMovement : MonoBehaviour
    {
        public bool m_bUseGravity;
        public bool m_bCanMove;

        [Header("Components")]
        [SerializeField] 
        private CharacterController m_characterController;

        [SerializeField]
        private SinglePlayerStamina m_playerStamina;

        private const float DEFAULT_WALK_SPEED = 3f;
        private const float DEFAULT_RUN_FORCE = 3f;
        private const float BRAKING_FORCE = 5f;
        private const float GRAVITY_ACCELERATION = 9.81f;
        private const float RUN_STAMINA_COST = 0.25f;

        private Transform m_root;

        private bool m_bIsMoving;
        private bool m_bIsRunning;
        private Vector2 m_input;
        private Vector3 m_velocity;

        private void OnEnable()
        {
            m_root = transform.root;
            m_input = Vector2.zero;
            m_velocity = Vector3.zero;

            m_bUseGravity = true;
            m_bCanMove = true;
            m_bIsMoving = false;
            m_bIsRunning = false;
        }

        private void FixedUpdate()
        {
            if (m_bIsRunning && m_bIsMoving)
            {
                if (m_playerStamina.m_currentStamina < RUN_STAMINA_COST)
                {
                    m_bIsRunning = false;
                    return;
                }

                m_playerStamina.ConsumeStamina(RUN_STAMINA_COST);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            CalculateFallVelocity();
            Move();
        }

        public void Teleport(Transform target)
        {
            m_characterController.enabled = false;

            m_root.transform.position = target.position;
            m_root.transform.rotation = target.rotation;

            m_characterController.enabled = true;

            // 관성 제거 
            m_velocity = Vector3.zero;
        }    

        public void OnMove(InputAction.CallbackContext context) 
        {
            m_input = context.ReadValue<Vector2>();

            m_bIsMoving = (m_input.x != 0f || m_input.y != 0f);
        }

        public void OnRun(InputAction.CallbackContext context) 
        {
            if (m_playerStamina.m_currentStamina < RUN_STAMINA_COST)
            {
                return;
            }

            m_bIsRunning = context.ReadValueAsButton();
        }

        private void CalculateFallVelocity()
        {
            if (!m_bUseGravity) 
            {
                m_velocity.y = 0f;
                return; 
            }

            if (m_characterController.isGrounded && m_velocity.y < 0)
            {
                m_velocity.y = -1f;
            }
            else
            {
                m_velocity.y -= GRAVITY_ACCELERATION * Time.deltaTime;
            }
        }

        private void Move()
        {
            if (!m_bCanMove)
            {
                return;
            }

            float speed = DEFAULT_WALK_SPEED;

            if (m_bIsRunning)
            {
                speed *= DEFAULT_RUN_FORCE;
            }

            Vector3 targetHorizontalVelocity = ((m_root.right * m_input.x) + (m_root.forward * m_input.y)) * speed;
            Vector3 currentHorizontalVelocity = new Vector3(m_velocity.x, 0, m_velocity.z);
            Vector3 horizontalVelocity = Vector3.Lerp(currentHorizontalVelocity, targetHorizontalVelocity, BRAKING_FORCE * Time.deltaTime);

            m_velocity.x = horizontalVelocity.x;
            m_velocity.z = horizontalVelocity.z;

            m_characterController.Move(m_velocity * Time.deltaTime);
        }
    }
}
