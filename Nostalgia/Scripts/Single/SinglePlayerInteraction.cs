using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nostal.Single
{
    public class SinglePlayerInteraction : MonoBehaviour
    {
        [SerializeField]
        private SinglePlayer m_player;

        [SerializeField]
        private Camera m_camera;

        private const float RAY_DISTANCE = 3f;

        private HideableCabinet m_hideableCabinet;

        public void OnInteract(InputAction.CallbackContext context)
        {
            // ?
            if (context.phase != InputActionPhase.Performed)
            {
                return;
            }

            // 플레이어가 캐비넷 안에 있는 경우
            if (m_player.m_bIsHidding)
            {
                m_hideableCabinet.Interact(m_player);
                return;
            }

            Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);
            Ray ray = m_camera.ViewportPointToRay(screenCenter);

            // Raycast 안 닿음
            if (!Physics.Raycast(ray, out RaycastHit hit, RAY_DISTANCE))
            {
                return;
            }

            // 태그 없음
            if (!hit.collider.gameObject.CompareTag("Interactable"))
            {
                return;
            }

            // Interactable를 상속하는 컴포넌트 없음
            if (!hit.collider.gameObject.TryGetComponent(out Interactable interactable))
            {
                return;
            }

            interactable.Interact(m_player);

            // 만약 캐비넷이라면 저장
            if (interactable is HideableCabinet)
            {
                m_hideableCabinet = interactable as HideableCabinet;
            }
        }
    }
}
