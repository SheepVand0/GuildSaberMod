using SongCore.Utilities;

namespace GuildSaber.Utils
{
    internal class GSBeatmapUtils
    {
        public static string DifficultyBeatmapToHash(IDifficultyBeatmap difficultyBeatmap)
        {
            if (difficultyBeatmap.level is CustomPreviewBeatmapLevel p_CustomLevel)
            {
                string l_Hash = Hashing.GetCustomLevelHash(p_CustomLevel);
                return l_Hash;
            }
            return null;
        }

        public static int DifficultyToNumber(BeatmapDifficulty p_Difficulty)
        {
            return p_Difficulty switch
            {
                BeatmapDifficulty.Easy => 1,
                BeatmapDifficulty.Normal => 3,
                BeatmapDifficulty.Hard => 5,
                BeatmapDifficulty.Expert => 7,
                BeatmapDifficulty.ExpertPlus => 9,
                _ => 0
            };
        }
    }
}
