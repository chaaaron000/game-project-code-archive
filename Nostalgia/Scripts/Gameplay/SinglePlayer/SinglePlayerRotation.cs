using UnityEngine;
using Nostal.Settings;
using System.Collections;

namespace Nostal.Single
{
	public class SinglePlayerRotation : MonoBehaviour
	{
		public bool m_bCanRotate;

		private const float MAX_PITCH = 50f;
		private const float MIN_PITCH = -90f;

        [SerializeField] 
		private GamePlaySettingsSO m_gamePlaySettingsSO;

		[SerializeField]
		private Transform m_pitchTarget;

		[SerializeField]
		private Transform m_yawTarget;

		private float m_currentPitch;
		private float m_currentYaw;

        private void OnEnable()
        {
			m_currentPitch = m_pitchTarget.transform.eulerAngles.x;
			m_currentYaw = m_yawTarget.transform.eulerAngles.y;

			m_bCanRotate = true;
        }

		// Update is called once per frame
		private void LateUpdate()
		{
			if (!m_bCanRotate)
			{
				return;
			}

            float mouseY = Input.GetAxis("Mouse Y");
            float mouseX = Input.GetAxis("Mouse X");

            m_currentPitch -= mouseY * m_gamePlaySettingsSO.MouseSensitivity;
			m_currentYaw += mouseX * m_gamePlaySettingsSO.MouseSensitivity;

			m_currentYaw = Mathf.Repeat(m_currentYaw, 360f);
            m_currentPitch = Mathf.Clamp(m_currentPitch, MIN_PITCH, MAX_PITCH);

			m_pitchTarget.localRotation = Quaternion.Euler(m_currentPitch, 0, 0);
			m_yawTarget.localRotation = Quaternion.Euler(0, m_currentYaw, 0);
        }

		public void LookForward()
		{
			m_pitchTarget.localRotation = Quaternion.Euler(Vector3.zero);
		}
    }
}