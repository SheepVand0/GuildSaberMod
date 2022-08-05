using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaberProfile.Utils
{
    class GSBeatmapUtils
    {
        public static string DifficultyBeatmapToHash(IDifficultyBeatmap difficultyBeatmap)
        {
            if (difficultyBeatmap.level is CustomPreviewBeatmapLevel p_CustomLevel)
            {
                var l_Hash = SongCore.Utilities.Hashing.GetCustomLevelHash(p_CustomLevel);
                return l_Hash;
            }
            return null;
        }

    }
}
