using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CSProfile.API;

public static class CSApi
{
    public static PlayerApiReworkOutput GetPlayerByScoreSaberId(string p_ID)
    {
        PlayerApiReworkOutput l_ResultPlayer = new PlayerApiReworkOutput();
        using HttpClient l_Client = new HttpClient();

        try
        {
            Task<string> l_Result = l_Client.GetStringAsync($"http://api.bsdr.fdom.eu/player/data/{p_ID}");
            l_Result.Wait();
            l_ResultPlayer = JsonConvert.DeserializeObject<PlayerApiReworkOutput>(l_Result.Result);

        }
        catch (AggregateException l_AggregateException)
        {
            if (l_AggregateException.InnerException is HttpRequestException l_HttpRequestException)
            {
                Plugin.Log.Info("Error during getting Player data");
            }
        }

        return l_ResultPlayer;
    }
}