using GuildSaber.API;
using GuildSaber.Logger;
using Newtonsoft.Json;
using PlaylistManager.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.UI
{
    internal class InternalPlaylistsManager
    {


        internal static bool SavePlaylistsToFile(string p_SerializedPlaylists, string p_GuildName, string p_CategoryName)
        {
            try
            {
                string l_GuildSaberPlaylistsDirectory = "./GuildSaberPlaylists";
                if (!Directory.Exists(l_GuildSaberPlaylistsDirectory))
                    Directory.CreateDirectory(l_GuildSaberPlaylistsDirectory);

                System.IO.File.WriteAllText($"./{l_GuildSaberPlaylistsDirectory}/{p_GuildName}/{GeneratePlaylistSaveName(p_GuildName, p_CategoryName)}.json", p_SerializedPlaylists);

                return true;
            } catch (Exception ex) {
                GSLogger.Instance.Error(ex, nameof(InternalPlaylistsManager), nameof(SavePlaylistsToFile));
                return false;
            }
        }

        internal static bool SavePlaylistsToFile(List<PlaylistModel> p_Playlists, string p_GuildName, string p_CategoryName)
        {
            string l_Serialized = JsonConvert.SerializeObject(p_Playlists);
            return SavePlaylistsToFile(l_Serialized, p_GuildName, p_CategoryName);
        }

        internal static List<PlaylistModel> LoadPlaylists(string p_GuildName, string p_CategoryName)
        {
            List<PlaylistModel> l_Playlists = new List<PlaylistModel>();
            string l_Link = $"./GuildSaberPlaylists/{p_GuildName}/{GeneratePlaylistSaveName(p_GuildName, p_CategoryName)}.json";
            if (!File.Exists(l_Link)) { return new List<PlaylistModel>(); }
            string l_Serialized = File.ReadAllText(l_Link);
            l_Playlists = JsonConvert.DeserializeObject<List<PlaylistModel>>(l_Serialized);
            return l_Playlists;
        }

        internal static string GeneratePlaylistSaveName(string p_GuildName, string p_CategoryName)
        {
            return $"{p_GuildName.Replace(' ', '_')}_{p_CategoryName.Replace(' ', '_')}";
        }

    }
}
