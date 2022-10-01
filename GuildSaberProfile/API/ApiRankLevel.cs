using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.API
{
    public struct RankData
    {
        public string PointsType { get; set; }
        public string PointsName { get; set; }
        public float Points { get; set; }
        public int Rank { get; set; }
    }

    struct ApiRankingLevel
    {
        public int ID { get; set; }
        public int GuildID { get; set; }
        public float LevelNumber { get; set; }
        public bool IsObtainable { get; set; }
        public bool UseName { get; set; }
        public string Name { get; set; }
        public float DefaultWeight { get; set; }
        public string CoverB64 { get; set; }
        public int DiscordRoleID { get; set; }
        public string Description { get; set; }
        public Utils.Color.Color Color { get; set; }
    }
}
