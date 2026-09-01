using UnityEngine;

namespace Nostal.Single
{
	public class Mob : MonoBehaviour
	{
		private SinglePlayer m_player;

		private void OnEnable()
		{
			m_player = FindAnyObjectByType<SinglePlayer>();
		}
	}
}

