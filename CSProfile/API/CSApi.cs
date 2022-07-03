using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using BSDiscordRanking.Formats.API;
using Newtonsoft.Json;

namespace CSProfile.API
{
    public static class CSApi
    {

        public static PlayerApiReworkOutput GetPlayerByScoreSaberId(string p_id)
        {
            PlayerApiReworkOutput l_ResultPlayer = new PlayerApiReworkOutput();
            using HttpClient l_Client = new HttpClient();

            try
            {
                Task<string> l_Result = l_Client.GetStringAsync($"http://api.bsdr.fdom.eu/player/data/{p_id}");
                l_Result.Wait();
                l_ResultPlayer = JsonConvert.DeserializeObject<PlayerApiReworkOutput>(l_Result.Result);

            } catch (System.AggregateException l_AggregateException)
            {
                if (l_AggregateException.InnerException is HttpRequestException l_httpRequestException)
                {
                    Plugin.Log.Info("Error during getting Player data");
                }
            }

            return l_ResultPlayer;
        }

    }
}
