using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Fusion;
using Nostal.Steam;
using Steamworks;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

namespace Nostal.Challenge
{
    public class ClearTime : NetworkBehaviour
    {
        [SerializeField] private NostalgiaGameLevel m_currentGameLevel;
        
        private readonly Stopwatch m_stopwatch = new Stopwatch();
        
        [Networked] public long ClearTimeMilliseconds { get; private set; }

        public override void Spawned()
        {
            base.Spawned();
            
            GameManager.OnReadyLevel += StartTimer;
            GameplayEventManager.GameOver += StopTimer;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            
            GameManager.OnReadyLevel -= StartTimer;
            GameplayEventManager.GameOver -= StopTimer;
        }

        public void StartTimer()
        {
            if (!HasStateAuthority || m_stopwatch.IsRunning)
            {
                return;
            }
            
            m_stopwatch.Reset();
            m_stopwatch.Start();
        }

        public void StopTimer(bool bIsClear)
        {
            if (!HasStateAuthority || !m_stopwatch.IsRunning)
            {
                return;
            }

            if (bIsClear)
            {
                m_stopwatch.Stop();
                ClearTimeMilliseconds = m_stopwatch.ElapsedMilliseconds;
                RecordClearTimeRPC(ClearTimeMilliseconds);
            }
        }

        /// <summary>
        /// Steamworks 통계에 클리어 타임을 저장합니다.
        /// </summary>
        /// <param name="clearTimeMilliseconds"></param>
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RecordClearTimeRPC(long clearTimeMilliseconds)
        {
            Debug.Log($"클리어 타임: {clearTimeMilliseconds}");

            if (clearTimeMilliseconds > int.MaxValue)
            {
                return;
            }

            SteamStatsAndAchievements.Instance.UpdateLevelClearStat(m_currentGameLevel, (int)clearTimeMilliseconds);
        }

#if UNITY_EDITOR || DEBUG
        private void OnGUI()
        {
            if (HasStateAuthority)
            {
                ClearTimeMilliseconds = m_stopwatch.ElapsedMilliseconds;
            }

            string timeText = TimeSpan.FromMilliseconds(ClearTimeMilliseconds).ToString(@"hh\:mm\:ss\.fff");
            GUI.Label(new Rect(10, 10, 200, 50), "Editor & Debug Only\n경과 시간: " + timeText);
        }
        
        [ContextMenu("수동 시작")] 
        public void ManualStart() => StartTimer();
        
        [ContextMenu("수동 종료")] 
        public void ManualStop() => StopTimer(true);
#endif
        
    }
}