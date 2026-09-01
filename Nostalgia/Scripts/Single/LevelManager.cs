using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nostal.Single
{
    public class LevelManager : MonoBehaviour
    {
		[SerializeField]
		private NostalSingleLevel m_nextLevel;

		private SinglePlayer m_player;

		private void OnEnable()
		{
			m_player = FindAnyObjectByType<SinglePlayer>();
		}

        private void StartGame()
        {

        }

        public void EndGame(bool bIsClear)
        {
			m_player.Movement.m_bCanMove = false;
			m_player.Movement.m_bUseGravity = false;
			m_player.Rotation.m_bCanRotate = false;
			
			if (bIsClear)
			{
				SingleSceneManager.Instance.LoadScene(m_nextLevel);
				return;
			}
        }
    }
}
