using GuildSaberProfile.API;
using GuildSaberProfile.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zenject;
using System.Reflection;

namespace GuildSaberProfile.Utils
{
    public static class GuildSaberUtils
    {
        public static int GetStaticPlayerLevel(string p_Category = null)
        {
            //Gettings all levels
            List<int> l_Levels = new List<int>() { 0 };
            l_Levels = GetLevelCache(PluginConfig.Instance.SelectedGuild, p_Category);
            int l_LastLevel = l_Levels[0];
            if (l_Levels.Count == 0)
                return -1;

            //Getting Max Level
            for (int l_i = 0; l_i < l_Levels.Count - 1;l_i++)
            {
                if (l_Levels.Count - 1 != l_i) {
                    if (l_LastLevel + 1 == l_Levels[l_i + 1])
                    {
                        l_LastLevel = l_Levels[l_i + 1];
                    }
                }
            }

            return l_LastLevel;
        }

        public static List<int> GetLevelCache(string p_Guild, string p_CategoryName = null)
        {
            List<int> l_Levels = new List<int>() { -1, -2 };
            try
            {
                using (HttpClient l_Client = new HttpClient())
                {
                    string l_Link = string.Empty;
                    switch (p_Guild)
                    {
                        case "CS":
                            l_Link = $"{ReturnLinkFromGuild("CS")}/levelcache/{p_CategoryName}";
                            break;
                        case "BSCC":
                            l_Link = $"{ReturnLinkFromGuild("BSCC")}/levelcache/";
                            break;
                        default: return null;
                    }

                    //Wait for result
                    Task<string> l_Result = l_Client.GetStringAsync(l_Link);
                    l_Result.Wait();
                    if (l_Result.Result == string.Empty) {
                        Plugin.Log.Info("Result is null");
                        return new List<int> { 0 };
                    }
                    //Function explicit
                    LevelIDs l_LevelIDs = JsonConvert.DeserializeObject<LevelIDs>(l_Result.Result);
                    l_Levels = l_LevelIDs.LevelID;
                }
            } catch (HttpRequestException p_E)
            {
                Plugin.Log.Error(p_E);
            }
            return l_Levels;
        }

        public static string ReturnLinkFromGuild(string p_Guild)
        {
            switch (p_Guild)
            {
                case "CS":
                    return "http://api.bsdr.fdom.eu";
                case "BSCC":
                    return "https://api.jupilian.me";
                default: return string.Empty;
            }
        }

        public static readonly PropertyInfo m_ContainerPropertyInfo = typeof(MonoInstallerBase).GetProperty("Container", BindingFlags.Instance | BindingFlags.NonPublic);
        public static DiContainer GetContainer(this MonoInstallerBase p_MonoInstallerBase)
        {
            return (DiContainer)m_ContainerPropertyInfo.GetValue(p_MonoInstallerBase);
        }
    }
}
