using UnityEngine;

namespace Nostal.Single
{
	public class OwlAbillityVisibillity : MonoBehaviour
	{
		[SerializeField]
		protected GameObject[] m_targetObject;

		private void OnEnable()
		{
			foreach (GameObject obj in m_targetObject)
			{
				obj.SetActive(false);
			}
		}
	}
}
