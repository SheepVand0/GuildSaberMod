using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.API
{

    struct ApiRankingLevel
    {
        public UInt32 ID { get; set; }
        public UInt32 GuildID { get; set; }
        public float LevelNumber { get; set; }
        public bool IsObtainable { get; set; }
        public bool UseName { get; set; }
        public string Name { get; set; }
        public float DefaultWeight { get; set; }
        public string? CoverB64 { get; set; }
        public UInt64? DiscordRoleID { get; set; }
        public string? Description { get; set; }
        public UInt32 Color { get; set; }
    }
}
