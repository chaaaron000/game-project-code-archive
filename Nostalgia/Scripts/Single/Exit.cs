using Nostal.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nostal.Single
{
	[RequireComponent(typeof(BoxCollider))]
	public class Exit : MonoBehaviour
	{
		[Header("문 파츠")]
		[SerializeField]
		private GameObject m_doorFrame;

		[SerializeField]
		private GameObject m_exit;

		[Header("레이어")]
		[SerializeField]
		private LayerMask m_defaultLayer;

		[SerializeField]
		private LayerMask m_exitHighlightLayer;

		private int m_defaultLayerIndex;
		private int m_exitHighlightLayerIndex;

		private LevelManager m_levelManager;

		[SerializeField]
		private bool m_bIsOpen;

		private void OnEnable()
		{
			m_defaultLayerIndex = LayerUtility.GetFirstLayerIndex(m_defaultLayer);
			m_exitHighlightLayerIndex = LayerUtility.GetFirstLayerIndex(m_exitHighlightLayer);

			gameObject.layer = m_defaultLayerIndex;
			LayerUtility.SetLayerAllChildren(transform, m_defaultLayerIndex);

			m_levelManager = FindAnyObjectByType<LevelManager>();

			m_bIsOpen = false;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!m_bIsOpen || !other.CompareTag("Player"))
			{
				return;
			}

			m_levelManager.EndGame(true);
		}
	}
}

