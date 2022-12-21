using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using BeatLeader.Models;
using GuildSaber.Logger;

namespace GuildSaber.API
{
    public static class BeatLeaderReplayDownloader
    {

        public static async Task<Replay?> GetReplay(string p_Link)
        {
            WebClient l_Client = new WebClient();

            Replay? l_NewData = null;
            try
            {
                byte[] l_Bytes = await l_Client.DownloadDataTaskAsync(new Uri(p_Link));
                GSLogger.Instance.Log(p_Link, IPA.Logging.Logger.LogLevel.InfoUp);

                MethodInfo? l_BeatLeaderMethod = Assembly.Load(AssemblyName.GetAssemblyName("Plugins\\BeatLeader.dll")).GetType("BeatLeader.Models.ReplayDecoder", true).GetMethod("Decode", new[]
                {
                    typeof(byte[])
                });
                GSLogger.Instance.Log(l_BeatLeaderMethod == null, IPA.Logging.Logger.LogLevel.NoticeUp);
                l_NewData = (Replay)l_BeatLeaderMethod.Invoke(null, new object[]
                {
                    l_Bytes
                });

            }
            catch (Exception l_E)
            {
                GSLogger.Instance.Error(l_E,
                    nameof(BeatLeaderReplayDownloader), nameof(GetReplay));
                return null;
            }
            return l_NewData;
        }
    }
}