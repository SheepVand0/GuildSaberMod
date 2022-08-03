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
                return new PlayerApiReworkOutput();
            }
        }

        return l_ResultPlayer;
    }
}
