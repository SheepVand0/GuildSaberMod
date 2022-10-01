using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace GuildSaber.API
{
    public struct Badges
    {
        public string description { get; set; }
        public string image { get; set; }
    }
    public struct Trophy
    {
        public int? ID { get; set; }
        public int? CategoryID { get; set; }
        public string? Logo { get; set; }
        public int? Count { get; set; }
    }
    public struct CategoryData
    {
        public string CategoryName { get; set; }
        public int CategoryID { get; set; }
        public float LevelValue { get; set; }
        public int LevelID { get; set; }
        public float MaxLevelValue { get; set; }
        public int MaxLevelID { get; set; }
        public List<Trophy> Trophies { get; set; }
    }
    public struct ApiPlayerData
    {
        public int? ID { get; set; }
        public string? ScoreSaberID { get; set; }
        public string? BeatLeaderID { get; set; }
        public string? DiscordUserID { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public int? LastScoreTime { get; set; }
        public int? TotalPlayCount { get; set; }
        public int? GuildPassCount { get; set; }
        public string? Avatar { get; set; }
        public Utils.Color.Color? Color { get; set; }
        public List<Badges>? Badges { get; set; }
        public List<Trophy>? Trophies { get; set; }
        public float? LevelValue { get; set; }
        public int? LevelID { get; set; }
        public bool? IsBanned { get; set; }
        public List<RankData>? RankData { get; set; }
        public List<CategoryData>? CategoryData { get; set; }
    }
}
