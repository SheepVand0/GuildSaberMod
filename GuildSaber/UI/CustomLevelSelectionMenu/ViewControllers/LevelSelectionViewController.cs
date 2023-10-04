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
using Color = UnityEngine.Color;
using System.Configuration;
using GuildSaber.UI.Defaults;
using PlaylistManager.HarmonyPatches;
using JetBrains.Annotations;
using GuildSaber.UI.Others;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers
{
    internal class LevelSelectionViewController : BeatSaberPlus.SDK.UI.ViewController<LevelSelectionViewController>
    {
        internal static LevelSelectionViewController Instance;

        private XUIVScrollView m_MapList;
        private XUIVScrollView m_PlaylistsContainer;
        private MapDetails m_MapDetails;

        string CategoryName;
        string GuildName;

        protected float m_SelectedLevel = 0;

        List<IDifficultyBeatmap> Beatmaps = new List<IDifficultyBeatmap>();
        List<string> MapsToDownload = new List<string>();
        List<PlaylistModel> Playlists = new List<PlaylistModel>();
        List<PlaylistButton> UIPlaylists = new List<PlaylistButton>();
        List<MapButton> UIMaps = new List<MapButton>();

        protected XUIVLayout m_WorkingLayout;
        protected XUIVLayout m_NoLevelsFoundLayout;
        protected XUIVLayout m_LoadingLayout;

        protected XUIText m_StatusText;
        protected XUIText m_MissingMapsText;

        protected XUISecondaryButton m_DownloadMapsButton;

        protected XUITextInput m_SearchTextInput;

        protected string m_SearchValue = string.Empty;

        protected override void OnViewCreation()
        {
             (m_WorkingLayout = Templates.FullRectLayout(
                XUIHLayout.Make(
                    XUIText.Make("").Bind(ref m_MissingMapsText),
                    GSSecondaryButton.Make("Download", 20, 5, p_OnClick: DownloadLevels)
                    .SetActive(false)
                    .Bind(ref m_DownloadMapsButton)
                )
                .SetHeight(10),
                XUIHLayout.Make(
                    XUITextInput.Make("Search").OnValueChanged((val) =>
                    {
                        m_SearchValue = val;
                        UpdateMapList(false, true);
                    }).Bind(ref m_SearchTextInput),
                    GSSecondaryButton.Make("x", 5, 5).OnClick(() =>
                    {
                        m_SearchValue = string.Empty;
                        m_SearchTextInput.Element.SetValue(string.Empty);
                        UpdateMapList(false, true);
                    })
                    ),
                XUIHLayout.Make(
                    XUIHLayout.Make(
                        XUIVLayout.Make(
                            XUIVScrollView.Make()
                            .Bind(ref m_PlaylistsContainer)
                        ).SetWidth(20)
                         .OnReady(x => x.CSizeFitter.horizontalFit = x.CSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained)
                         .OnReady(x => x.HOrVLayoutGroup.childForceExpandHeight = true),
                        XUIVLayout.Make(
                            XUIVScrollView.Make()
                            .Bind(ref m_MapList)
                        ).SetWidth(50)
                         .OnReady(x => x.CSizeFitter.horizontalFit = x.CSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained)
                         .OnReady(x => x.HOrVLayoutGroup.childForceExpandHeight = true)
                    )
                    .SetHeight(55),
                    XUIVLayout.Make(
                        MapDetails.Make()
                            .Bind(ref m_MapDetails)
                            .SetWidth(20)
                    )
                ).SetWidth(150)
            )).BuildUI(transform);

            (m_NoLevelsFoundLayout = Templates.FullRectLayout(
                XUIVLayout.Make(
                    XUIText.Make("No levels found")
                    )
            )).BuildUI(transform);

            (m_LoadingLayout = Templates.FullRectLayout(
                XUIText.Make("Loading category levels..."),
                XUIText.Make("").Bind(ref m_StatusText)
             )).BuildUI(transform);

            //m_PlaylistsContainer.Element.SetContentSize(10);
            m_PlaylistsContainer.Element.LElement.minWidth = 2f;
            m_PlaylistsContainer.Element.LElement.preferredWidth = 2f;
            m_PlaylistsContainer.Element.LElement.flexibleWidth = 2f;

            m_MapList.Element.LElement.minWidth = 2f;
            m_MapList.Element.LElement.preferredWidth = 2f;
            m_MapList.Element.LElement.flexibleWidth = 2f;

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

        /*public void ShowStatusText(string p_Text)
        {

        }

        public void HideStatusText()
        {

        }
*/
        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public async void SetLevels(int p_GuildId, ApiCategory p_Category)
        {
            await WaitUtils.Wait(() => m_WorkingLayout != null, 1);

            GuildSaberLeaderboardViewController.Instance.SetGuild(p_GuildId);

            SetMode(EMode.Loading);

            //GSLogger.Instance.Log(p_GuildId, IPA.Logging.Logger.LogLevel.InfoUp);

            GuildData l_Guild = GuildSaberModule.AvailableGuilds.Where((x) => x.ID == p_GuildId).ElementAt(0);

            m_WorkingLayout.OnReady(async (x) =>
            {
                m_StatusText.SetText("Loading category file...");

                GuildName = l_Guild.Name;
                CategoryName = p_Category.Name;

                //GSLogger.Instance.Log(GuildName, IPA.Logging.Logger.LogLevel.InfoUp);
                //GSLogger.Instance.Log(CategoryName, IPA.Logging.Logger.LogLevel.InfoUp);

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

                UpdateLevelsOnUI();

                SetMode(EMode.Normal);

                PlaylistModel l_FirstLevel = Playlists.ElementAt(0);

                SetSelectedPlaylist(l_FirstLevel.customData.PlaylistLevel);

                GSLogger.Instance.Log("Finished Loading levels", IPA.Logging.Logger.LogLevel.InfoUp);
            });
        }

        public void ClearUILevels()
        {
            foreach (var l_Index in UIPlaylists)
                l_Index.SetActive(false);
        }

        public void UpdateLevelsOnUI()
        {
            ClearUILevels();

            foreach (var l_Index in UIPlaylists)
            {
                l_Index.SetHeight(0);
                l_Index.SetWidth(0);
            }

            for (int l_i = 0; l_i < Playlists.Count();l_i++)
            {
                PlaylistModel l_Model = Playlists[l_i];

                List<PlaylistModelSong> l_Songs = l_Model.songs;
                

                m_StatusText.SetText($"Loading level {l_Model.customData.PlaylistLevel}");

                if (l_i > UIPlaylists.Count - 1)
                {
                    UIPlaylists.Add(PlaylistButton.Make().SetLevel(l_Model.image, l_Songs, l_Model.customData.PlaylistLevel));
                    UIPlaylists[l_i].BuildUI(m_PlaylistsContainer.Element.Container);
                    UIPlaylists[l_i].SetActive(true);
                } else
                {
                    UIPlaylists[l_i].SetActive(true);
                    UIPlaylists[l_i].SetLevel(l_Model.image, l_Songs, l_Model.customData.PlaylistLevel);
                }
            }
        }

        public async void SetSelectedPlaylist(float p_Level)
        {
            m_SelectedLevel = p_Level;

            Beatmaps.Clear();
            MapsToDownload.Clear();

            PlaylistModel l_Playlist = Playlists.Where(x => x.customData.PlaylistLevel == p_Level).ElementAt(0);
            if (l_Playlist.Equals(null))
            {
                GSLogger.Instance.Log("Level is null" , IPA.Logging.Logger.LogLevel.InfoUp);
                return;
            }

            m_MapList.Element.ScrollTo(0, false);

            HideMapsButton();

            m_SearchTextInput.SetValue(string.Empty);

            for (int l_i = 0; l_i < l_Playlist.songs.Count();l_i++)
            {
                var l_Index = l_Playlist.songs[l_i];

                //GSLogger.Instance.Log(SongCore.Loader.CustomLevels.Keys.ElementAt(0), IPA.Logging.Logger.LogLevel.InfoUp);
                await GetSongFromHash($"{l_Index.hash.ToUpper()}", Resources.FindObjectsOfTypeAll<BeatmapLevelsModel>().First(), (p_Level) =>
                {
                    if (p_Level == null)
                    {
                        MapsToDownload.Add(l_Index.hash);
                        return;
                    }

                    List<IDifficultyBeatmap> l_Beatmaps = new List<IDifficultyBeatmap>();
                    foreach (var l_Beatmapset in p_Level.beatmapLevelData.difficultyBeatmapSets)
                    {
                        foreach (var l_Difficulty in l_Beatmapset.difficultyBeatmaps)
                        {
                            foreach (var l_PlaylistDiff in l_Index.difficulties)
                            {
                                if (l_PlaylistDiff.name.ToLower() == l_Difficulty.difficulty.ToString().ToLower() && l_PlaylistDiff.characteristic == l_Beatmapset.beatmapCharacteristic.serializedName)
                                {
                                    Beatmaps.Add(l_Difficulty);
                                    //UpdateMapButtonAtIndex(l_i, l_Difficulty);
                                }
                            }
                        }
                    }
                });
            }

            UpdateMapList(true);
        }

        public async void DownloadLevels()
        {
            m_MissingMapsText.SetText("0%");
            m_DownloadMapsButton.SetActive(false);
            for (int l_i = 0; l_i < MapsToDownload.Count;l_i++) 
            {
                await BeatSaverApi.DownloadMapByHash(MapsToDownload[l_i].ToLower());
                m_MissingMapsText.SetText($"{(((float)l_i / MapsToDownload.Count) * 100):00}%");
            }

            SongCore.Loader.SongsLoadedEvent += OnMapsFinishedToRefresh;
            SongCore.Loader.Instance.RefreshSongs(true);
            MapsToDownload.Clear();
        }

        private void OnMapsFinishedToRefresh(SongCore.Loader arg1, System.Collections.Concurrent.ConcurrentDictionary<string, CustomPreviewBeatmapLevel> arg2)
        {
            SongCore.Loader.SongsLoadedEvent -= OnMapsFinishedToRefresh;
            SetSelectedPlaylist(m_SelectedLevel);
            m_StatusText.SetText(string.Empty);
        }

        public async Task<IBeatmapLevel> GetSongFromHash(string p_Hash, BeatmapLevelsModel p_BeatmapLevelsModel, Action<IBeatmapLevel> Callback = null)
        {
            try
            {
                IPreviewBeatmapLevel l_BeatmapLevel = SongCore.Loader.GetLevelByHash(p_Hash);
                var l_Result = await p_BeatmapLevelsModel.GetBeatmapLevelAsync(l_BeatmapLevel.levelID, new System.Threading.CancellationToken());
                if (Callback != null)
                    Callback.Invoke(l_Result.beatmapLevel);
                return l_Result.beatmapLevel;

            } catch (Exception p_E)
            {
                //GSLogger.Instance.Error(p_E, nameof(LevelSelectionViewController), nameof(GetSongFromHash));
                if (Callback != null)
                    Callback.Invoke(null);
                return null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void HideMapsButton()
        {
            foreach (var l_Index in UIMaps)
            {
                l_Index.SetActive(false);
            }
        }

        public void UpdateMapList(bool p_RefreshVisuals = true, bool p_UpdateFromSearch = false)
        {
            if (MapsToDownload.Count > 0)
            {
                m_MissingMapsText.SetText($"<color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>Missing {MapsToDownload.Count} maps");
                m_DownloadMapsButton.SetActive(true);
            } else
            {
                m_MissingMapsText.SetText(string.Empty);
                m_DownloadMapsButton.SetActive(false);
            }

            if (p_RefreshVisuals)
                HideMapsButton();

            if (p_UpdateFromSearch == true)
            {
                foreach(var l_Index in UIMaps)
                {
                    if (m_SearchValue == string.Empty)
                    {
                        l_Index.SetActive(true);
                        continue;
                    }

                    l_Index.SetActive(NameCorrespond(l_Index.GetMapName(), m_SearchValue));
                }
            }

            if (!p_RefreshVisuals)
                return;

            for (int l_i = 0; l_i < Beatmaps.Count;l_i++)
            {
                UpdateMapButtonAtIndex(l_i, Beatmaps[l_i]);
            }
        }

        public void UpdateMapButtonAtIndex(int p_Index, IDifficultyBeatmap p_Beatmap)
        {
            if (p_Index > UIMaps.Count - 1)
            {
                UIMaps.Add(MapButton.Make(p_Beatmap));
                UIMaps[p_Index].BuildUI(m_MapList.Element.Container);
                UIMaps[p_Index].SetActive(true);
            }
            else
            {
                UIMaps[p_Index].SetActive(true);
                UIMaps[p_Index].SetBeatmap(p_Beatmap);
            }
        }

        public void SetSelectedMap(IDifficultyBeatmap p_Beatmap)
        {
            m_MapDetails.SetMap(p_Beatmap);
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

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public bool NameCorrespond(string p_Name, string p_SearchValue)
        {
            int l_Score = 0;
            int l_RequierdScore = p_SearchValue.Length;

            string l_Name = p_Name.ToLower();
            string l_SearchValue = p_SearchValue.ToLower();

            foreach (var l_Index in l_SearchValue)
            {
                if (l_Name.Contains(l_Index))
                    l_Score += 1;
            }
            return GuildSaberUtils.Diff(l_Score, l_RequierdScore) == 0;
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        /*protected override void OnViewActivation()
        {
            //GameObject.Find("LeftScreen").SetActive(true);

            //CampaignFlowCoordinator
        }

        protected override void OnViewDeactivation()
        {
            try
            {
                GameObject.Find("LeftScreen").SetActive(false);
            } catch
            {

            }
        }*/
    }
}
