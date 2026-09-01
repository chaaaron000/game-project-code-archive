using System;

namespace Nostal.Steam
{
    public enum ESteamStat
    {
        BestClearTime_Tutorial,
        BestClearTime_LevelOne,
        BestClearTime_LevelTwo,
        BestClearTime_Chase,
        ClearCount_Tutorial,
        ClearCount_LevelOne,
        ClearCount_LevelTwo,
        ClearCount_Chase,
        EndingCreditsWatchedCount,
    }

    public enum ESteamStatValueType
    {
        Int,
        Float,
        AvgRate
    }

    [Serializable]
    public class SteamStat
    {
        public ESteamStat Stat;
        public ESteamStatValueType ValueType;
        public int IntValue;
        public float FloatValue;

        public string StatAPI => Stat.ToString();
    }
}