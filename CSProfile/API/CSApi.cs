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
            PlayerApiReworkOutput l_resultPlayer = new PlayerApiReworkOutput();
            using HttpClient l_client = new HttpClient();

            try
            {
                Task<string> l_result = l_client.GetStringAsync($"http://api.bsdr.fdom.eu/player/data/{p_id}");
                l_result.Wait();
                l_resultPlayer = JsonConvert.DeserializeObject<PlayerApiReworkOutput>(l_result.Result);

            } catch (System.AggregateException l_AggregateException)
            {
                if (l_AggregateException.InnerException is HttpRequestException l_httpRequestException)
                {
                    Plugin.Log.Info("Error during getting Player data");
                }
            }

            return l_resultPlayer;
        }

    }
}
