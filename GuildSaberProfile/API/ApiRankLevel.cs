using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaberProfile.API
{
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
