using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BS_Utils.Gameplay;
using GuildSaber.BSPModule;
using GuildSaber.Configuration;
using GuildSaber.Logger;
using GuildSaber.UI.Card;
using GuildSaber.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace GuildSaber.API;

public static class GuildApi
{
    ///This class is ugly lol

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    /// Player Info
    /// <summary>
    /// Get Player Info from API
    /// </summary>
    /// <param name="p_GuildFromConfig">Guild From config</param>
    /// <param name="p_GuildId">Guild ID</param>
    /// <param name="p_UseGuild">Info from a guild or defaut ?</param>
    /// <returns></returns>
    public static async Task<ApiPlayerData> GetPlayerInfoFromAPI(bool p_GuildFromConfig = true, int p_GuildId = 0, bool p_UseGuild = true)
    {
        /// We don't care if it return null because this function is loaded on the MenuSceneLoadedFresh, and the UserID will most likely be fetched way before that happen.
#pragma warning disable CS0618

        // ReSharper disable once JoinDeclarationAndInitializer
        ulong l_PlayerId
            /*76561199187029281*/;
        l_PlayerId = (ulong)long.Parse(GetUserInfo.GetUserID());

#pragma warning restore CS0618

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (l_PlayerId == 0)
        {
            GSLogger.Instance.Error(new Exception("Cannot get player Id"), nameof(GuildApi), nameof(GetPlayerInfoFromAPI));
            return default;
        }

        GuildSaberModule.SsPlayerId = l_PlayerId;

        int l_SelectedGuildId = 0;

        if (p_UseGuild == true)
        {
            l_SelectedGuildId = (p_GuildFromConfig == true) ? GSConfig.Instance.SelectedGuild : p_GuildId;

            if (!GuildSaberUtils.GuildsListContainsId(GuildSaberModule.AvailableGuilds, l_SelectedGuildId))
                l_SelectedGuildId = GuildSaberModule.AvailableGuilds[0].ID;
        }

        try
        {
            using HttpClient l_Client = new HttpClient();
            ApiPlayerData l_DefinedPlayer;
            if (GuildSaberModule.GSPlayerId == null)
            {
                string l_SerializedPlayer =
                    await l_Client.GetStringAsync($"https://api.guildsaber.com/players/data/by-ssid/{GuildSaberModule.SsPlayerId}/1");
                ApiPlayerData l_Player = JsonConvert.DeserializeObject<ApiPlayerData>(l_SerializedPlayer);
                l_DefinedPlayer = l_Player;
                GuildSaberModule.GSPlayerId = (int)l_Player.ID;
            }
            else
            {
                string l_SerializedPlayer
                    = await l_Client.GetStringAsync($"https://api.guildsaber.com/players/data/by-ssid/{GuildSaberModule.SsPlayerId}/1{(p_UseGuild == true ? "?guild-id=" + l_SelectedGuildId : string.Empty)}");
                ApiPlayerData l_Player = JsonConvert.DeserializeObject<ApiPlayerData>(l_SerializedPlayer);
                l_DefinedPlayer = l_Player;
            }
            GuildSaberModule.SetState(GuildSaberModule.EModState.Fonctionnal);
            return l_DefinedPlayer;
        }
        catch (Exception l_E)
        {
            GuildSaberModule.SetState(GuildSaberModule.EModState.APIError);
            GuildSaberModule.SetErrorState(l_E);
            GSLogger.Instance.Error(l_E, nameof(GuildApi), nameof(GetPlayerInfoFromAPI));
            return default;
        }
    }

    /// <summary>
    /// Get player and all guilds
    /// </summary>
    /// <returns></returns>
    public static async Task<PlayerGuildsInfo> GetPlayerGuildsInfo()
    {
        try
        {
            ApiPlayerData l_Player = default;
            ApiPlayerData l_NoGuildsPlayer = await GetPlayerInfoFromAPI(p_UseGuild: false);

            HttpClient l_Client = new();

            Task<string> l_SerializedGuilds = l_Client.GetStringAsync($"https://api.guildsaber.com/guilds/data/all?player-id={l_NoGuildsPlayer.ID}");
            l_SerializedGuilds.Wait();
            ApiGuildCollection l_GuildCollection = JsonConvert.DeserializeObject<ApiGuildCollection>(l_SerializedGuilds.Result);
            List<GuildData> l_Guilds = l_GuildCollection.Guilds;
            l_Player = l_NoGuildsPlayer;


            GuildSaberModule.SetState(GuildSaberModule.EModState.Fonctionnal);
            return new(l_Player, l_Guilds);
        }
        catch (Exception l_E)
        {
            GSLogger.Instance.Error(l_E, nameof(GuildApi), nameof(GetPlayerGuildsInfo));
            GuildSaberModule.SetState(GuildSaberModule.EModState.APIError);
            GuildSaberModule.SetErrorState(l_E);
            return new(default, default);
        }
    }

    /// <summary>
    /// Get player by score saber id and a guild id
    /// </summary>
    /// <param name="p_ID"></param>
    /// <param name="p_Guild"></param>
    /// <returns></returns>
    public static ApiPlayerData GetPlayerByScoreSaberIdAndGuild(string p_ID, int p_Guild)
    {
        ApiPlayerData l_ResultPlayer = default;
        using HttpClient l_Client = new();

        try
        {
            Task<string> l_Result = l_Client.GetStringAsync($"https://api.guildsaber.com/player/data/by-ssid/{p_ID}/full?guild={p_Guild}");
            l_Result.Wait();
            l_ResultPlayer = JsonConvert.DeserializeObject<ApiPlayerData>(l_Result.Result);
            GuildSaberModule.SetState(GuildSaberModule.EModState.Fonctionnal);
        }
        catch (AggregateException l_AggregateException)
        {
            GuildSaberModule.SetState(GuildSaberModule.EModState.APIError);
            GuildSaberModule.SetErrorState(l_AggregateException);
            if (l_AggregateException.InnerException is HttpRequestException)
            {
                GSLogger.Instance.Error(l_AggregateException, nameof(GuildApi), nameof(GetPlayerByScoreSaberIdAndGuild));
                return new();
            }
        }

        return l_ResultPlayer;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    /// Guilds
    /// <summary>
    /// Get guild categories
    /// </summary>
    /// <param name="p_GuildID"></param>
    /// <returns></returns>
    public static async Task<List<ApiCategory>> GetCategoriesForGuild(int p_GuildID)
    {
        try
        {
            using HttpClient l_Client = new();
            string l_SerializedCat = await l_Client.GetStringAsync(new Uri($"https://api.guildsaber.com/categories/data/all?guild-id={p_GuildID}"));
            List<ApiCategory> l_Cats = JsonConvert.DeserializeObject<List<ApiCategory>>(l_SerializedCat);
            GuildSaberModule.SetState(GuildSaberModule.EModState.Fonctionnal);
            return l_Cats;
        }
        catch (Exception l_E)
        {
            GuildSaberModule.SetState(GuildSaberModule.EModState.APIError);
            GuildSaberModule.SetErrorState(l_E);
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
            using HttpClient l_Client = new();
            Task<string> l_Str = l_Client.GetStringAsync($"https://api.guildsaber.com/guilds/data/all?player-id={p_PlayerId}");
            l_Str.Wait();
            ApiGuildCollection l_GuildsCollection = JsonConvert.DeserializeObject<ApiGuildCollection>(l_Str.Result);
            foreach (var l_Current in l_GuildsCollection.Guilds)
            {
                if (l_Current.ID != p_GuildId)
                    continue;
                GuildSaberModule.SetState(GuildSaberModule.EModState.Fonctionnal);
                return l_Current;
            }

            return default;
        }
        catch (Exception l_E)
        {
            GuildSaberModule.SetState(GuildSaberModule.EModState.APIError);
            GuildSaberModule.SetErrorState(l_E);
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

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    /// Leaderboards
    /// <summary>
    /// Get leaderboard from hash
    /// </summary>
    /// <param name="p_Guild">GuildID</param>
    /// <param name="p_Hash">Hash</param>
    /// <param name="p_BeatMap">Difficulty beatmap</param>
    /// <param name="p_Page">Page</param>
    /// <param name="p_GSId">Player GuildSaber ID</param>
    /// <param name="p_Country">Country</param>
    /// <param name="p_CountPerPage">Score count per page</param>
    /// <returns>The leaderboard</returns>
    public static async Task<ApiMapLeaderboardCollectionStruct> GetLeaderboard(
        int p_Guild, string p_Hash, IDifficultyBeatmap p_BeatMap,
        int p_Page, int p_GSId, string p_Country, string p_Mode,
        int p_CountPerPage = 10, int p_SearchType = 1)
    {
        try
        {
            using HttpClient l_Client = new HttpClient();
            string l_Result = null;
            string l_Country = (p_Country != string.Empty) ? $"&Country={p_Country}" : string.Empty;
            string l_Link = $"https://api.guildsaber.com/leaderboards/map/by-hash/{p_Hash}/{GSBeatmapUtils.DifficultyToNumber(p_BeatMap.difficulty)}/{p_Mode}?guild-id={p_Guild}{((p_Page > 0) ? "&Page=" + p_Page : string.Empty)}&PlayerID={p_GSId}{l_Country}&CountPerPage={p_CountPerPage}&AroundMe=false&SearchType={p_SearchType}";
            GSLogger.Instance.Log(l_Link, IPA.Logging.Logger.LogLevel.DebugUp);
            l_Result = await l_Client.GetStringAsync(l_Link);
            var l_Leaderboard = JsonConvert.DeserializeObject<ApiMapLeaderboardCollectionStruct>(l_Result);
            return l_Leaderboard;
        }
        catch (Exception l_E)
        {
            return default;
        }
    }

    /// <summary>
    /// Get player data from card
    /// </summary>
    /// <returns></returns>
    public static ApiPlayerData GetPlayerDataFromCurrent()
    {
        return PlayerCardUI.m_Player;
    }


}

public struct PlayerGuildsInfo
{
    public PlayerGuildsInfo(ApiPlayerData p_Player = default(ApiPlayerData), List<GuildData> p_AvailableGuilds = null)
    {
        ReturnPlayer = p_Player;
        AvailableGuilds = p_AvailableGuilds;
    }

    public ApiPlayerData ReturnPlayer { get; } = default(ApiPlayerData);
    public List<GuildData> AvailableGuilds { get; } = default(List<GuildData>);
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

    public static string GetColorFromPassState(EState p_State)
    {
        return p_State switch
        {
            EState.Allowed => ColorUtility.ToHtmlStringRGBA(Color.green),
            EState.NeedConfirmation => ColorUtility.ToHtmlStringRGBA(Color.yellow),
            EState.Denied => ColorUtility.ToHtmlStringRGBA(Color.red),
            EState.UpdatedScore => GetColorFromPassState(EState.Allowed),
            EState.NewScore => GetColorFromPassState(EState.NeedConfirmation),
            EState.UnVerified => GetColorFromPassState(EState.NeedConfirmation),
            EState.MinScoreRequirement => GetColorFromPassState(EState.Denied),
            EState.MissingModifiers => GetColorFromPassState(EState.Denied),
            EState.ProhibitedModifiers => GetColorFromPassState(EState.Denied),
            _ => ColorUtility.ToHtmlStringRGBA(Color.white),
        };
    }
}
