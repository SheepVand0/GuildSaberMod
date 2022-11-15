using System.Collections.Generic;

namespace GuildSaber.API
{
    #nullable enable

    public struct PlaylistFormatApi
    {
        public string playlistTitle { get; set; }
        public string playlistAuthor { get; set; }
        public string playlistDescription { get; set; }
        public PlaylistCustomData? customData { get; set; }
        public List<PlaylistSong> songs { get; set; }
        public string? image { get; set; }
    }

    public struct PlaylistCustomData
    {
        public string? syncURL { get; set; }
    }

    public struct PlaylistSong
    {
        public string hash { get; set; }
        public List<PlaylistDifficultyData> difficulties { get; set; }
    }

    public struct PlaylistDifficultyData
    {
        public string characteristic { get; set; }
        public string name { get; set; }
    }
}
