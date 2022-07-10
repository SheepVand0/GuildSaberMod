using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            switch (p_Guild)
            {
                case "CS":
                    l_Result = l_Client.GetStringAsync($"http://api.bsdr.fdom.eu/player/data/{p_ID}");
                    break;
                case "BSCC":
                    l_Result = l_Client.GetStringAsync($"Insérer un lien");
                    break;
            }
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
