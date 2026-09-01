using UnityEngine;

namespace Nostal.Single
{
	[RequireComponent(typeof(BoxCollider))]
	public class TriggerMobActivator : MonoBehaviour
	{
		/// <summary>
		/// 발동 확률. 0 이하로 설정하면 작동하지 않습니다. 1 이상으로 설정하면 언제나 작동합니다.
		/// </summary>
		[SerializeField]
		private float m_invocationProbability = 0.5f;

		[SerializeField]
		private Mob m_mobPrefab;

		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.CompareTag("Player"))
			{
				return;
			}

			if (m_invocationProbability <= 0f)
			{
				return;
			}

			if (m_invocationProbability >= 1f || UnityEngine.Random.value < m_invocationProbability)
			{
				Debug.Log($"{gameObject.name} 작동", this);
				//Instantiate(m_mobPrefab);
			}
		}
	}
}
