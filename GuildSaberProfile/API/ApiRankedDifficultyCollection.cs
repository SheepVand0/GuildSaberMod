using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaberProfile.API
{
    public struct RankedDifficulty
    {
        public int ID { get; set; }
        public int GuildID { get; set; }
        public int GuildRankingLevelId { get; set; }
        public int SongDifficultyId { get; set; }
        public int MaxScore { get; set; }
        public int GuildCategoryID { get; set; }
        public int MinScoreRequirement { get; set; }
        public int ProhibitedModifiers { get; set; }
        public int MandatoryModifiers { get; set; }
        public float Weight { get; set; }
        public float AutoWeight { get; set; }
        public float ManualWeight { get; set; }
        public float EnableManualWeight { get; set; }
        public int UnixRankedTime { get; set; }
        public int UnixEditedTime { get; set; }
    }

    public struct SongDiffucilty
    {
        public int ID { get; set; }
        public int SongID { get; set; }
        public int SSID { get; set; }
        public string BLID { get; set; }
        public int GameMode { get; set; }
        public int Difficulty { get; set; }
        public int MaxScore { get; set; }
    }

    public struct Song
    {
        public int ID { get; set; }
        public string Hash { get; set; }
        public string BeatSaverID { get; set; }
        public string Name { get; set; }
        public string SongName { get; set; }
        public string SongSubName { get; set; }
        public string SongAuthor { get; set; }
        public string Mapper { get; set; }
    }

    public struct RankedMapsData
    {
        public RankedDifficulty RankedDifficulty { get; set; }
        public SongDiffucilty SongDiffucilty { get; set; }
        public Song Song { get; set; }
    }

    public struct PageMetaData
    {
        public int Page { get; set; }
        public int MaxPage { get; set; }
        public int CountPerPage { get; set; }
        public int TotalCount { get; set; }
    }

    public struct ApiRankedDifficultyCollection
    {
        public List<RankedMapsData> RankedMaps { get; set; }
        public PageMetaData Metadata { get; set; }
    }
}
