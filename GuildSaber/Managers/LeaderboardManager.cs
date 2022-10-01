using GuildSaber.UI.GuildSaber.Leaderboard;
using UnityEngine;

namespace GuildSaber.Managers
{
    class LeaderboardManager : MonoBehaviour
    {
        public string l_CurrentSongHash;

        public readonly GuildSaberCustomLeaderboard _GuildSaberCustomLeaderboard;

        public LeaderboardManager(GuildSaberCustomLeaderboard p_GuildSaberCustomLeaderboard)
        {
            _GuildSaberCustomLeaderboard = p_GuildSaberCustomLeaderboard;
        }

        public void RefreshLeaderboard(string p_Hash)
        {
            l_CurrentSongHash = p_Hash;
        }

        public static readonly int LeaderboardCellsCount = 10;
    }
}
