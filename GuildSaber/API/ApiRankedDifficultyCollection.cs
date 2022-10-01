using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.API
{
    public struct RankedDifficultyCollection
    {
        public List<RankedMapData> RankedMaps { get; set; }
        public ApiPageMetadataStruct Metadata { get; set; }

        public struct RankedMapData
        {
            public UInt32 MapID { get; set; }
            public string? MapName { get; set; }
            public string? MapSubName { get; set; }
            public string? MapAuthorName { get; set; }
            public string? Mapper { get; set; }
            public string? BeatSaverID { get; set; }
            public string BeatSaverHash { get; set; }
            public string CoverURL { get; set; }
            public bool IsAutoMapped { get; set; }
            public float BPM { get; set; }
            public UInt32 Duration { get; set; }
            public string? UnixUploadedTime { get; set; }

            public List<RankedDifficultyData> Difficulties { get; set; }
        }

        public struct RankedDifficultyData
        {
            public UInt32 GameModeValue { get; set; }
            public string? GameModeName { get; set; }
            public UInt32 BeatSaverDifficultyValue { get; set; }
            public string BeatSaverDifficultyName { get; set; }
            public UInt32 DifficultyID { get; set; }
            public UInt32 LevelID { get; set; }
            public UInt32? GuildCategoryID { get; set; }
            public UInt32 MaxScore { get; set; }
            public float NoteJumpSpeed { get; set; }
            public UInt32 NoteCount { get; set; }
            public UInt32 BombCount { get; set; }
            public UInt32 ObstacleCount { get; set; }
            public float NotesPerSecond { get; set; }
            public float Seconds { get; set; }
            public UInt32 MinScoreRequirement;
            public string ProhibitedModifiers;
            public string MandatoryModifiers;
            public float PassWeight { get; set; }
            public float AccWeight { get; set; }
            public float PureWeight { get; set; }
            public string UnixRankedTime { get; set; }
            public string UnixEditedTime { get; set; }
            public bool HasBestReplay { get; set; }
            public string? ReplayViewerURL { get; set; }
            public UInt32? BestReplayPlayerID { get; set; }
        }
    }

    public struct RankedMapCollection
    {
        public List<RankedDifficultyCollection.RankedMapData> RankedMaps { get; set; }
        public PageMetaData Metadata { get; set; }
    }

    public struct PageMetaData
    {
        public int Page { get; set; }
        public int MaxPage { get; set; }
        public int CountPerPage { get; set; }
        public int TotalCount { get; set; }
    }
}
