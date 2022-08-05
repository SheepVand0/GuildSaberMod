using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GuildSaberProfile.Utils;

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
        int p_Page, string p_ScoreSaberId = null, string p_Country = null,
        int p_CountPerPage = 0)
    {
        ApiMapLeaderboardCollectionStruct l_Leaderboard = new ApiMapLeaderboardCollectionStruct();
        using HttpClient l_Client = new HttpClient();

        try
        {
            Task<string> l_Result = null;
            l_Result = l_Client.GetStringAsync(
                $"{GuildSaberUtils.ReturnLinkFromGuild(p_Guild)}/mapleaderboard/{p_Hash}/{GuildSaberLeaderboardUtils.BeatmapDifficultyToDifficultyInOrder(p_Beatmap.difficulty)}/{p_Beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName}/{p_Page}/{p_ScoreSaberId}/{p_Country}/{p_CountPerPage.ToString()}");
            l_Result.Wait();
            l_Leaderboard = JsonConvert.DeserializeObject<ApiMapLeaderboardCollectionStruct>(l_Result.Result);
        } catch(AggregateException p_E)
        {
            Plugin.Log.Error(p_E.Message);
            return new();
        }
        return l_Leaderboard;
    }
}
