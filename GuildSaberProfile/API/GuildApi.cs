using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GuildSaberProfile.Utils;
using GuildSaberProfile.Configuration;
using BS_Utils.Gameplay;
using System.Collections.Generic;
using GuildSaberProfile.UI.Card;

namespace GuildSaberProfile.API;

public static class GuildApi
{
    public static PlayerApiReworkOutput GetPlayerByScoreSaberIdAndGuild(string p_ID, string p_Guild)
    {
        PlayerApiReworkOutput l_ResultPlayer = new PlayerApiReworkOutput();
        using HttpClient l_Client = new HttpClient();

        try
        {
            Task<string> l_Result = null;
            l_Result = l_Client.GetStringAsync($"{GuildSaberUtils.ReturnLinkFromGuild(p_Guild)}/player/data/{p_ID}");
            l_Result.Wait();
            l_ResultPlayer = JsonConvert.DeserializeObject<PlayerApiReworkOutput>(l_Result.Result);
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

    public static ApiMapLeaderboardCollectionStruct GetLeaderboard(
        string p_Guild, string p_Hash, IDifficultyBeatmap p_Beatmap,
        string p_Page, string p_ScoreSaberId, string p_Country,
        string p_CountPerPage = null)
    {
        ApiMapLeaderboardCollectionStruct l_Leaderboard = new ApiMapLeaderboardCollectionStruct();
        using HttpClient l_Client = new HttpClient();
        try {
            Task<string> l_Result = null;
            l_Result = l_Client.GetStringAsync(
                $"{GuildSaberUtils.ReturnLinkFromGuild(p_Guild)}/mapleaderboard/{p_Hash}/{GuildSaberLeaderboardUtils.BeatmapDifficultyToDifficultyInOrder(p_Beatmap.difficulty)}/{p_Beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName}/{p_Page}/{p_ScoreSaberId}/{p_Country}/{p_CountPerPage}");
            l_Result.Wait();
            l_Leaderboard = JsonConvert.DeserializeObject<ApiMapLeaderboardCollectionStruct>(l_Result.Result);
        } catch(AggregateException p_E) {
            Plugin.Log.Debug(p_E.Message);
            return new();
        }
        return l_Leaderboard;
    }

    public static PlayerGuildsInfo GetPlayerInfoFromAPI(bool p_GuildFromConfig = true, string p_Guild = null)
    {
        /// We don't care if it return null because this function is loaded on the MenuSceneLoadedFresh, and the UserID will most likely be fetched way before that happen.
        #pragma warning disable CS0618
        string l_PlayerId = GetUserInfo.GetUserID();
        #pragma warning restore CS0618

        if (l_PlayerId.StringIsNullOrEmpty())
        { Plugin.Log.Error("Cannot get Player ID, not creating card"); return new PlayerGuildsInfo(); }

        Plugin.m_PlayerId = l_PlayerId;

        string l_SelectedGuild = (p_GuildFromConfig == true) ? PluginConfig.Instance.SelectedGuild : p_Guild;

        PlayerApiReworkOutput l_OutputPlayer = new PlayerApiReworkOutput();
        PlayerApiReworkOutput l_DefinedPlayer = new PlayerApiReworkOutput();
        PlayerApiReworkOutput l_LastValidPlayer = new PlayerApiReworkOutput();

        string l_LastValidGuild = string.Empty;

        List<string> l_TempAvailableGuilds = new List<string>
            { "CS", "BSCC" };
        Plugin.AvailableGuilds = new List<object>();

        for (int l_i = 0; l_i < l_TempAvailableGuilds.Count; l_i++)
        {
            l_OutputPlayer = GetPlayerByScoreSaberIdAndGuild(l_PlayerId, l_TempAvailableGuilds[l_i]);

            /*If Current Player from guild is valid setting l_LastValidPlayer to l_OutputPlayer and adding guild to AvailableGuilds,
            l_LastValidGuild is defined to the current guild too*/
            if (!l_OutputPlayer.Equals(null) && l_OutputPlayer.Level > 0)
            {
                l_LastValidPlayer = l_OutputPlayer;
                l_LastValidGuild = l_TempAvailableGuilds[l_i];
                Plugin.AvailableGuilds.Add(l_TempAvailableGuilds[l_i]);
            }

            //If the current guild in for is the selected, l_DefinedPlayer will be set to OutputPlayer (Current Player)
            if (l_TempAvailableGuilds[l_i] == l_SelectedGuild)
                l_DefinedPlayer = l_OutputPlayer;
        }

        //If there is no valid guilds returning empty PlayerGuildsInfo
        if (Plugin.AvailableGuilds.Count == 0) return new();

        //If the selected guild is not valid for current Player settings, settings SelectedGuild to l_LastValidGuild and DefinedPlayer to l_LastValidPlayer
        if (!GuildSaberUtils.IsGuildValidForPlayer(PluginConfig.Instance.SelectedGuild))
        {
            PluginConfig.Instance.SelectedGuild = l_LastValidGuild;
            l_DefinedPlayer = l_LastValidPlayer;
        }

        //If the processes succeffully end, returning l_DefinedPlayer and AvailableGuilds for Player
        return new(l_DefinedPlayer, Plugin.AvailableGuilds);
    }

    public static PlayerGuildsInfo GetPlayerInfoFromCurrent()
    {
        if (PlayerCardUI.m_Instance.CardViewController != null)
            return new PlayerGuildsInfo(PlayerCardUI.m_Player, Plugin.AvailableGuilds);
        else
            return new PlayerGuildsInfo();
    }
}
