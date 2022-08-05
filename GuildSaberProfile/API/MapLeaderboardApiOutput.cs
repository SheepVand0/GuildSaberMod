using System;
using System.Collections.Generic;
using GuildSaberProfile.Utils.Color;

namespace GuildSaberProfile.API
{
    public struct ApiMapLeaderboardCollectionStruct
    {
        public List<ApiMapLeaderboardContentStruct> Leaderboards { get; set; }
        public ApiPageMetadataStruct Metadata { get; set; }
        public ApiCustomDataStruct CustomData { get; set; }
    }

   public struct ApiMapLeaderboardContentStruct
    {
        public UInt64 ScoreSaberID { get; set; }
        public int Rank { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Avatar { get; set; }
        public List<RankData> RankData;
        /// <summary>
        /// The weight is used to display how much points it actually really gave to the player (=> Earned Points = Points * Weight) (not used yet on BSDR, but it's something used on ScoreSaber)
        /// </summary>
        public float Weight { get; set; }
        public int BaseScore { get; set; }
        public int ModifiedScore { get; set; }
        public string Modifiers { get; set; }
        public float Multiplier { get; set; }
        public int BadCuts { get; set; }
        public int MissedNotes { get; set; }
        public int MaxCombo { get; set; }
        public bool FullCombo { get; set; }
        public int HMD { get; set; }
        public string TimeSet { get; set; }
    }

    public struct ApiPageMetadataStruct
    {
        public int Page { get; set; }
        public int MaxPage { get; set; }
        public int CountPerPage { get; set; }
    }

    public struct ApiCustomDataStruct
    {
        public int Level { get; set; }
        public Color Color { get; set; }
        public string Category { get; set; }
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
