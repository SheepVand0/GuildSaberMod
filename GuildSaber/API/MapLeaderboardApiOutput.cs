using System;
using System.Collections.Generic;
using GuildSaber.Utils.Color;
using Newtonsoft.Json;

namespace GuildSaber.API
{
#nullable enable
    public struct ApiMapLeaderboardCollectionStruct
    {
        //TODO: Make some adjust inside ContentStruct, with a new struct for only the player info + add the GuildRankedDifficultyID
        public List<ApiMapLeaderboardContentStruct> Leaderboards { get; set; }
        public ApiPageMetadataStruct Metadata { get; set; }
        public ApiCustomDataStruct CustomData { get; set; }
        public ApiMapLeaderboardContentStruct? PlayerScore { get; set; }
    }

    public struct PointsData
    {
        public string PointsType { get; set; }
        public string PointsName { get; set; }
        public float Points { get; set; }
    }

    public struct ApiMapLeaderboardContentStruct
    {
        public UInt32 ID { get; set; }
        public string? ScoreSaberID { get; set; }
        public string? BeatLeaderID { get; set; }
        public UInt32 Rank { get; set; }
        public string Name { get; set; }
        public string? Country { get; set; }
        public string Avatar { get; set; }
        public UInt32 State { get; set; }
        public List<PointsData> PointsData;
        /// <summary>
        /// The weight is used to display how much points it actually really gave to the player (=> Earned Points = Points * Weight) (not used yet on BSDR, but it's something used on ScoreSaber)
        /// </summary>
        public float Weight { get; set; }
        public UInt32 BaseScore { get; set; }
        public UInt32 ModifiedScore { get; set; }
        public string Modifiers { get; set; }
        public float Multiplier { get; set; }
        public UInt32 BadCuts { get; set; }
        public UInt32 MissedNotes { get; set; }
        public UInt32 MaxCombo { get; set; }
        public bool FullCombo { get; set; }
        public UInt32 HMD { get; set; }
        public string UnixTimeSet { get; set; }
        public bool HasReplay { get; set; }
        public string? ReplayViewerURL { get; set; }
        public bool HasScoreStatistic { get; set; }
        public ApiScoreStatistic? ScoreStatistic { get; set; }
    }

    public struct ApiScoreStatistic
    {
        public float LeftPreSwing { get; set; }
        public float LeftPureAcc { get; set; }
        public float LeftPostSwing { get; set; }
        public float RightPreSwing { get; set; }
        public float RightPureAcc { get; set; }
        public float RightPostSwing { get; set; }
        public UInt32 PauseCount { get; set; }
    }

    public struct ApiPageMetadataStruct
    {
        public UInt32 Page { get; set; }
        public UInt32 MaxPage { get; set; }
        public UInt32 CountPerPage { get; set; }
        public UInt32 TotalCount { get; set; }
    }

    public struct ApiCustomDataStruct
    {
        public UInt32 RankedDifficultyID { get; set; }
        public UInt32 LevelID { get; set; }
        public string LevelName { get; set; }
        public float LevelValue { get; set; }

        public Color LevelColor { get; set; }

        public UInt32? CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public UInt32 MaxScore { get; set; }
    }

    public static class GuildSaberLeaderboardUtils
    {
        public static UInt32 StringToDifficulty(string p_Input)
        {
            return p_Input switch
            {
                "Easy" => 1,
                "Normal" => 3,
                "Hard" => 5,
                "Expert" => 7,
                "ExpertPlus" => 9,
                _ => 0
            };
        }

        public static UInt32 BeatmapDifficultyToDifficultyInOrder(BeatmapDifficulty p_Difficulty)
        {

            return (UInt32)p_Difficulty switch
            {
                0 => 1,
                1 => 3,
                2 => 5,
                3 => 7,
                4 => 9,
                _ => 0
            };

        }

        public static string DifficultyToString(UInt32 p_Input)
        {
            return p_Input switch
            {
                1 => "Easy",
                3 => "Normal",
                5 => "Hard",
                7 => "Expert",
                9 => "ExpertPlus",
                _ => string.Empty
            };
        }
    }
}
