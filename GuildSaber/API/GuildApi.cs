using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GuildSaber.Utils;
using GuildSaber.Configuration;
using BS_Utils.Gameplay;
using System.Collections.Generic;
using GuildSaber.UI.Card;
using CP_SDK.Network;
using OVR.OpenVR;
using GuildSaber.UI.CustomLevelSelection;
using GuildSaber.BSPModule;
using System.Security.Cryptography;
using UnityEngine;
using GuildSaber.Logger;

namespace GuildSaber.API;

public static class GuildApi
{
    /// <summary>
    /// Get player and all guilds
    /// </summary>
    /// <returns></returns>
    public static PlayerGuildsInfo GetPlayerGuildsInfo()
    {
        ApiPlayerData l_Player = default;
        List<GuildData> l_Guilds = new();
        try
        {
            ApiPlayerData l_NoGuildsPlayer = GuildApi.GetPlayerInfoFromAPI(p_UseGuild: false);

            using (HttpClient l_Client = new())
            {
                Task<string> l_SerializedGuilds = l_Client.GetStringAsync($"https://api.guildsaber.com/guilds/data/all?player-id={l_NoGuildsPlayer.ID}");
                l_SerializedGuilds.Wait();
                ApiGuildCollection l_GuildCollection = JsonConvert.DeserializeObject<ApiGuildCollection>(l_SerializedGuilds.Result);
                l_Guilds = l_GuildCollection.Guilds;
                l_Player = l_NoGuildsPlayer;
            }
            GuildSaberModule.ModState = GuildSaberModule.EModState.Fonctionnal;
        } catch (Exception l_E)
        {
            GSLogger.Instance.Error(l_E, nameof(GuildApi), nameof(GetPlayerGuildsInfo));
            GuildSaberModule.ModState = GuildSaberModule.EModState.APIError;
            return new(default, new List<GuildData>());
        }
        return new(l_Player, l_Guilds);
    }

    /// <summary>
    /// Get player by score saber id and a guild id
    /// </summary>
    /// <param name="p_ID"></param>
    /// <param name="p_Guild"></param>
    /// <returns></returns>
    public static ApiPlayerData GetPlayerByScoreSaberIdAndGuild(string p_ID, int p_Guild)
    {
        ApiPlayerData l_ResultPlayer = default(ApiPlayerData);
        using HttpClient l_Client = new HttpClient();

        try
        {
            Task<string> l_Result = l_Client.GetStringAsync($"https://api.guildsaber.com/player/data/by-ssid/{p_ID}/full?guild={p_Guild}");
            l_Result.Wait();
            l_ResultPlayer = JsonConvert.DeserializeObject<ApiPlayerData>(l_Result.Result);
            GuildSaberModule.ModState = GuildSaberModule.EModState.Fonctionnal;
        }
        catch (AggregateException l_AggregateException)
        {
            GuildSaberModule.ModState = GuildSaberModule.EModState.APIError;
            if (l_AggregateException.InnerException is HttpRequestException)
            {
                GSLogger.Instance.Error(l_AggregateException, nameof(GuildApi), nameof(GetPlayerByScoreSaberIdAndGuild));
                return new();
            }
        }

        return l_ResultPlayer;
    }

    /// <summary>
    /// Get leaderboard from hash
    /// </summary>
    /// <param name="p_Guild">GuildID</param>
    /// <param name="p_Hash">Hash</param>
    /// <param name="p_Beatmap">Difficulty beatmap</param>
    /// <param name="p_Page">Page</param>
    /// <param name="p_ScoreSaberId">Player scoresaber ID</param>
    /// <param name="p_Country">Country</param>
    /// <param name="p_CountPerPage">Score count per page</param>
    /// <returns></returns>
    public static ApiMapLeaderboardCollectionStruct GetLeaderboard(
        int p_Guild, string p_Hash, IDifficultyBeatmap p_Beatmap,
        int p_Page, ulong p_ScoreSaberId, string p_Country,
        int p_CountPerPage = 10)
    {
        ApiMapLeaderboardCollectionStruct l_Leaderboard = default(ApiMapLeaderboardCollectionStruct);
        using HttpClient l_Client = new HttpClient();
        try {
            Task<string> l_Result = null;
            string l_ScoreSaberID = (p_ScoreSaberId != 0) ? $"&player-ssid={p_ScoreSaberId}" : string.Empty;
            string l_Country = (p_Country != string.Empty) ? $"&country={p_Country}" : string.Empty;
            //GSLogger.Instance.Log($"https://api.guildsaber.com/maps/leaderboard/by-hash/{p_Hash}/{GSBeatmapUtils.DifficultyToNumber(p_Beatmap.difficulty)}?guild-id={p_Guild}&page={p_Page}&countperpage={p_CountPerPage}{l_ScoreSaberID}{l_Country}", IPA.Logging.Logger.LogLevel.InfoUp);
            l_Result = l_Client.GetStringAsync(
                $"https://api.guildsaber.com/maps/leaderboard/by-hash/{p_Hash}/{GSBeatmapUtils.DifficultyToNumber(p_Beatmap.difficulty)}?guild-id={p_Guild}{((p_Page > 0) ? "&page="+p_Page : string.Empty)}&countperpage={p_CountPerPage}{l_ScoreSaberID}{l_Country}");
            l_Result.Wait();
            l_Leaderboard = JsonConvert.DeserializeObject<ApiMapLeaderboardCollectionStruct>(l_Result.Result);
        } catch(AggregateException l_E) {
            //GSLogger.Instance.Error(l_E, nameof(GuildApi), nameof(GetLeaderboard));
            return default(ApiMapLeaderboardCollectionStruct);
        }
        return l_Leaderboard;
    }

    /// <summary>
    /// Get Player Info from API
    /// </summary>
    /// <param name="p_GuildFromConfig">Guild From config</param>
    /// <param name="p_GuildId">Guild ID</param>
    /// <param name="p_UseGuild">use a guild</param>
    /// <returns></returns>
    public static ApiPlayerData GetPlayerInfoFromAPI(bool p_GuildFromConfig = true, int p_GuildId = 0, bool p_UseGuild = true)
    {
        /// We don't care if it return null because this function is loaded on the MenuSceneLoadedFresh, and the UserID will most likely be fetched way before that happen.
        #pragma warning disable CS0618
        ulong l_PlayerId = (ulong)long.Parse(GetUserInfo.GetUserID());
        #pragma warning restore CS0618

        if (l_PlayerId == 0)
        { GSLogger.Instance.Error(new Exception("Cannot get player Id"), nameof(GuildApi), nameof(GetPlayerInfoFromAPI)); return default(ApiPlayerData); }

        BSPModule.GuildSaberModule.m_SSPlayerId = l_PlayerId;
        int l_SelectedGuildId = 0;
        if (p_UseGuild == true)
        {
            l_SelectedGuildId = (p_GuildFromConfig == true) ? GSConfig.Instance.SelectedGuild : p_GuildId;

            if (!GuildSaberUtils.GuildsListContainsId(GuildSaberModule.AvailableGuilds, l_SelectedGuildId))
                l_SelectedGuildId = GuildSaberModule.AvailableGuilds[0].ID;
        }

        ApiPlayerData l_DefinedPlayer = default(ApiPlayerData);

        try
        {
            using (HttpClient l_Client = new HttpClient())
            {
                if (BSPModule.GuildSaberModule.m_GSPlayerId == null)
                {
                    Task<string> l_SerializedPlayer =
                        l_Client.GetStringAsync($"https://api.guildsaber.com/player/data/by-ssid/{BSPModule.GuildSaberModule.m_SSPlayerId}/1");
                    l_SerializedPlayer.Wait();
                    ApiPlayerData l_Player = JsonConvert.DeserializeObject<ApiPlayerData>(l_SerializedPlayer.Result);
                    l_DefinedPlayer = l_Player;
                    BSPModule.GuildSaberModule.m_GSPlayerId = (int)l_Player.ID;
                }
                else {
                    //Plugin.Log.Info($"https://api.guildsaber.com/player/data/by-ssid/{GuildSaber.m_SSPlayerId}/1{(p_UseGuild == true ? "?guild-id=" + l_SelectedGuildId : string.Empty)}");
                    Task<string> l_SerializedPlayer
                        = l_Client.GetStringAsync($"https://api.guildsaber.com/player/data/by-ssid/{BSPModule.GuildSaberModule.m_SSPlayerId}/1{(p_UseGuild == true ? "?guild-id=" + l_SelectedGuildId : string.Empty)}");
                    l_SerializedPlayer.Wait();
                    ApiPlayerData l_Player = JsonConvert.DeserializeObject<ApiPlayerData>(l_SerializedPlayer.Result);
                    l_DefinedPlayer = l_Player;
                }
                GuildSaberModule.ModState = GuildSaberModule.EModState.Fonctionnal;
            }
        } catch (Exception l_E)
        {
            GuildSaberModule.ModState = GuildSaberModule.EModState.APIError;
            GSLogger.Instance.Error(l_E, nameof(GuildApi), nameof(GetPlayerInfoFromAPI));
            return default(ApiPlayerData);
        }

        return l_DefinedPlayer;
    }
    /// <summary>
    /// Get player data from card
    /// </summary>
    /// <returns></returns>
    public static ApiPlayerData GetPlayerDataFromCurrent()
    {
        return PlayerCardUI.m_Player;
    }

    /// <summary>
    /// Get guild categories
    /// </summary>
    /// <param name="p_GuildID"></param>
    /// <returns></returns>
    public static async Task<List<ApiCategory>> GetCategoriesForGuild(int p_GuildID)
    {
        try
        {
            using (HttpClient l_Client = new()) {
                string l_SerializedCat = await l_Client.GetStringAsync(new System.Uri($"https://api.guildsaber.com/categories/data/all?guild-id={p_GuildID}"));
                List<ApiCategory> l_Cats = JsonConvert.DeserializeObject<List<ApiCategory>>(l_SerializedCat);
                return l_Cats;
                ///Fuck opti
            }
        } catch(Exception l_E)
        {
            GSLogger.Instance.Error(l_E, nameof(GuildApi), nameof(GetCategoriesForGuild));
        }
        return default;
    }

    /// <summary>
    /// Get Guild Data
    /// </summary>
    /// <param name="p_GuildId"></param>
    /// <param name="p_PlayerId"></param>
    /// <returns></returns>
    public static GuildData GetGuildData(int p_GuildId, int p_PlayerId)
    {
        try
        {
            using (HttpClient l_Client = new())
            {
                Task<string> l_Str = l_Client.GetStringAsync($"https://api.guildsaber.com/guilds/data/all?player-id={p_PlayerId}");
                l_Str.Wait();
                ApiGuildCollection l_GuildsCollection = JsonConvert.DeserializeObject<ApiGuildCollection>(l_Str.Result);
                foreach (var l_Current in l_GuildsCollection.Guilds)
                {
                    if (l_Current.ID != p_GuildId)
                        continue;

                    return l_Current;
                }
                return default;
            }
        }catch (Exception l_E)
        {
            GSLogger.Instance.Error(l_E, nameof(GuildApi), nameof(GetGuildData));
        }
        return default;
    }

    /// <summary>
    /// Get Guild From ID
    /// </summary>
    /// <param name="p_Id"></param>
    /// <returns></returns>
    public static GuildData GetGuildFromId(int p_Id)
    {

        foreach (GuildData l_Guild in GuildSaberModule.AvailableGuilds)
        {
            if (l_Guild.ID != p_Id) continue;

            return l_Guild;
        }
        return default(GuildData);
    }

    /// <summary>
    /// Get Guild from name
    /// </summary>
    /// <param name="p_Name">Exact Guild Name</param>
    /// <returns></returns>
    public static GuildData GetGuildFromName(string p_Name)
    {
        foreach (GuildData l_Current in GuildSaberModule.AvailableGuilds)
        {
            if (l_Current.Name != p_Name)
                continue;

            return l_Current;
        }
        return default(GuildData);
    }
}

public struct GuildCategories
{
    public int GuildId { get; set; } = 0;
    public List<ApiPlayerCategory> Categories { get; set; } = default(List<ApiPlayerCategory>);

    public GuildCategories(int p_GuildId, List<ApiPlayerCategory> p_Categories)
    {
        GuildId = p_GuildId;
        Categories = p_Categories;
    }
}

public struct PlayerGuildsInfo
{
    public PlayerGuildsInfo(ApiPlayerData p_Player = default(ApiPlayerData), List<GuildData> p_AvailableGuilds = null)
    {
        m_ReturnPlayer = p_Player;
        m_AvailableGuilds = p_AvailableGuilds;
    }

    public ApiPlayerData m_ReturnPlayer { get; set; } = default(ApiPlayerData);
    public List<GuildData> m_AvailableGuilds { get; set; } = default(List<GuildData>);
}

class PassState
{
    [Flags]
    public enum EState
    {
        UnVerified = 0,
        Allowed = 1 << 0,
        Denied = 1 << 1,
        NeedConfirmation = 1 << 2,
        MinScoreRequirement = 1 << 3,
        ProhibitedModifiers = 1 << 4,
        MissingModifiers = 1 << 5,
        NewScore = 1 << 6,
        UpdatedScore = 1 << 7,
    }

    public static string GetColorFromPassState(API.PassState.EState p_State)
    {
        switch (p_State)
        {
            case API.PassState.EState.Allowed: return ColorUtility.ToHtmlStringRGBA(Color.green);
            case API.PassState.EState.NeedConfirmation: return ColorUtility.ToHtmlStringRGBA(Color.yellow);
            case API.PassState.EState.Denied: return ColorUtility.ToHtmlStringRGBA(Color.red);
            ///Others with same color
            case API.PassState.EState.NewScore: return GetColorFromPassState(API.PassState.EState.NeedConfirmation);
            case API.PassState.EState.UpdatedScore: return GetColorFromPassState(API.PassState.EState.Allowed);
            case API.PassState.EState.UnVerified: return GetColorFromPassState(API.PassState.EState.NeedConfirmation);
            case API.PassState.EState.MinScoreRequirement: return GetColorFromPassState(API.PassState.EState.Denied);
            case API.PassState.EState.MissingModifiers: return GetColorFromPassState(API.PassState.EState.Denied);
            case API.PassState.EState.ProhibitedModifiers: return GetColorFromPassState(API.PassState.EState.Denied);
            default: return ColorUtility.ToHtmlStringRGBA(Color.white);
        }
    }
}

