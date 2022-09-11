using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaberProfile.API
{
    public struct MapLeaderboardContent
    {
        public int ID { get; set; }
        public int ScoreSaberID { get; set; }
        public int BeatLeaderID { get; set; }
        public int Rank { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Avatar { get; set; }
        public List<RankData> RankData { get; set; }
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
        public int UnixTimeSet { get; set; }
    }

    public struct MapLeaderboardCustomData
    {
        public int RankedDifficultyID { get; set; }
        public int LevelID { get; set; }
        public string LevelName { get; set; }
        public float LevelValue { get; set; }
        public Utils.Color.Color LevelColor { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int MaxScore { get; set; }

    }

    public struct ApiMapLeaderboardCollection
    {
        public List<MapLeaderboardContent> Leaderboards { get; set; }
        public PageMetaData Metadata { get; set; }
        public MapLeaderboardCustomData CustomData { get; set; }
    }
}
