﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace GuildSaber.API
{
#nullable enable
    public struct ApiPlayerData
    {
        public UInt32 ID { get; set; }
        public string? ScoreSaberID { get; set; }
        public string? BeatLeaderID { get; set; }
        public string? DiscordUserID { get; set; }
        public string Name { get; set; }
        public string? Country { get; set; }
        public string LastScoreTime { get; set; }
        public UInt32 TotalPlayCount { get; set; }
        public UInt32? GuildValidPassCount { get; set; }
        public string Avatar { get; set; }

        public Utils.Color.Color? Color { get; set; }

        public List<Badge> Badges { get; set; }
        public List<Trophy>? Trophies { get; set; }
        public float? LevelValue { get; set; }
        public UInt32? LevelID { get; set; }
        public bool IsBanned { get; set; }
        public List<RankData>? RankData;
        public List<ApiAPlayerCategory>? CategoryData { get; set; }
    }

    public struct RankData
    {
        public string PointsType { get; set; }
        public string PointsName { get; set; }
        public float Points { get; set; }
        public UInt32 Rank { get; set; }
    }

    public struct Badge
    {
        public string description { get; set; }
        public string image { get; set; }
    }

    public struct Trophy
    {
        public UInt32 ID { get; set; }
        public UInt32? CategoryID { get; set; }
        public string? Logo { get; set; }
        public UInt32 Count { get; set; }
    }

    public struct ApiAPlayerCategory
    {
        public string CategoryName { get; set; }
        public UInt32 CategoryID { get; set; }
        public float? LevelValue { get; set; }
        public UInt32? LevelID { get; set; }
        public float? MaxLevelValue { get; set; }
        public UInt32? MaxLevelID { get; set; }
        /*public int NumberOfPass { get; set; }
        public int TotalNumberOfMaps { get; set; }*/
        public List<Trophy> Trophies { get; set; }
    }
}
