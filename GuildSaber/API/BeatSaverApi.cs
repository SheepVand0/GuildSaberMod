using GuildSaber.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.API
{
    internal class BeatSaverApi
    {
        internal static async Task<bool> DownloadMapByHash(string p_Hash)
        {
            HttpClient l_Client = new HttpClient();
            try
            {
                byte[] l_Data = await l_Client.GetByteArrayAsync($"https://r2cdn.beatsaver.com/{p_Hash}.zip");
                string l_TempDirectory = "./Temp";
                if (!Directory.Exists(l_TempDirectory))
                {
                    Directory.CreateDirectory(l_TempDirectory);
                }

                /// Extract zip file

                string l_FilePath = l_TempDirectory + "/" + p_Hash + ".zip";

                File.WriteAllBytes(l_FilePath, l_Data);

                /// Creating a folder with hash in name and extracting

                string l_NewPath = $"./Beat Saber_Data/CustomLevels/{p_Hash}";
                ZipFile.ExtractToDirectory(l_FilePath, l_NewPath, Encoding.UTF8);

                /// copying map files to a folder with name and mapper in folder name
                File.Delete($"{l_TempDirectory}/{p_Hash}.zip");
                
                return true;
            } catch (Exception ex)
            {
                GSLogger.Instance.Error(ex, nameof(BeatSaverApi), nameof(DownloadMapByHash));
                return false;
            }
        }

    }
}
