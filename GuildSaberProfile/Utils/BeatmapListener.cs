using UnityEngine;
using GuildSaberProfile.UI.GuildSaber.Leaderboard;

namespace GuildSaberProfile.Utils
{
    class BeatmapListener
    {
        public BeatmapListener(IDifficultyBeatmap p_Beatmap)
        {
            Resources.FindObjectsOfTypeAll<GuildSaberLeaderboardView>()[0].m_CurrentBeatmap = p_Beatmap;
        }
    }
}
