using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.API
{
    internal struct PlaylistModel
    {
        public string playlistTitle { get; set; }
        public string playlistAuthor { get; set; }
        public string playlistDescription { get; set; }
        public PlaylistModelCustomData customData { get; set; }
        public string image { get; set; }
    }

    internal struct PlaylistModelCustomData
    {
        public string syncURL { get; set; }
        public List<PlaylistModelSong> songs { get; set; }
        public float? PlaylistLevel;
    }

    internal struct PlaylistModelSong
    {
        public string hash { get; set; }
        public List<PlaylistModelSongDifficulty> difficulties { get; set; }
    }

    internal struct PlaylistModelSongDifficulty {
        public string characteristic { get; set; }
        public string name { get; set; }
    }

}
