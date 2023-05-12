using BeatSaberPlaylistsLib.Types;
using CP_SDK.UI;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.Logger;
using GuildSaber.UI.CustomLevelSelectionMenu.Components;
using IPA.Config.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers
{
    internal class LevelSelectionViewController : ViewController<LevelSelectionViewController>
    {
        internal static LevelSelectionViewController Instance;

        private XUIHLayout m_LevelsLayout;
        private XUIVScrollView m_MapList;
        private XUIVScrollView m_PlaylistContainer;
        private MapDetails m_MapDetails;

        string CategoryName;
        string GuildName;

        List<IDifficultyBeatmap> Beatmaps { get; set; }
        List<PlaylistModel> Playlists;
        List<PlaylistButton> UIPlaylists = new List<PlaylistButton>();
        List<MapButton> UIMaps = new List<MapButton>();

        protected override void OnViewCreation()
        {
            Templates.FullRectLayout(
                XUIHLayout.Make()
                    .SetHeight(10)
                    .SetWidth(5)
                    .Bind(ref m_LevelsLayout),
                XUIVLayout.Make(
                    XUIHLayout.Make(
                        XUIVScrollView.Make()
                            .Bind(ref m_MapList),
                        XUIVScrollView.Make()
                            .Bind(ref m_PlaylistContainer),
                        MapDetails.Make().Bind(ref m_MapDetails)
                    )
                )
                .SetWidth(50)
                .SetHeight(55)
                .OnReady(x => x.CSizeFitter.horizontalFit = x.CSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained)
                .OnReady(x => x.HOrVLayoutGroup.childForceExpandHeight = true)
                .OnReady(x => x.HOrVLayoutGroup.childForceExpandWidth = true)
            ).BuildUI(transform);
            Instance = this;
        }

        public void SetLevels(string p_GuildName, string p_CategoryName)
        {
            GuildName = p_GuildName;
            CategoryName = p_CategoryName;

            GSLogger.Instance.Log(GuildName, IPA.Logging.Logger.LogLevel.InfoUp);
            GSLogger.Instance.Log(CategoryName, IPA.Logging.Logger.LogLevel.InfoUp);

            List<PlaylistModel> l_Playlists = InternalPlaylistsManager.LoadPlaylists(GuildName, CategoryName);
            Playlists = l_Playlists;

            UpdateUILevels();

            SetLevel(1);
        }

        public void UpdateUILevels()
        {
            foreach (var l_Index in UIPlaylists)
            {
                l_Index.SetActive(false);
            }

            for (int l_i = 0; l_i < Playlists.Count();l_i++)
            {
                PlaylistModel l_Model = Playlists[l_i];

                List<PlaylistModelSong> l_Songs = l_Model.customData.songs;
                byte[] l_ImageBytes = new byte[l_Model.image.Length];
                for (int l_Char = 0; l_Char < l_Model.image.Length; l_Char++)
                {
                    l_ImageBytes[l_Char] = Convert.ToByte(l_Model.image[l_Char]);
                }
                Texture2D l_Text = new Texture2D(1, 1);
                l_Text.LoadImage(l_ImageBytes);

                if (UIPlaylists.Count - 1 <= l_i)
                {
                    UIPlaylists[l_i].SetActive(true);
                    UIPlaylists[l_i].SetLevel(l_Text, l_Songs);
                } else
                {
                    UIPlaylists.Add(PlaylistButton.Make().SetLevel(l_Text, l_Songs));
                    UIPlaylists[l_i].BuildUI(m_PlaylistContainer.Element.transform);
                }
            }
        }

        public void UpdateMapList()
        {
            foreach (var l_Index in UIMaps)
                l_Index.SetActive(false);

            for (int l_i = 0; l_i < Beatmaps.Count;l_i++)
            {
                if (UIMaps.Count - 1 < l_i)
                {
                    UIMaps[l_i].SetActive(true);
                    UIMaps[l_i].SetBeatmap(Beatmaps[l_i]);
                } else
                {
                    UIMaps.Add(MapButton.Make(Beatmaps[l_i]));
                    UIMaps[l_i].BuildUI(m_MapList.Element.transform);
                }
            }
        }

        public void DownloadLevels()
        {

        }

        public async Task<List<ApiRankingLevel>> CurrentCategoryLevels()
        {
            ApiPlayerData l_Player = await GuildApi.GetPlayerByScoreSaberIdAndGuild(GuildSaberModule.SsPlayerId.ToString(), GuildApi.GetGuildFromName(GuildName).ID);
            ApiPlayerCategory l_Category = default;
            foreach (var l_Index in l_Player.CategoryData)
            {
                if (l_Index.CategoryName != CategoryName) continue;

                l_Category = l_Index;
                break;
            }

            GuildData l_Guild = GuildApi.GetGuildFromName(GuildName);

            if (l_Category.Equals(default(ApiPlayerCategory)))
                l_Category.CategoryID = -1;

            List<ApiRankingLevel> l_Levels = await GuildApi.GetLevels(l_Guild.ID, l_Category.CategoryID);
            return l_Levels;
        }

        public async void SetLevel(int p_Level)
        {
            Beatmaps.Clear();

            PlaylistModel l_Playlist = Playlists.Where(x => x.customData.PlaylistLevel == p_Level).ElementAt(0);
            foreach (var l_Index in l_Playlist.customData.songs)
            {
                await BeatSaberPlus.SDK.Game.Levels.LoadSong($"custom_level_", (p_Level) =>
                {
                    List<IDifficultyBeatmap> l_Beatmaps = new List<IDifficultyBeatmap>();
                    foreach (var l_Beatmapset in p_Level.beatmapLevelData.difficultyBeatmapSets)
                    {
                        foreach (var l_Difficulty in l_Beatmapset.difficultyBeatmaps)
                        {
                            foreach (var l_PlaylistDiff in l_Index.difficulties)
                            {
                                if (l_PlaylistDiff.name == l_Difficulty.difficulty.ToString() && l_PlaylistDiff.characteristic == l_Beatmapset.beatmapCharacteristic.name)
                                    Beatmaps.Add(l_Difficulty);
                            }
                        }
                    }
                });
            }
        }

    }
}
