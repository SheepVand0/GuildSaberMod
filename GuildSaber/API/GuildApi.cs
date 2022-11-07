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

namespace GuildSaber.API;

public static class GuildApi
{
    //public static

    public static PlayerGuildsInfo GetPlayerGuildsInfo()
    {
        ApiPlayerData l_Player = default(ApiPlayerData);
        List<GuildData> l_Guilds = new List<GuildData>();
        try
        {
            ApiPlayerData l_NoGuildsPlayer = GuildApi.GetPlayerInfoFromAPI(p_UseGuild: false);

            using (HttpClient l_Client = new HttpClient())
            {
                Task<string> l_SerializedGuilds = l_Client.GetStringAsync($"https://api.guildsaber.com/guilds/data/all?player-id={l_NoGuildsPlayer.ID}");
                l_SerializedGuilds.Wait();
                ApiGuildCollection l_GuildCollection = JsonConvert.DeserializeObject<ApiGuildCollection>(l_SerializedGuilds.Result);
                l_Guilds = l_GuildCollection.Guilds;
                l_Player = l_NoGuildsPlayer;
            }
        } catch (Exception l_E)
        {
            Plugin.Log.Error(l_E);
            return new(default(ApiPlayerData), new List<GuildData>());
        }
        return new(l_Player, l_Guilds);
    }

    public static ApiPlayerData GetPlayerByScoreSaberIdAndGuild(string p_ID, int p_Guild)
    {
        ApiPlayerData l_ResultPlayer = default(ApiPlayerData);
        using HttpClient l_Client = new HttpClient();

        try
        {
            Task<string> l_Result = l_Client.GetStringAsync($"https://api.guildsaber.com/player/data/by-ssid/{p_ID}/full?guild={p_Guild}");
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
            //Plugin.Log.Info($"https://api.guildsaber.com/maps/leaderboard/by-hash/{p_Hash}/{GSBeatmapUtils.DifficultyToNumber(p_Beatmap.difficulty)}?guild-id={p_Guild}&page={p_Page}&countperpage={p_CountPerPage}{l_ScoreSaberID}{l_Country}");
            l_Result = l_Client.GetStringAsync(
                $"https://api.guildsaber.com/maps/leaderboard/by-hash/{p_Hash}/{GSBeatmapUtils.DifficultyToNumber(p_Beatmap.difficulty)}?guild-id={p_Guild}&page={p_Page}&countperpage={p_CountPerPage}{l_ScoreSaberID}{l_Country}");
            l_Result.Wait();
            l_Leaderboard = JsonConvert.DeserializeObject<ApiMapLeaderboardCollectionStruct>(l_Result.Result);
        } catch(AggregateException l_E) {
            //Plugin.Log.Error(p_E.Message);
            return default(ApiMapLeaderboardCollectionStruct);
        }
        return l_Leaderboard;
    }

    public static ApiPlayerData GetPlayerInfoFromAPI(bool p_GuildFromConfig = true, int p_GuildId = 0, bool p_UseGuild = true)
    {
        /// We don't care if it return null because this function is loaded on the MenuSceneLoadedFresh, and the UserID will most likely be fetched way before that happen.
        #pragma warning disable CS0618
        ulong l_PlayerId = ulong.Parse(GetUserInfo.GetUserID());
        #pragma warning restore CS0618

        if (l_PlayerId == 0)
        { Plugin.Log.Error("Cannot get Player ID, not creating card"); return default(ApiPlayerData); }

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
            }
        } catch (Exception l_E)
        {
            Plugin.Log.Error(l_E);
            return default(ApiPlayerData);
        }

        return l_DefinedPlayer;
    }

    public static ApiPlayerData GetPlayerDataFromCurrent()
    {
        return PlayerCardUI.m_Player;
    }

    public static PlayerGuildsInfo GetPlayerGuildsInfoFromCurrent()
    {
        if (PlayerCardUI.m_Instance.CardViewController != null)
            return new PlayerGuildsInfo(PlayerCardUI.m_Player, GuildSaberModule.AvailableGuilds);
        else
            return new PlayerGuildsInfo();
    }

    public static List<ApiAPlayerCategory> GetPlayerCategoriesDataForGuild(ulong p_Player, int p_GuildId)
    {
        try
        {
            using (HttpClient l_Client = new HttpClient()) {
                Task<string> l_Player = l_Client.GetStringAsync($"https://api.guildsaber.com/player/data/by-ssid/{p_Player}/1?guild-id={p_GuildId}");
                l_Player.Wait();
                List<ApiAPlayerCategory> l_Categories = new List<ApiAPlayerCategory>();
                l_Categories = JsonConvert.DeserializeObject<ApiPlayerData>(l_Player.Result).CategoryData;
            }

        } catch( Exception l_E)
        {
            Plugin.Log.Error(l_E);
        }
        return new List<ApiAPlayerCategory>();
    }

    public static GuildData GetGuildData(int p_GuildId, int p_PlayerId)
    {
        try
        {
            using (HttpClient l_Client = new HttpClient())
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
                return default(GuildData);
            }
        }catch (Exception l_E)
        {
            Plugin.Log.Error(l_E);
        }
        return default(GuildData);
    }
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
    public List<ApiAPlayerCategory> Categories { get; set; } = default(List<ApiAPlayerCategory>);

    public GuildCategories(int p_GuildId, List<ApiAPlayerCategory> p_Categories)
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
}

