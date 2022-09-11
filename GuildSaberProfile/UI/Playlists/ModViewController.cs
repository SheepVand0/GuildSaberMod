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
using GuildSaberProfile.Configuration;
using GuildSaberProfile.Utils;
using HMUI;
using IPA.Utilities;
using Newtonsoft.Json;
using SongCore;
using UnityEngine;
using UnityEngine.UI;
using GuildSaberProfile.API;
using TMPro;

namespace GuildSaberProfile.UI.GuildSaber
{

    public class CategoryUI
    {
        public int CurrentPlaylistIndex;
        public bool DownloadOnlyUnPassed;
        public string m_CategoryDirectory = Plugin.NOT_DEFINED;
        public string m_GuildName = "CS";
        public List<int> m_ValidPlaylists = new List<int>
            { 1, 2, 3 };
        public int PlaylistsCountInCategory;

        [UIComponent("CategoryNameText")]
        private TextMeshProUGUI m_CategoryNameText = null;

        #region UIComponents
        [UIComponent("DownloadBut")] private readonly Button m_DownloadButton = null;
        [UIComponent("ElemsHorizontal")] public readonly HorizontalLayoutGroup m_HorizontalElems = null;
        #endregion

        public CategoryUI(string p_Name, string p_GuildName, bool p_DownloadOnlyUnpassed)
        {
            CategoryName = p_Name;
            m_GuildName = p_GuildName;
            DownloadOnlyUnPassed = p_DownloadOnlyUnpassed;

            using (HttpClient l_Client = new HttpClient())
            {
                try
                {
                    Task<string> l_SerializedObject = l_Client.GetStringAsync(!CategoryName.StringIsNullOrEmpty() ?
                        $"{GuildSaberUtils.ReturnLinkFromGuild(m_GuildName)}/levelcache/{CategoryName}"
                        : $"{GuildSaberUtils.ReturnLinkFromGuild(m_GuildName)}/levelcache/");
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
            m_CategoryDirectory = $"./Playlists/GuildSaber/{m_GuildName}/{CategoryName}";

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

        public string CategoryName { get; set; } = Plugin.NOT_DEFINED;

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

            if (CategoryName == string.Empty)
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
            if (CategoryName == Plugin.NOT_DEFINED || m_ValidPlaylists.Count == 0) return;

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
                    l_QueryString = !CategoryName.StringIsNullOrEmpty()
                        ? $"{GuildSaberUtils.ReturnLinkFromGuild(m_GuildName)}/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}/{CategoryName}"
                        : $"{GuildSaberUtils.ReturnLinkFromGuild(m_GuildName)}/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}";
                else
                    l_QueryString = !CategoryName.StringIsNullOrEmpty()
                        ? $"{GuildSaberUtils.ReturnLinkFromGuild(m_GuildName)}/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}/{CategoryName}/{Plugin.m_PlayerId}"
                        : $"{GuildSaberUtils.ReturnLinkFromGuild(m_GuildName)}/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}/{Plugin.m_PlayerId}";

                Plugin.Log.Info(l_QueryString);
                try
                {
                    CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType.PlaylistsOnly);
                    l_Client.DownloadFileAsync(new Uri(l_QueryString), $"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex]:D3}_{m_GuildName}_{CategoryName}.bplist");
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
                    if (File.Exists($"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex]:D3}_{m_GuildName}_{CategoryName}.bplist"))
                        File.Delete($"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex]:D3}_{m_GuildName}_{CategoryName}.bplist");
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
    [ViewDefinition("GuildSaberProfile.UI.GuildSaber.View.ModViewController.bsml")]
    public class ModViewController : BSMLAutomaticViewController
    {
        #region Properties
        public bool m_OnlyUnPassedMaps;
        public string GuildName = GSConfig.Instance.SelectedGuild;
        public delegate void OnOnlyPassedMapsChange(bool p_OnlyUnPassed);
        public event OnOnlyPassedMapsChange e_OnUnPassedOnlyValueChanged;
        public CategoryUI CategoryInterface;
        public List<GuildCategories> m_Guilds = new List<GuildCategories>
        {
            new GuildCategories("CS", new List<string>
                { "Tech", "Vibro", "Streams", "Jumps", "Shitpost" }),
            new GuildCategories("BSCC", new List<string>
                { "Main" })
        };
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
            GuildName = p_Selected;
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
                List<GuildCategories> l_Categories = new List<GuildCategories>();
                m_Guilds.Clear();
                foreach (string l_CurrentGuild in Plugin.AvailableGuilds)
                {
                    PlayerGuildsInfo l_Player = GuildApi.GetPlayerInfoFromAPI(false, l_CurrentGuild);
                    if (l_Player.m_AvailableGuilds.Count == 0)
                        continue;

                    GuildCategories l_Guild = new GuildCategories(l_CurrentGuild, new List<string>());
                    foreach (CustomApiPlayerCategory l_Current in l_Player.m_ReturnPlayer.CategoryData)
                        l_Guild.Categories.Add(l_Current.Category);
                    l_Categories.Add(l_Guild);
                }
                m_Guilds = l_Categories;
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
                    l_Guilds.Add(l_Current.GuildName);
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
                GuildCategories l_CurrentGuild = GetGuildFromName(m_Guilds, SelectedGuild);
                if (l_CurrentGuild.GuildName == Plugin.NOT_DEFINED) { Plugin.Log.Error($"Selected guild not valid : returned guild {l_CurrentGuild.GuildName}"); return; }
                foreach (string l_Current in l_CurrentGuild.Categories)
                    m_ListCategories.Add(new CategoryUI(l_Current, GuildName, m_OnlyUnPassedMaps));
                m_ListCategories.Add(new CategoryUI(string.Empty, GuildName, m_OnlyUnPassedMaps));
            });
            if (m_CategoriesTableList != null)
                m_CategoriesTableList.tableView.ReloadData();
            return Task.CompletedTask;
        }

        public void Init(string p_GuildName)
        {
            GuildName = p_GuildName;
        }

        public GuildCategories GetGuildFromName(List<GuildCategories> p_Guilds, string p_Name)
        {
            foreach (GuildCategories l_Current in p_Guilds)
                if (l_Current.GuildName == p_Name)
                    return l_Current;
            return new GuildCategories(Plugin.NOT_DEFINED, new List<string>
                { Plugin.NOT_DEFINED });
        }

        public List<string> GetGuildsName()
        {
            List<string> l_Temp = new List<string>();
            foreach (GuildCategories l_Current in m_Guilds)
                l_Temp.Add(l_Current.GuildName);
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
            get => GuildName;
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
