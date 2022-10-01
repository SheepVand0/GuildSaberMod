using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Components.Settings;
using GuildSaber.Configuration;
using GuildSaber.Utils;
using HMUI;
using IPA.Utilities;
using Newtonsoft.Json;
using SongCore;
using UnityEngine;
using UnityEngine.UI;
using GuildSaber.API;
using TMPro;

namespace GuildSaber.UI.GuildSaber
{

    public class CategoryUI
    {
        public int CurrentPlaylistIndex;
        public bool DownloadOnlyUnPassed;
        public string m_CategoryDirectory = Plugin.NOT_DEFINED;
        public int m_GuildId = 1;
        public List<int> m_ValidPlaylists = new List<int>
            { 1, 2, 3 };
        public int PlaylistsCountInCategory;

        [UIComponent("CategoryNameText")]
        private TextMeshProUGUI m_CategoryNameText = null;

        #region UIComponents
        [UIComponent("DownloadBut")] private readonly Button m_DownloadButton = null;
        [UIComponent("ElemsHorizontal")] public readonly HorizontalLayoutGroup m_HorizontalElems = null;
        #endregion

        public CategoryUI(CategoryData p_Category, int p_GuildId, bool p_DownloadOnlyUnpassed)
        {
            Category = p_Category;
            m_GuildId = p_GuildId;
            DownloadOnlyUnPassed = p_DownloadOnlyUnpassed;

            using (HttpClient l_Client = new HttpClient())
            {
                try
                {
                    Task<string> l_SerializedObject = l_Client.GetStringAsync($"https://api.guildsaber.com/levels/data/all?guild=1{Category}");
                    l_SerializedObject.Wait();
                    LevelIDs l_TempValidLevels = JsonConvert.DeserializeObject<LevelIDs>(l_SerializedObject.Result);
                    m_ValidPlaylists = l_TempValidLevels.LevelID;
                }
                catch (HttpRequestException p_E)
                {
                    m_ValidPlaylists = new List<int>() { };
                    Plugin.Log.Error(p_E);
                }
            }

            PlaylistsCountInCategory = m_ValidPlaylists.Count;
            m_CategoryDirectory = $"./Playlists/GuildSaber/{m_GuildId}/{Category}";

            if (PlaylistsCountInCategory == 0)
            {
                m_DownloadButton.interactable = false;
                m_DownloadButton.SetButtonText("Error");
            }
        }
        public void UnbindEvent()
        {
            Plugin._modFlowCoordinator._modViewController.e_OnUnPassedOnlyValueChanged -= OnUnpassedOnlyChanged;
        }

        public CategoryData Category { get; set; }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            ImageView l_Background = m_HorizontalElems.GetComponent<ImageView>();

            l_Background.SetField("_skew", 0.0f);
            l_Background.SetImage("#RoundRect10BorderFade");

            l_Background.color = UnityEngine.Color.white.ColorWithAlpha(0.5f);
            l_Background.color0 = UnityEngine.Color.white.ColorWithAlpha(0.4f);
            l_Background.color1 = UnityEngine.Color.white.ColorWithAlpha(0.4f);
            l_Background.overrideSprite = null;

            Plugin._modFlowCoordinator._modViewController.e_OnUnPassedOnlyValueChanged += OnUnpassedOnlyChanged;

            if (Category.CategoryName == string.Empty)
                m_CategoryNameText.text = "Default";
        }
        private void OnUnpassedOnlyChanged(bool p_UnPassedOnly)
        {
            Plugin.Log.Info($"Updating On Un Passed Only: {p_UnPassedOnly}");
            DownloadOnlyUnPassed = p_UnPassedOnly;
        }

        [UIAction("DownloadPlaylist")]
        private void PreDownloadPlaylist()
        {
            CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType.FolderOnly);
            DownloadPlaylist();
        }

        private async void DownloadPlaylist()
        {
            if (Category.CategoryName == Plugin.NOT_DEFINED || m_ValidPlaylists.Count == 0) return;

            if (CurrentPlaylistIndex == PlaylistsCountInCategory)
            {
                m_DownloadButton.SetButtonText("Finished");
                m_DownloadButton.interactable = false;
                Loader.Instance.RefreshSongs();
                return;
            }

            using (WebClient l_Client = new WebClient())
            {
                Plugin.Log.Info((CurrentPlaylistIndex / (float)PlaylistsCountInCategory).ToString());
                m_DownloadButton.SetButtonText($"Downloading {CurrentPlaylistIndex / (float)PlaylistsCountInCategory * 100:00}%");
                m_DownloadButton.interactable = false;
                string l_QueryString = string.Empty;
                if (!DownloadOnlyUnPassed)
                    l_QueryString = !Category.CategoryName.StringIsNullOrEmpty()
                        ? $"https://api.guildsaber.com/level/{m_ValidPlaylists[CurrentPlaylistIndex]}/{Category}"
                        : $"https://api.guildsaber.com/level/{m_ValidPlaylists[CurrentPlaylistIndex]}";
                else
                    l_QueryString = !Category.CategoryName.StringIsNullOrEmpty()
                        ? $"https://api.guildsaber.com/level/{m_ValidPlaylists[CurrentPlaylistIndex]}/{Category}/{Plugin.m_PlayerId}"
                        : $"https://api.guildsaber.com/level/{m_ValidPlaylists[CurrentPlaylistIndex]}/{Plugin.m_PlayerId}";

                Plugin.Log.Info(l_QueryString);
                try
                {
                    CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType.PlaylistsOnly);
                    l_Client.DownloadFileAsync(new Uri(l_QueryString), $"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex]:D3}_{m_GuildId}_{Category}.bplist");
                    l_Client.DownloadFileCompleted += (p_Sender, p_Event) =>
                    {
                        if (p_Event.Error == null)
                        {
                            CurrentPlaylistIndex += 1;
                            DownloadPlaylist();
                        }
                        else
                            if (DownloadOnlyUnPassed)
                        {
                            CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType.PlaylistsOnly);
                            CurrentPlaylistIndex += 1;
                            DownloadPlaylist();
                        }
                        else
                        {
                            m_DownloadButton.SetButtonText("Error");
                            m_DownloadButton.interactable = false;
                        }
                    };
                }
                catch (WebException p_E)
                {
                    Plugin.Log.Error(p_E);
                }
            }
        }

        private void CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType p_VerificationType)
        {
            switch (p_VerificationType)
            {
                case PlaylistsVerificationType.FolderOnly:
                    if (!Directory.Exists(m_CategoryDirectory))
                        Directory.CreateDirectory(m_CategoryDirectory);
                    break;
                case PlaylistsVerificationType.PlaylistsOnly:
                    if (File.Exists($"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex]:D3}_{m_GuildId}_{Category}.bplist"))
                        File.Delete($"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex]:D3}_{m_GuildId}_{Category}.bplist");
                    break;
                case PlaylistsVerificationType.All:
                    CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType.FolderOnly);
                    CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType.PlaylistsOnly);
                    break;
                default: return;
            }
        }
    }

    [HotReload(RelativePathToLayout = @"ModViewController.bsml")]
    [ViewDefinition("GuildSaber.UI.GuildSaber.View.ModViewController.bsml")]
    public class ModViewController : BSMLAutomaticViewController
    {
        #region Properties
        public bool m_OnlyUnPassedMaps;
        public int GuildId = GSConfig.Instance.SelectedGuild;
        public delegate void OnOnlyPassedMapsChange(bool p_OnlyUnPassed);
        public event OnOnlyPassedMapsChange e_OnUnPassedOnlyValueChanged;
        public CategoryUI CategoryInterface;
        public List<GuildCategories> m_Guilds = new List<GuildCategories>();
        #endregion

        #region UIComponents
        [UIComponent("CategoryList")] public CustomCellListTableData m_CategoriesTableList = null;
        [UIComponent("Dropdown")] public DropDownListSetting m_GuildChoiceDropdown = null;
        [UIComponent("LoadingLayout")] public GridLayoutGroup m_LoadingGrid = null;
        [UIComponent("Elems")] public VerticalLayoutGroup m_ElemsLayout = null;
        #endregion

        #region UIActions
        [UIAction("#post-parse")]
        private void PostParse()
        {
            try
            {
                UpdateCategories();
            } catch (Exception l_E)
            {
                Plugin.Log.Error(l_E.StackTrace);
            }
        }

        [UIAction("OnGuildChange")]
        public async void OnGuildChange(string p_Selected)
        {
            SetLoadingMode(LoadingMode.loading);
            SelectedGuild = p_Selected;
            GuildId = GuildApi.GetGuildFromName(p_Selected).ID;
            await RefreshList();
            SetLoadingMode(LoadingMode.normal);
        }
        #endregion

        public void SetLoadingMode(LoadingMode p_Mode)
        {
            switch (p_Mode)
            {
                case LoadingMode.loading:
                    m_LoadingGrid.gameObject.SetActive(true);
                    m_GuildChoiceDropdown.interactable = false;
                    break;
                case LoadingMode.normal:
                    m_GuildChoiceDropdown.interactable = true;
                    m_LoadingGrid.gameObject.SetActive(false);
                    break;
                default: return;
            }
        }

        public async void UpdateCategories()
        {
            await Task.Run(delegate
            {
                SetLoadingMode(LoadingMode.loading);
                List<CategoryData> l_Categories = new List<CategoryData>();
                foreach (GuildData l_CurrentGuild in Plugin.AvailableGuilds)
                {
                    PlayerGuildsInfo l_Player = GuildApi.GetPlayerGuildsInfo();
                    if (l_Player.m_AvailableGuilds.Count == 0)
                        continue;

                    GuildCategories l_Guild = new GuildCategories(l_CurrentGuild.ID, new List<CategoryData>());
                    foreach (CategoryData l_Current in l_Player.m_ReturnPlayer.CategoryData)
                        l_Guild.Categories.Add(l_Current);
                    m_Guilds.Add(l_Guild);
                }
            });
            await RefreshDropdown();
            await RefreshList();
            SetLoadingMode(LoadingMode.normal);
        }

        public async Task<Task> RefreshDropdown()
        {
            await Task.Run(delegate
            {
                List<string> l_Guilds = new List<string>();
                foreach (GuildCategories l_Current in m_Guilds)
                    l_Guilds.Add(GuildSaberUtils.GetGuildFromId(l_Current.GuildId).Name);
                m_AvailableGuilds.Clear();
                foreach (string l_Current in l_Guilds)
                    m_AvailableGuilds.Add(l_Current);
                m_GuildChoiceDropdown.UpdateChoices();
                SelectedGuild = (string)m_GuildChoiceDropdown.Value;
            });
            return Task.CompletedTask;
        }

        public async Task<Task> RefreshList()
        {
            await Task.Run(delegate
            {
                foreach (CategoryUI l_Current in m_ListCategories)
                {
                    l_Current.UnbindEvent();
                    GameObject.DestroyImmediate(l_Current.m_HorizontalElems.gameObject);
                }
                m_ListCategories.Clear();
                List<CategoryData> l_Categories = GuildApi.GetPlayerCategoriesDataForGuild(BSPModule.GuildSaber.m_SSPlayerId, GuildId);
                foreach (CategoryData l_Current in l_Categories)
                {
                    m_ListCategories.Add(new CategoryUI(l_Current, GuildId, m_OnlyUnPassedMaps));
                }
            });
            if (m_CategoriesTableList != null)
                m_CategoriesTableList.tableView.ReloadData();
            return Task.CompletedTask;
        }

        public void Init(int p_GuildId)
        {
            GuildId = p_GuildId;
        }

        public GuildData GetGuildFromName(string p_Name)
        {
            foreach (GuildData l_Current in Plugin.AvailableGuilds)
            {
                if (l_Current.Name != p_Name)
                    continue;

                return l_Current;
            }

            return default(GuildData);
        }

        public List<string> GetGuildsName()
        {
            List<string> l_Temp = new List<string>();
            foreach (GuildData l_Current in Plugin.AvailableGuilds)
                l_Temp.Add(l_Current.Name);
            return l_Temp;
        }

        #region UIValues
        [UIValue("AvailableCategories")]
        public List<object> m_ListCategories = new List<object>();

        [UIValue("AvailableGuilds")]
        public List<object> m_AvailableGuilds
        {
            get
            {
                List<object> l_TempList = new List<object>();
                foreach (string l_Current in GetGuildsName())
                    l_TempList.Add(l_Current);
                return l_TempList;
            }
            set { }
        }

        [UIValue("SelectedGuild")]
        public string SelectedGuild
        {
            get => BSPModule.GuildSaber.m_PlaylistDownloadSelectedGuild.Name;
            set { }
        }

        [UIValue("UnPassedMaps")]
        private bool DlUnPassedMaps
        {
            get => m_OnlyUnPassedMaps;
            set
            {
                SetLoadingMode(LoadingMode.loading);
                m_OnlyUnPassedMaps = value;
                e_OnUnPassedOnlyValueChanged?.Invoke(m_OnlyUnPassedMaps);
                RefreshFromUnpassed();
            }
        }

        [UIValue("UwU")]
        private bool UwUMode
        {
            get => GSConfig.Instance.UwUMode;
            set => GSConfig.Instance.UwUMode = value;
        }
        #endregion

        public async void RefreshFromUnpassed()
        {
            await RefreshList();
            SetLoadingMode(LoadingMode.normal);
        }
    }
}

public class LevelIDs
{
    public List<int> LevelID { get; set; }
}

public enum PlaylistsVerificationType
{
    FolderOnly, PlaylistsOnly, All
}

public enum LoadingMode
{
    normal, loading
}
