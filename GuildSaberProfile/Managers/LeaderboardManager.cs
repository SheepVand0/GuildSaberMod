using GuildSaberProfile.UI.GuildSaber.Leaderboard;

namespace GuildSaberProfile.Managers
{
    class LeaderboardManager
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
