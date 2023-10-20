using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BS_Utils.Gameplay;
using CP_SDK.Network;
using GuildSaber.Configuration;
using GuildSaber.Logger;
using GuildSaber.UI.Card;
using GuildSaber.Utils;
using Newtonsoft.Json;
using UnityEngine;
using Color = UnityEngine.Color;

namespace GuildSaber.API;

public static class GuildApi
{
    public const string PASS_POINTS_TYPE = "pass";
    public const string ACC_POINTS_TYPE = "acc";

    /*///This class is ugly lol

    public static async Task<ApiPlayerData> Init()
    {


    }

    ////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////


    public static async Task<ApiPlayerData> GetBasicPlayerData(int p_GuildSaberID)
    {
        try
        {
            var l_SerializedData = (await GuildSaberUtils.GetStringAsync($"https://api.guildsaber.com/players/data/by-id/{p_GuildSaberID}/0")).BodyString;
            var l_FinalData = JsonConvert.DeserializeObject<ApiPlayerData>(l_SerializedData);
            return l_FinalData;
        } catch (Exception ex)
        {
            GSLogger.Instance.Error(ex, nameof(GuildApi), nameof(GetBasicPlayerData));
            return default;
        }

    }*/

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    /// Player Info
    /// <summary>
    ///     Get Player Info from API
    /// </summary>
    /// <param name="p_GuildFromConfig">Guild From config</param>
    /// <param name="p_GuildId">Guild ID</param>
    /// <param name="p_UseGuild">Info from a guild or default ?</param>
    /// <returns></returns>
    public static async Task<ApiPlayerData> GetPlayerData(bool p_GuildFromConfig = true, int p_GuildId = 0, bool p_UseGuild = true)
    {
        // ReSharper disable once JoinDeclarationAndInitializer
        ulong l_PlayerId;
        l_PlayerId = (ulong)long.Parse((await GetUserInfo.GetUserAsync()).platformUserId);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (l_PlayerId == 0)
        {
            GSLogger.Instance.Error(new Exception("Cannot get player Id"), nameof(GuildApi), nameof(GetPlayerData));
            return default(ApiPlayerData);
        }

        GuildSaberModule.SsPlayerId = l_PlayerId;

        int l_SelectedGuildId = 0;

        if (p_UseGuild)
        {
            l_SelectedGuildId = p_GuildFromConfig ? GSConfig.Instance.SelectedGuild : p_GuildId;

            if (!GuildSaberUtils.GuildsListContainsId(GuildSaberModule.AvailableGuilds, l_SelectedGuildId)) l_SelectedGuildId = GuildSaberModule.AvailableGuilds[0].ID;
        }

        try
        {
            CP_SDK.Network.WebClientCore l_Client = CP_SDK.Network.WebClientCore.GlobalClient;

            ApiPlayerData l_DefinedPlayer = default;
            if (GuildSaberModule.GSPlayerId == null)
            {
                var l_Player = JsonConvert.DeserializeObject<ApiPlayerData>((await GuildSaberUtils.GetStringAsync($"https://api.guildsaber.com/players/data/by-blid/{GuildSaberModule.SsPlayerId}/1")).BodyString);
                l_DefinedPlayer = l_Player;
                GuildSaberModule.GSPlayerId = (int)l_Player.ID;
            }
            else
            {
                var l_Player = JsonConvert.DeserializeObject<ApiPlayerData>((await GuildSaberUtils.GetStringAsync($"https://api.guildsaber.com/players/data/by-blid/{GuildSaberModule.SsPlayerId}/1{(p_UseGuild ? "?guild-id=" + l_SelectedGuildId : string.Empty)}")).BodyString);
                l_DefinedPlayer = l_Player;
                
            }
            GuildSaberModule.SetState(GuildSaberModule.EModState.Functional);
            return l_DefinedPlayer;
        }
        catch (Exception l_E)
        {
            GuildSaberModule.SetState(GuildSaberModule.EModState.APIError);
            GuildSaberModule.SetErrorState(l_E);
            GSLogger.Instance.Error(l_E, nameof(GuildApi), nameof(GetPlayerData));
            throw;
        }
    }

    /// <summary>
    ///     Get player and all guilds
    /// </summary>
    /// <returns></returns>
    public static async Task<PlayerGuildsInfo> GetPlayerGuildsInfo()
    {
        try
        {
            ApiPlayerData l_Player = await GetPlayerData(p_UseGuild: false);

            var l_Client = CP_SDK.Network.WebClientCore.GlobalClient;

            var l_GuildCollection = JsonConvert.DeserializeObject<ApiGuildCollection>((await GuildSaberUtils.GetStringAsync($"https://api.guildsaber.com/guilds/data/all?player-id={l_Player.ID}")).BodyString);
            List<GuildData> l_Guilds = l_GuildCollection.Guilds;


            GuildSaberModule.SetState(GuildSaberModule.EModState.Functional);
            return new PlayerGuildsInfo(l_Player, l_Guilds);
        }
        catch (Exception l_E)
        {
            GSLogger.Instance.Error(l_E, nameof(GuildApi), nameof(GetPlayerGuildsInfo));
            GuildSaberModule.SetState(GuildSaberModule.EModState.APIError);
            GuildSaberModule.SetErrorState(l_E);
            return new PlayerGuildsInfo();
        }
    }

    /// <summary>
    ///     Get player by score saber id and a guild id
    /// </summary>
    /// <param name="p_ID"></param>
    /// <param name="p_Guild"></param>
    /// <returns></returns>
    public static async Task<ApiPlayerData> GetPlayerByScoreSaberIdAndGuild(string p_ID, int p_Guild)
    {
        var l_ResultPlayer = default(ApiPlayerData);
        using var l_Client = new HttpClient();

        try
        {
            string l_Result = await l_Client.GetStringAsync($"https://api.guildsaber.com/player/data/by-blid/{p_ID}/full?guild={p_Guild}");
            l_ResultPlayer = JsonConvert.DeserializeObject<ApiPlayerData>(l_Result);
            GuildSaberModule.SetState(GuildSaberModule.EModState.Functional);
        }
        catch (AggregateException l_AggregateException)
        {
            GuildSaberModule.SetState(GuildSaberModule.EModState.APIError);
            GuildSaberModule.SetErrorState(l_AggregateException);
            if (l_AggregateException.InnerException is HttpRequestException)
            {
                GSLogger.Instance.Error(l_AggregateException, nameof(GuildApi), nameof(GetPlayerByScoreSaberIdAndGuild));
                return new ApiPlayerData();
            }
        }

        return l_ResultPlayer;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    /// Guilds
    /// <summary>
    ///     Get guild categories
    /// </summary>
    /// <param name="p_GuildID"></param>
    /// <returns></returns>
    public static async Task<List<ApiCategory>> GetCategoriesForGuild(int p_GuildID)
    {
        try
        {
            using var l_Client = new HttpClient();
            string l_SerializedCat = await l_Client.GetStringAsync(new Uri($"https://api.guildsaber.com/categories/data/all?guild-id={p_GuildID}"));
            List<ApiCategory> l_Cats = JsonConvert.DeserializeObject<List<ApiCategory>>(l_SerializedCat);
            GuildSaberModule.SetState(GuildSaberModule.EModState.Functional);
            return l_Cats;
        }
        catch (Exception l_E)
        {
            GuildSaberModule.SetState(GuildSaberModule.EModState.APIError);
            GuildSaberModule.SetErrorState(l_E);
            GSLogger.Instance.Error(l_E, nameof(GuildApi), nameof(GetCategoriesForGuild));
        }
        return default(List<ApiCategory>);
    }


    /// <summary>
    ///     Get Guild From ID
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
    ///     Get Guild from name
    /// </summary>
    /// <param name="p_Name">Exact Guild Name</param>
    /// <returns></returns>
    public static GuildData GetGuildFromName(string p_Name)
    {
        foreach (GuildData l_Current in GuildSaberModule.AvailableGuilds)
        {
            if (l_Current.Name != p_Name && l_Current.SmallName != p_Name) continue;

            return l_Current;
        }
        return default(GuildData);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    /// Leaderboards
    /// <summary>
    ///     Get leaderboard from hash
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
        int p_CountPerPage = GuildSaberModule.SCORES_BY_PAGE, int p_SearchType = 1)
    {
        try
        {
            using var l_Client = new HttpClient();
            string l_Country = p_Country != string.Empty ? $"&Country={p_Country}" : string.Empty;
            string l_Link = $"https://api.guildsaber.com/leaderboards/map/by-hash/{p_Hash}/{GSBeatmapUtils.DifficultyToNumber(p_BeatMap.difficulty)}/{p_Mode}?guild-id={p_Guild}{(p_Page > 0 ? "&Page=" + p_Page : string.Empty)}&PlayerID={p_GSId}{l_Country}&CountPerPage={p_CountPerPage}&AroundMe=false&SearchType={p_SearchType}";
            GSLogger.Instance.Log(l_Link, IPA.Logging.Logger.LogLevel.DebugUp);
            string l_Result = await l_Client.GetStringAsync(l_Link);
            var l_Leaderboard = JsonConvert.DeserializeObject<ApiMapLeaderboardCollectionStruct>(l_Result);
            return l_Leaderboard;
        }
        catch (Exception l_E)
        {
            return default(ApiMapLeaderboardCollectionStruct);
        }
    }

    /// <summary>
    ///     Get player data from card
    /// </summary>
    /// <returns></returns>
    public static ApiPlayerData GetPlayerDataFromCurrent()
    {
        return PlayerCardUI.m_Player;
    }

    internal static async Task<List<ApiRankingLevel>> GetLevels(int p_GuildId, int p_CategoryId)
    {
        try
        {
            HttpClient l_Client = new HttpClient();
            string l_CategoryString = (p_CategoryId == -1) ? string.Empty : $"&category-id={p_CategoryId}";
            string l_Serialized = await l_Client.GetStringAsync($"https://api.guildsaber.com/levels/data/all?guild-id={p_GuildId}{l_CategoryString}");
            List<ApiRankingLevel> l_Levels = JsonConvert.DeserializeObject<List<ApiRankingLevel>>(l_Serialized);
            return l_Levels;
        }
        catch (Exception l_E)
        {
            return default(List<ApiRankingLevel>);
        }
    }

    internal static async Task<List<PlaylistModel>> GetCategoryPlaylists(int p_GuildId, ApiCategory p_Category)
    {
        try
        {
            string l_CategoryQueryString = p_Category.ID != 0 ? $"category-id={p_Category.ID}" : string.Empty;
            List<ApiRankingLevel> l_Levels = await GuildApi.GetLevels(p_GuildId, p_Category.ID);
            HttpClient l_Client = new HttpClient();
            List<PlaylistModel> l_Playlists = new List<PlaylistModel>();
            for (int l_i = 0; l_i < l_Levels.Count; l_i++)
            {
                //player-id={GuildSaberModule.GSPlayerId}
                string l_QueryString = $"https://api.guildsaber.com/playlists/data/by-id/{l_Levels[l_i].ID}?{l_CategoryQueryString}";
                PlaylistModel l_Playlist = JsonConvert.DeserializeObject<PlaylistModel>(await l_Client.GetStringAsync(l_QueryString));
                l_Playlist.customData.PlaylistLevel = l_Levels[l_i].LevelNumber;
                l_Playlists.Add(l_Playlist);
            }

            return l_Playlists;
        }
        catch (Exception l_E)
        {
            GSLogger.Instance.Error(l_E, nameof(GuildApi), nameof(GetCategoryPlaylists));
            return new List<PlaylistModel>();
        }
    }
}

public struct PlayerGuildsInfo
{
    public PlayerGuildsInfo(ApiPlayerData p_Player, List<GuildData>? p_AvailableGuilds)
    {
        ReturnPlayer = p_Player;
        AvailableGuilds = p_AvailableGuilds ?? new List<GuildData>();
    }

    public ApiPlayerData ReturnPlayer { get; } = default(ApiPlayerData);
    public List<GuildData> AvailableGuilds { get; } = new List<GuildData>();
}

internal class PassState
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
        UpdatedScore = 1 << 7
    }

    public static Color GetColorFromPassState(EState p_State)
    {
        if (p_State.HasFlag(EState.NeedConfirmation))
            return Color.yellow;
        else if (!p_State.HasFlag(EState.Allowed))
            return Color.red;
        return Color.green;
    }
}
