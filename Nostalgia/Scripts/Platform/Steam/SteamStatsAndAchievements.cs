using System.Collections.Generic;
using Nostal.Util;
using Steamworks;
using UnityEngine;

namespace Nostal.Steam
{
    public class SteamStatsAndAchievements : Singleton<SteamStatsAndAchievements>
    {
        [SerializeField] private SteamStat[] m_stats;
        [SerializeField] private SteamAchievementSO[] m_achievements;

        private readonly Dictionary<ESteamStat, SteamStat> m_statsMap = new Dictionary<ESteamStat, SteamStat>();

        private CGameID m_gameID;

        // 스팀에서 Stats를 받아왔는지 확인
        private bool m_bStatsRequested;
        private bool m_bStatsValid;

        // Stats를 Store 해야 하는지 확인
        private bool m_bShouldStoreStats;
        
        private Callback<UserStatsReceived_t>     m_userStatsReceived;
        private Callback<UserStatsStored_t>       m_userStatsStored;
        private Callback<UserAchievementStored_t> m_userAchievementStored;
        
        public bool StatsValid => m_bStatsValid;

        protected override void Awake()
        {
            base.Awake();

            // 딕셔너리 값 할당
            foreach (SteamStat stat in m_stats)
            {
                if (!m_statsMap.TryAdd(stat.Stat, stat))
                {
                    Debug.LogError($"Steam 딕셔너리 초기화 - 중복 키({stat.Stat})", this);
                }
            }
        }

        private void OnEnable()
        {
            if (!SteamManager.Initialized)
            {
                return;
            }

            m_gameID = new CGameID(SteamUtils.GetAppID());
            
            m_userStatsReceived     = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
            m_userStatsStored       = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
            m_userAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);

            m_bStatsRequested = false;
            m_bStatsValid     = false;
        }

        private void Update()
        {
            if (!SteamManager.Initialized)
            {
                return;
            }

            if (!m_bStatsRequested)
            {
                // 스팀이 로드되지 않았으면 통계를 받을 수 없음. 끝냄.
                if (!SteamManager.Initialized)
                {
                    m_bStatsRequested = true;
                    return;
                }

                m_bStatsRequested = SteamUserStats.RequestCurrentStats();
            }

            // Stats를 정상적으로 받아왔는지 확인
            if (!m_bStatsValid)
            {
                return;
            }

            // 도전과제 클리어 여부 확인
            foreach (SteamAchievementSO ach in m_achievements)
            {
                if (ach.IsAchievedOnSteam)
                {
                    continue;
                }

                if (ach.IsAchieved)
                {
                    ach.IsAchievedOnSteam = true;
                    SteamUserStats.SetAchievement(ach.AchievementAPI);
                    m_bShouldStoreStats = true;
                }
            }
            
            if (!m_bShouldStoreStats)
            {
                return;
            }
            
            // Set Stats
            foreach (SteamStat stat in m_stats)
            {
                switch (stat.ValueType)
                {
                    case ESteamStatValueType.Int:
                        SteamUserStats.SetStat(stat.StatAPI, stat.IntValue);
                        break;
                    
                    case ESteamStatValueType.Float:
                        SteamUserStats.SetStat(stat.StatAPI, stat.FloatValue);
                        break;
                    
                    case ESteamStatValueType.AvgRate:
                        // TODO: AvgRate 타입의 SetStat 기능 추가
                        break;
                    
                    default:
                        Debug.LogError("SetStat - FAILED");
                        break;
                }
            }

            m_bShouldStoreStats = !SteamUserStats.StoreStats();
        }

        /// <summary>
        /// 레벨 클리어와 관련한 스팀 통계를 업데이트합니다. 베스트 클리어 타임과 레벨 클리어 횟수를 기록합니다.
        /// </summary>
        /// <param name="level">클리어 타임을 기록할 레벨. 튜토리얼, 레벨 1, 레벨 2, 추격 씬만 가능합니다.</param>
        /// <param name="newClearTime">새로운 클리어 타임</param>
        /// <returns>Stats를 정상적으로 받아오지 못했거나 올바른 ESteamStat가 아닌 경우에 false를 반환합니다.</returns>
        public bool UpdateLevelClearStat(NostalgiaGameLevel level, int newClearTime)
        {
            if (!m_bStatsValid)
            {
                return false;
            }

            ESteamStat bctStat;  // 베스트 클리어 타임
            ESteamStat ccStat;   // 클리어 카운트
            switch (level)
            {
                case NostalgiaGameLevel.Tutorial:
                    bctStat = ESteamStat.BestClearTime_Tutorial;
                    ccStat = ESteamStat.ClearCount_Tutorial;
                    break;
                
                case NostalgiaGameLevel.LevelOne:
                    bctStat = ESteamStat.BestClearTime_LevelOne;
                    ccStat = ESteamStat.ClearCount_LevelOne;
                    break;
                
                case NostalgiaGameLevel.LevelTwo:
                    bctStat = ESteamStat.BestClearTime_LevelTwo;
                    ccStat = ESteamStat.ClearCount_LevelTwo;
                    break;
                
                case NostalgiaGameLevel.Chase:
                    bctStat = ESteamStat.BestClearTime_Chase;
                    ccStat = ESteamStat.ClearCount_Chase;
                    break;
                
                default:
                    Debug.LogError("Update Best Clear Time - FAILED, 최단 시간 클리어 기록을 지원하는 레벨이 아닙니다.", this);
                    return false;
            }
            
            // 클리어 횟수 증가
            m_statsMap[ccStat].IntValue++;

            // 새로운 클리어 타임 업데이트
            int savedClearTime = m_statsMap[bctStat].IntValue;
            if (newClearTime < savedClearTime)
            {
                m_statsMap[bctStat].IntValue = newClearTime;
            }
            
            // Stats 업데이트 지시
            m_bShouldStoreStats = true;
            
            return true;
        }

        public void IncreaseEndingCreditsWatchedCount()
        {
            if (!m_bStatsValid)
            {
                return;
            }

            ++m_statsMap[ESteamStat.EndingCreditsWatchedCount].IntValue;

            // Stats 업데이트 지시
            m_bShouldStoreStats = true;
        }

        public SteamStat GetSteamStat(ESteamStat stat)
        {
            return m_statsMap[stat];
        }
        
        /// <summary>
        /// SteamUserStats.RequestCurrentStats()로 인해 정보를 수신하면 호출되는 콜백함수입니다.
        /// </summary>
        /// <param name="pCallback">콜백의 결과</param>
        private void OnUserStatsReceived(UserStatsReceived_t pCallback)
        {
            if (!SteamManager.Initialized)
            {
                return;
            }

            // 다른 스팀 앱에서 같은 콜백을 시도할 경우
            if (pCallback.m_nGameID != (ulong)m_gameID)
            {
                return;
            }

            if (pCallback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError("RequestStats - FAILED, " + pCallback.m_eResult, this);
                return;
            }

            // 모든 조건 통과
            // 도전과제 로드
            foreach (SteamAchievementSO ach in m_achievements)
            {
                if (!SteamUserStats.GetAchievement(ach.AchievementAPI, out ach.IsAchievedOnSteam))
                {
                    Debug.LogWarning(ach.AchievementAPI +
                                     " 도전과제에서 SteamUserStats.GetAchievement 가 실패했습니다. " +
                                     "\nSteam 파트너 사이트에 등록되어 있습니까?");
                }
            }
            
            // Steam Stats 받아오기
            foreach (SteamStat stat in m_stats)
            {
                bool bSuccess = stat.ValueType == ESteamStatValueType.Int
                    ? SteamUserStats.GetStat(stat.StatAPI, out stat.IntValue)
                    : SteamUserStats.GetStat(stat.StatAPI, out stat.FloatValue);

                if (!bSuccess)
                {
                    Debug.LogError($"GetStat - FAILED, '{stat.StatAPI}'가 올바른지 확인하세요.", this);
                }
            }
            
            m_bStatsValid = true;

            Debug.Log("RequestCurrentStats - SUCCESS", this);
        }

        private void OnUserStatsStored(UserStatsStored_t pCallback)
        {
            // 다른 스팀 앱에서 같은 콜백을 시도할 경우
            if (pCallback.m_nGameID != (ulong)m_gameID)
            {
                return;
            }

            switch (pCallback.m_eResult)
            {
                case EResult.k_EResultOK:
                    Debug.Log("StoreStats - SUCCESS", this);
                    break;
                
                case EResult.k_EResultInvalidParam:
                    Debug.Log("StoreStats - Some failed to validate", this);
                    UserStatsReceived_t callback = new UserStatsReceived_t
                    {
                        m_eResult = EResult.k_EResultOK,
                        m_nGameID = (ulong)m_gameID
                    };
                    OnUserStatsReceived(callback);
                    break;
                
                default:
                    Debug.LogError("StoreStats - FAILED, " + pCallback.m_eResult, this);
                    break;
            }
        }

        private void OnAchievementStored(UserAchievementStored_t pCallback)
        {
            if (pCallback.m_nGameID != (ulong)m_gameID)
            {
                return;
            }
            
            if (0 == pCallback.m_nMaxProgress) 
            {
                Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
            }
            else 
            {
                Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
            }
        }
        
#if UNITY_EDITOR
        public void ResetAllStats()
        {
            if (!m_bStatsValid)
            {
                return;
            }

            SteamUserStats.ResetAllStats(true);
            SteamUserStats.RequestCurrentStats();
        }
#endif
    }
}