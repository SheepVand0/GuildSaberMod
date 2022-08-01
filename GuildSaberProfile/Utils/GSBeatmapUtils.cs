using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaberProfile.Utils
{
    class GSBeatmapUtils
    {
        public static string DifficultyBeatmapToString(IDifficultyBeatmap difficultyBeatmap)
        {
            if (difficultyBeatmap.level is CustomPreviewBeatmapLevel p_CustomLevel)
            {
                var hash = SongCore.Utilities.Hashing.GetCustomLevelHash(p_CustomLevel);
                var difficulty = difficultyBeatmap.difficulty.ToString();
                var characteristic = difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName;
                return $"{hash}%7C_{difficulty}_Solo{characteristic}";
            }

            return null;
        }

    }
}
