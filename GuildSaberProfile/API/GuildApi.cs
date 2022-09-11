using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GuildSaberProfile.Utils;
using GuildSaberProfile.Configuration;
using BS_Utils.Gameplay;
using System.Collections.Generic;
using GuildSaberProfile.UI.Card;
using CP_SDK.Network;

namespace GuildSaberProfile.API;

public static class GuildApi
{
    public static ApiPlayerData GetPlayerByScoreSaberIdAndGuild(string p_ID, int p_Guild)
    {
        ApiPlayerData l_ResultPlayer = default(ApiPlayerData);
        using HttpClient l_Client = new HttpClient();

        try
        {
            Task<string> l_Result = null;
            l_Result = l_Client.GetStringAsync($"https://api.guildsaber.com/player/data/by-ssid/{p_ID}/full?guild={p_Guild}");
            l_Result.Wait();
            l_ResultPlayer = JsonConvert.DeserializeObject<ApiPlayerData>(l_Result.Result);
        }
        catch (AggregateException l_AggregateException)
        {
            if (l_AggregateException.InnerException is HttpRequestException)
            {
                Plugin.Log.Error("Error during getting Player data");
                return new();
            }
        }

        return l_ResultPlayer;
    }

    public static ApiMapLeaderboardCollection GetLeaderboard(
        int p_Guild, string p_Hash, IDifficultyBeatmap p_Beatmap,
        int p_Page, int p_ScoreSaberId, string p_Country,
        int p_CountPerPage = 10)
    {
        ApiMapLeaderboardCollection l_Leaderboard = default(ApiMapLeaderboardCollection);
        using HttpClient l_Client = new HttpClient();
        try {
            Task<string> l_Result = null;
            string l_ScoreSaberID = (p_ScoreSaberId != 0) ? $"&player-ssid={p_ScoreSaberId}" : string.Empty;
            l_Result = l_Client.GetStringAsync(
                $"https://api.guildsaber.com/maps/leaderboard/by-hash/{p_Hash}/9?guild={p_Guild}&page={p_Page}&countperpage={p_CountPerPage}{l_ScoreSaberID}");
            l_Result.Wait();
            l_Leaderboard = JsonConvert.DeserializeObject<ApiMapLeaderboardCollection>(l_Result.Result);
        } catch(AggregateException p_E) {
            Plugin.Log.Debug(p_E.Message);
            return new();
        }
        return l_Leaderboard;
    }

    public static PlayerGuildsInfo GetPlayerInfoFromAPI(bool p_GuildFromConfig = true, int p_GuildId = 0)
    {
        /// We don't care if it return null because this function is loaded on the MenuSceneLoadedFresh, and the UserID will most likely be fetched way before that happen.
        #pragma warning disable CS0618
        long l_PlayerId = long.Parse(GetUserInfo.GetUserID());
        #pragma warning restore CS0618

        if (l_PlayerId == 0)
        { Plugin.Log.Error("Cannot get Player ID, not creating card"); return new PlayerGuildsInfo(); }

        GuildSaber.m_SSPlayerId = l_PlayerId;

        int l_SelectedGuild = (p_GuildFromConfig == true) ? GSConfig.Instance.SelectedGuild : p_GuildId;

        ApiPlayerData l_DefinedPlayer = default(ApiPlayerData);
        List<GuildData> l_AvailableGuilds = new List<GuildData>();

        try
        {
            using (HttpClient l_Client = new HttpClient())
            {
                if (GuildSaber.m_GSPlayerId == null)
                {
                    Task<string> l_SerializedPlayer = l_Client.GetStringAsync($"https://api.guildsaber.com/player/data/by-ssid/{GuildSaber.m_SSPlayerId}/full");
                    l_SerializedPlayer.Wait();
                    ApiPlayerData l_Player = JsonConvert.DeserializeObject<ApiPlayerData>(l_SerializedPlayer.Result);
                    GuildSaber.m_GSPlayerId = l_Player.ID;
                }

                Task<string> l_Serialized = l_Client.GetStringAsync($"https://api.guildsaber.com/player/data/by-ssid/{GuildSaber.m_GSPlayerId}/full?guild={l_SelectedGuild}");
                l_Serialized.Wait();
                l_DefinedPlayer = JsonConvert.DeserializeObject<ApiPlayerData>(l_Serialized.Result);
            }

        } catch (Exception l_E)
        {

        }

        Plugin.AvailableGuilds.Clear();
        foreach (GuildData l_Current in l_AvailableGuilds)
        {
            Plugin.AvailableGuilds.Add(l_Current);
        }

        return new(l_DefinedPlayer, l_AvailableGuilds);
    }

    public static PlayerGuildsInfo GetPlayerInfoFromCurrent()
    {
        if (PlayerCardUI.m_Instance.CardViewController != null)
            return new PlayerGuildsInfo(PlayerCardUI.m_Player, Plugin.AvailableGuilds);
        else
            return new PlayerGuildsInfo();
    }
}
