using System.Linq;
using Fusion;
using UnityEngine;

namespace Nostal.Steam
{
    public enum ESteamAchievement
    {
        ACH_FIRST_CLEAR_TUTORIAL,
        ACH_FIRST_CLEAR_LEVEL_ONE,
        ACH_FIRST_CLEAR_LEVEL_TWO,
        ACH_FIRST_CLEAR_CHASE,
        ACH_ENDING_CREDITS_WATCHED,
    }
    
    [CreateAssetMenu(fileName = "SO_Achievement_", menuName = "Scriptable Object/도전과제", order = 4)]
    public class SteamAchievementSO : ScriptableObject
    {
        [SerializeField] private ESteamAchievement m_achievementID;
        [SerializeField] private AchievementCondition[] m_achievementConditions;

        public string AchievementAPI => m_achievementID.ToString();
        
        /// <summary>
        /// 스팀 데이터베이스에 저장된 달성 여부
        /// </summary>
        public bool IsAchievedOnSteam = false;
        
        /// <summary>
        /// 도전과제 조건이 달성 여부를 반환합니다. 
        /// </summary>
        public bool IsAchieved => m_achievementConditions.All(condition => condition.IsConditionMet());
        
        [System.Serializable]
        private class AchievementCondition
        {
            public ESteamStat TargetStat;
            public ECompareOp Operator;
            public double GoalValue;

            public bool IsConditionMet()
            {
                if (SteamStatsAndAchievements.Instance == null)
                {
                    return false;
                }
                
                SteamStat stat = SteamStatsAndAchievements.Instance.GetSteamStat(TargetStat);
                double currentStatValue = stat.ValueType == ESteamStatValueType.Int ? stat.IntValue : stat.FloatValue;
                
                switch (Operator)
                {
                    case ECompareOp.Equal:
                        return currentStatValue == GoalValue;
                    case ECompareOp.NotEqual:
                        return currentStatValue != GoalValue;
                    case ECompareOp.Greater:
                        return currentStatValue > GoalValue;
                    case ECompareOp.GreaterOrEqual:
                        return currentStatValue >= GoalValue;
                    case ECompareOp.Less:
                        return currentStatValue < GoalValue;
                    case ECompareOp.LessOrEqual:
                        return currentStatValue <= GoalValue;
                    
                    default:
                        return false;
                }
            }
        }
        
        private enum ECompareOp
        {
            Equal,
            NotEqual,
            Greater,
            GreaterOrEqual,
            Less,
            LessOrEqual
        }
    }
}