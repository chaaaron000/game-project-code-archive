using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nostal.Sound
{
    public class BGMController : MonoBehaviour
    {
        [Header("Audio Clip Table")]
        [SerializeField] private AudioClipTable m_bgmClipTable;

        [Header("Audio Source Components")] 
        [Tooltip("플레이 도중 기본적으로 깔릴 Ambient 음원을 재생할 AudioSource")]
        [SerializeField] private AudioSource m_ambientAudioSource;
        
        [Tooltip("추격, 동료 사망과 같은 특정 상황의 BGM을 재생하는 AudioSource")]
        [SerializeField] private AudioSource m_situationAudioSource;

        private Dictionary<string, AudioClip> m_bgmDictionary;
        private string m_nowPlayingBGM = "";
        private Coroutine m_playerDiedBGMCoroutine;

        private void Awake()
        {
            m_bgmDictionary = m_bgmClipTable.ToDictionary();
        }

        private void OnEnable()
        {
            GameplayEventManager.GameOver += OnGameOver;
            GameplayEventManager.PlayerChaseStarted += OnPlayerChaseStarted;
            GameplayEventManager.PlayerChaseEnded += OnPlayerChaseEnded;
            GameplayEventManager.PlayerRevived += OnPlayerRevived;
            GameplayEventManager.JumpscareEnded += OnJumpscareEnded;
            GameplayEventManager.StoperItemStarted += PlayClockTicking;
            GameplayEventManager.StoperItemEnded += StopClockTicking;
            GameplayEventManager.TutorialMapReset += OnTutorialReset;
        }
        
        private void Start()
        {
            m_situationAudioSource.Stop();
            
            m_ambientAudioSource.clip = m_bgmDictionary["Ambient"];
            m_ambientAudioSource.loop = true;
            m_ambientAudioSource.Play();
        }

        private void OnDisable()
        {
            GameplayEventManager.GameOver -= OnGameOver;
            GameplayEventManager.PlayerChaseStarted -= OnPlayerChaseStarted;
            GameplayEventManager.PlayerChaseEnded -= OnPlayerChaseEnded;
            GameplayEventManager.PlayerRevived -= OnPlayerRevived;
            GameplayEventManager.JumpscareEnded -= OnJumpscareEnded;
            GameplayEventManager.StoperItemStarted -= PlayClockTicking;
            GameplayEventManager.StoperItemEnded -= StopClockTicking;
            GameplayEventManager.TutorialMapReset -= OnTutorialReset;
        }

        private void OnGameOver(bool bIsClear)
        {
            if (bIsClear)
            {
                return;
            }
            
            if (m_playerDiedBGMCoroutine != null)
            {
                StopCoroutine(m_playerDiedBGMCoroutine);
            }
            
            StopBGM();
            PlayBGM("GameOver");
        }

        private void OnPlayerChaseStarted(PlayerRef chasedPlayerRef)
        {
            PlayBGM("Chasing");
        }

        private void OnPlayerChaseEnded(PlayerRef chasedPlayerRef)
        {
            StopBGM("Chasing");
        }

        private void OnPlayerRevived(PlayerRef revivedPlayerRef)
        {
            if (m_playerDiedBGMCoroutine != null)
            {
                StopCoroutine(m_playerDiedBGMCoroutine);
            }
            
            StopBGM("PlayerDied");
        }

        private void OnJumpscareEnded(PlayerRef scaredPlayerRef)
        {
            PlayBGM("PlayerDied");

            if (m_playerDiedBGMCoroutine != null)
            {
                StopCoroutine(m_playerDiedBGMCoroutine);
            }
            
            m_playerDiedBGMCoroutine = StartCoroutine(PlayerDiedBGMCoroutine());
        }
        
        private void PlayBGM(string audioClipName, bool loop = true)
        {
            if (audioClipName == m_nowPlayingBGM && m_situationAudioSource.isPlaying)
            {  
                return;
            }

            if (!m_bgmDictionary.ContainsKey(audioClipName))
            {
                Debug.LogError(audioClipName + " 이 BGM 딕셔러니에 없습니다.");
                return;
            }
            
            m_nowPlayingBGM = audioClipName;
            
            m_situationAudioSource.Stop();
            m_situationAudioSource.clip = m_bgmDictionary[audioClipName];
            m_situationAudioSource.loop = loop;
            m_situationAudioSource.Play();
        }

        private void StopBGM()
        {
            m_situationAudioSource.Stop();
        }

        private void StopBGM(string audioClipName)
        {
            if (m_situationAudioSource.isPlaying && m_situationAudioSource.clip == m_bgmDictionary[audioClipName])
            {
                m_situationAudioSource.Stop();
            }
        }

        private IEnumerator PlayerDiedBGMCoroutine()
        {
            for (;;)
            {
                yield return new WaitWhile(() => !m_situationAudioSource.isPlaying);
                yield return new WaitForSeconds(0.5f);
                
                if (m_situationAudioSource.isPlaying)
                {
                    continue;
                }
                
                PlayBGM("PlayerDied"); 
            }
        }
        
        private IEnumerator WaitToEndCoroutine(Action onComplete)
        {
            yield return new WaitWhile(() => m_ambientAudioSource.isPlaying);
            onComplete?.Invoke();
        }

        private void PlayClockTicking()
        {
            if (m_ambientAudioSource.clip == m_bgmDictionary["ClockTicking"])
            {
                return;
            }
            
            m_ambientAudioSource.Stop();
            m_ambientAudioSource.clip = m_bgmDictionary["ClockTicking"];
            m_ambientAudioSource.Play();
        }

        private void StopClockTicking()
        {
            if (m_ambientAudioSource.clip == m_bgmDictionary["Ambient"])
            {
                return;
            }
            
            m_ambientAudioSource.Stop();
            m_ambientAudioSource.clip = m_bgmDictionary["Ambient"];
            m_ambientAudioSource.Play();
        }
        
        private void OnTutorialReset()
        {
            if (m_playerDiedBGMCoroutine != null) 
            {
                StopCoroutine(m_playerDiedBGMCoroutine);
                m_playerDiedBGMCoroutine = null;
            }

            StopBGM();
            // m_situationAudioSource.Stop();
            // m_situationAudioSource.clip = null;

            m_ambientAudioSource.Stop();
            m_ambientAudioSource.clip = m_bgmDictionary["Ambient"];
            m_ambientAudioSource.loop = true;
            m_ambientAudioSource.Play();
        }
    }
}