using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.Utils
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

        public static int DifficultyToNumber(BeatmapDifficulty p_Difficulty)
        {
            switch (p_Difficulty)
            {
                case BeatmapDifficulty.Easy: return 1;
                case BeatmapDifficulty.Normal: return 3;
                case BeatmapDifficulty.Hard: return 5;
                case BeatmapDifficulty.Expert: return 7;
                case BeatmapDifficulty.ExpertPlus: return 9;
                default: return 0;
            }
        }

    }
}
