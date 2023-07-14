using BeatSaberPlaylistsLib.Types;
using CP_SDK.UI;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.Logger;
using GuildSaber.UI.CustomLevelSelectionMenu.Components;
using GuildSaber.UI.Leaderboard;
using GuildSaber.Utils;
using IPA.Config.Data;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SColor = SixLabors.ImageSharp.Color;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers
{
    internal class LevelSelectionViewController : ViewController<LevelSelectionViewController>
    {
        internal static LevelSelectionViewController Instance;

        private XUIVScrollView m_MapList;
        private XUIVScrollView m_PlaylistsContainer;
        private MapDetails m_MapDetails;

        string CategoryName;
        string GuildName;

        List<IDifficultyBeatmap> Beatmaps = new List<IDifficultyBeatmap>();
        List<PlaylistModel> Playlists = new List<PlaylistModel>();
        List<PlaylistButton> UIPlaylists = new List<PlaylistButton>();
        List<MapButton> UIMaps = new List<MapButton>();

        protected XUIVLayout m_WorkingLayout;
        protected XUIVLayout m_NoLevelsFoundLayout;
        protected XUIVLayout m_LoadingLayout;

        protected override void OnViewCreation()
        {
             (m_WorkingLayout = Templates.FullRectLayout(
                XUIHLayout.Make(
                    XUIHLayout.Make(
                        XUIVScrollView.Make()
                        .Bind(ref m_PlaylistsContainer)
                        ,
                        XUIVScrollView.Make()
                        .Bind(ref m_MapList)
                    ).SetHeight(55)
                     .OnReady(x => x.CSizeFitter.horizontalFit = x.CSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained)
                     .OnReady(x => x.HOrVLayoutGroup.childForceExpandHeight = true)
                     .OnReady(x => x.HOrVLayoutGroup.childForceExpandWidth = true),
                    XUIVLayout.Make(
                        MapDetails.Make()
                            .Bind(ref m_MapDetails)
                            .SetWidth(20)
                    )
                ).SetWidth(100)
            )).BuildUI(transform);

            (m_NoLevelsFoundLayout = Templates.FullRectLayout(
                XUIVLayout.Make(
                    XUIText.Make("No levels found")
                    )
            )).BuildUI(transform);

            (m_LoadingLayout = Templates.FullRectLayout(
                XUIText.Make("Loading category levels...")
             )).BuildUI(transform);

            Instance = this;
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public enum EMode
        {
            Normal,
            NoLevelsFound,
            Loading
        }

        public void SetMode(EMode p_Mode)
        {
            m_WorkingLayout.SetActive(p_Mode == EMode.Normal);
            m_NoLevelsFoundLayout.SetActive(p_Mode == EMode.NoLevelsFound);
            m_LoadingLayout.SetActive(p_Mode == EMode.Loading);
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public async void SetLevels(int p_GuildId, ApiCategory p_Category)
        {
            await WaitUtils.Wait(() => m_WorkingLayout != null, 1);

            SetMode(EMode.Loading);

            GSLogger.Instance.Log(p_GuildId, IPA.Logging.Logger.LogLevel.InfoUp);

            GuildData l_Guild = GuildSaberModule.AvailableGuilds.Where((x) => x.ID == p_GuildId).ElementAt(0);

            m_WorkingLayout.OnReady(async (x) =>
            {
                GuildName = l_Guild.Name;
                CategoryName = p_Category.Name;

                GSLogger.Instance.Log(GuildName, IPA.Logging.Logger.LogLevel.InfoUp);
                GSLogger.Instance.Log(CategoryName, IPA.Logging.Logger.LogLevel.InfoUp);

                List<PlaylistModel> l_Playlists = await InternalPlaylistsManager.LoadPlaylists(GuildName, CategoryName);
                Playlists = l_Playlists;

                if (Playlists.Count == 0)
                {
                    List<PlaylistModel> l_NewPlaylists = await GuildApi.GetCategoryPlaylists(p_GuildId, p_Category);

                    InternalPlaylistsManager.SavePlaylistsToFile(l_NewPlaylists, l_Guild.Name, p_Category.Name);
                    Playlists = l_NewPlaylists;
                }

                if (Playlists.Count == 0)
                {
                    SetMode(EMode.NoLevelsFound);
                    return;
                }

                await UpdateLevelsOnUI();

                SetMode(EMode.Normal);

                PlaylistModel l_FirstLevel = Playlists.ElementAt(0);

                SetSelectedPlaylist(l_FirstLevel.customData.PlaylistLevel);

                GSLogger.Instance.Log("Finished Loading levels", IPA.Logging.Logger.LogLevel.InfoUp);
            });
        }

        public async Task<Task> UpdateLevelsOnUI()
        {
            foreach (var l_Index in UIPlaylists)
            {
                l_Index.SetActive(false);
            }

            for (int l_i = 0; l_i < Playlists.Count();l_i++)
            {
                PlaylistModel l_Model = Playlists[l_i];

                List<PlaylistModelSong> l_Songs = l_Model.songs;
                byte[] l_ImageBytes = new byte[l_Model.image.Length];
                l_ImageBytes = Convert.FromBase64String(l_Model.image);

                Image<Rgba32> l_Image = null;

                await Task.Run(() =>
                {
                    l_Image = Image.Load<Rgba32>(l_ImageBytes);
                });

                Texture2D l_LevelCover = new Texture2D(l_Image.Width, l_Image.Height);

                await Task.Run(() =>
                {
                    for (int l_X = 0; l_X < l_Image.Width; l_X++)
                    {
                        for (int l_Y = 0; l_Y < l_Image.Height; l_Y++)
                        {
                            int l_FixedX = l_Image.Width - 1 - (l_X);
                            int l_FixedY = l_Image.Height - 1 - (l_Y);
                            SColor l_Color = l_Image[l_X, l_FixedY];
                            Rgba32 l_Pixel = l_Color.ToPixel<Rgba32>();
                            l_LevelCover.SetPixel(l_X, l_Y, new UnityEngine.Color((float)l_Pixel.R / 255, (float)l_Pixel.G / 255, (float)l_Pixel.B / 255, (float)l_Pixel.A / 255));
                        }
                    }
                });

                if (l_i > UIPlaylists.Count - 1)
                {
                    UIPlaylists.Add(PlaylistButton.Make().SetLevel(l_LevelCover, l_Songs));
                    UIPlaylists[l_i].BuildUI(m_PlaylistsContainer.Element.Container);
                } else
                {
                    UIPlaylists[l_i].SetActive(true);
                    UIPlaylists[l_i].SetLevel(l_LevelCover, l_Songs);
                }
            }

            return Task.CompletedTask;
        }

        public async void SetSelectedPlaylist(float p_Level)
        {
            Beatmaps.Clear();

            PlaylistModel l_Playlist = Playlists.Where(x => x.customData.PlaylistLevel == p_Level).ElementAt(0);
            if (l_Playlist.Equals(null))
            {
                GSLogger.Instance.Log("Level is null" , IPA.Logging.Logger.LogLevel.InfoUp);
                return;
            }

            foreach (var l_Index in l_Playlist.songs)
            {
                GSLogger.Instance.Log($"custom_level_{l_Index.hash}", IPA.Logging.Logger.LogLevel.InfoUp);
                await BeatSaberPlus.SDK.Game.Levels.LoadSong($"custom_level_{l_Index.hash.ToUpper()}", (p_Level) =>
                {
                    List<IDifficultyBeatmap> l_Beatmaps = new List<IDifficultyBeatmap>();
                    foreach (var l_Beatmapset in p_Level.beatmapLevelData.difficultyBeatmapSets)
                    {
                        foreach (var l_Difficulty in l_Beatmapset.difficultyBeatmaps)
                        {
                            foreach (var l_PlaylistDiff in l_Index.difficulties)
                            {
                                GSLogger.Instance.Log(l_PlaylistDiff.name.ToLower(), IPA.Logging.Logger.LogLevel.InfoUp);
                                GSLogger.Instance.Log(l_Difficulty.difficulty.ToString().ToLower(), IPA.Logging.Logger.LogLevel.InfoUp);
                                GSLogger.Instance.Log(l_PlaylistDiff.characteristic, IPA.Logging.Logger.LogLevel.InfoUp);
                                GSLogger.Instance.Log(l_Beatmapset.beatmapCharacteristic.serializedName, IPA.Logging.Logger.LogLevel.InfoUp);
                                GSLogger.Instance.Log("--------", IPA.Logging.Logger.LogLevel.InfoUp);

                                if (l_PlaylistDiff.name.ToLower() == l_Difficulty.difficulty.ToString().ToLower() && l_PlaylistDiff.characteristic == l_Beatmapset.beatmapCharacteristic.serializedName)
                                    Beatmaps.Add(l_Difficulty);
                            }
                        }
                    }
                });
            }

            GSLogger.Instance.Log(Beatmaps.Count, IPA.Logging.Logger.LogLevel.InfoUp);

            UpdateMapList();
        }

        public void DownloadLevels()
        {

        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void UpdateMapList()
        {
            foreach (var l_Index in UIMaps)
                l_Index.SetActive(false);

            for (int l_i = 0; l_i < Beatmaps.Count;l_i++)
            {
                if (l_i > UIMaps.Count - 1)
                {
                    UIMaps.Add(MapButton.Make(Beatmaps[l_i]));
                    UIMaps[l_i].BuildUI(m_MapList.Element.Container);
                } else
                {
                    UIMaps[l_i].SetActive(true);
                    UIMaps[l_i].SetBeatmap(Beatmaps[l_i]);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

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

    }
}
