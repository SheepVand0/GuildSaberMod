using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using GuildSaber.Configuration;
using HMUI;
using IPA.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using GuildSaber.API;
using TMPro;
using GuildSaber.BSPModule;
using BeatSaberPlus.SDK.UI;
using System.Reflection;
using PlaylistManager.Utilities;
using GuildSaber.Logger;
using PlaylistManager.UI;

namespace GuildSaber.UI.GuildSaber
{

    public class CategoryUI
    {
        public int CurrentPlaylistIndex = 0;
        public bool DownloadOnlyUnPassed = false;
        public string m_CategoryDirectory = Plugin.NOT_DEFINED;
        public int m_GuildId = 1;
        internal List<ApiRankingLevel> m_CategoryLevels = new() { };
        public int PlaylistsCountInCategory = 0;

        public bool Inited = false;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent("CategoryNameText")]
        private TextMeshProUGUI m_CategoryNameText = null;

        [UIComponent("DownloadBut")] private readonly UnityEngine.UI.Button m_DownloadButton = null;
        [UIComponent("ElemsHorizontal")] public readonly HorizontalLayoutGroup m_HorizontalElems = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public CategoryUI(ApiCategory p_Category, int p_GuildId, bool p_DownloadOnlyUnpassed)
        {
            Category = p_Category;
            m_GuildId = p_GuildId;
            DownloadOnlyUnPassed = p_DownloadOnlyUnpassed;
        }

        private async void Init()
        {
            using (HttpClient l_Client = new HttpClient())
            {
                try
                {
                    string l_CategoryQueryString = Category.ID == 0 ? string.Empty : $"&category-id={Category.ID}";
                    string l_SerializedObject;
                    //Plugin.Log.Info($"https://api.guildsaber.com/levels/data/all?guild-id={m_GuildId}{l_CategoryQueryString}");
                    l_SerializedObject = await l_Client.GetStringAsync($"https://api.guildsaber.com/levels/data/all?guild-id={m_GuildId}{l_CategoryQueryString}");
                    List<ApiRankingLevel> l_TempValidLevels = JsonConvert.DeserializeObject<List<ApiRankingLevel>>(l_SerializedObject);
                    foreach (var l_Index in l_TempValidLevels)
                        m_CategoryLevels.Add(l_Index);
                }
                catch (HttpRequestException p_E)
                {
                    m_CategoryLevels = new List<ApiRankingLevel>() { };
                    GSLogger.Instance.Error(p_E, nameof(PlaylistDownloaderViewController), nameof(Init));
                }

                PlaylistsCountInCategory = m_CategoryLevels.Count;
                m_CategoryDirectory = $"./Playlists/GuildSaber/{GuildApi.GetGuildFromId(m_GuildId).Name}/{Category.Name}";

                if (PlaylistsCountInCategory == 0)
                {
                    m_DownloadButton.interactable = false;
                    m_DownloadButton.SetButtonText("Error");
                }
                Inited = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void UnbindEvent()
        {
            Plugin._modFlowCoordinator._modViewController.e_OnUnPassedOnlyValueChanged -= OnUnpassedOnlyChanged;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public ApiCategory Category { get; set; }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIAction("#post-parse")]
        private void PostParse()
        {
            Init();

            ImageView l_Background = m_HorizontalElems.GetComponent<ImageView>();

            l_Background.SetField("_skew", 0.0f);
            l_Background.SetImage("#RoundRect10BorderFade");

            l_Background.color = UnityEngine.Color.white.ColorWithAlpha(0.5f);
            l_Background.color0 = UnityEngine.Color.white.ColorWithAlpha(0.4f);
            l_Background.color1 = UnityEngine.Color.white.ColorWithAlpha(0.4f);
            l_Background.overrideSprite = null;

            Plugin._modFlowCoordinator._modViewController.e_OnUnPassedOnlyValueChanged += OnUnpassedOnlyChanged;

            m_CategoryNameText.text = (Category.ID == 0) ? "Default" : Category.Name;

            new HMUI.ButtonBinder().AddBinding(m_DownloadButton, PreDownloadPlaylist);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private void OnUnpassedOnlyChanged(bool p_UnPassedOnly)
        {
            DownloadOnlyUnPassed = p_UnPassedOnly;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private void PreDownloadPlaylist()
        {
            CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType.FolderOnly);
            DownloadPlaylist();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private void DownloadPlaylist()
        {
            if (Category.Name == Plugin.NOT_DEFINED || m_CategoryLevels.Count == 0) return;

            if (CurrentPlaylistIndex == PlaylistsCountInCategory)
            {
                m_DownloadButton.SetButtonText("Finished");
                m_DownloadButton.interactable = false;
                PlaylistLibUtils.playlistManager.RefreshPlaylists(true);
                return;
            }

            using (WebClient l_Client = new WebClient())
            {
                //Plugin.Log.Info((CurrentPlaylistIndex / (float)PlaylistsCountInCategory).ToString());
                m_DownloadButton.SetButtonText($"Downloading {CurrentPlaylistIndex / (float)PlaylistsCountInCategory * 100:00}%");
                m_DownloadButton.interactable = false;
                string l_QueryString = string.Empty;
                string l_CategoryQueryString = Category.ID == 0 ? string.Empty : $"category-id={Category.ID}";
                if (DownloadOnlyUnPassed)
                    l_QueryString = $"https://api.guildsaber.com/playlists/data/by-id/{m_CategoryLevels[CurrentPlaylistIndex].ID}?player-id={GuildSaberModule.m_GSPlayerId}&{l_CategoryQueryString}";
                else
                    l_QueryString = $"https://api.guildsaber.com/playlists/data/by-id/{m_CategoryLevels[CurrentPlaylistIndex].ID}?{l_CategoryQueryString}";

                //Plugin.Log.Info(l_QueryString);
                try
                {
                    CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType.PlaylistsOnly);
                    l_Client.DownloadFileAsync(new Uri(l_QueryString), $"{m_CategoryDirectory}/{CurrentPlaylistIndex:D3}_{Category.Name}.bplist");
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
                    GSLogger.Instance.Error(p_E, nameof(CategoryUI), nameof(DownloadPlaylist));
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
                    if (File.Exists($"{m_CategoryDirectory}/{m_CategoryLevels[CurrentPlaylistIndex]:D3}_{m_GuildId}_{Category}.bplist"))
                        File.Delete($"{m_CategoryDirectory}/{m_CategoryLevels[CurrentPlaylistIndex]:D3}_{m_GuildId}_{Category}.bplist");
                    break;
                case PlaylistsVerificationType.All:
                    CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType.FolderOnly);
                    CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType.PlaylistsOnly);
                    break;
                default: return;
            }
        }
    }


    public class PlaylistViewController : ViewController<PlaylistViewController>
    {
        protected override string GetViewContentDescription()
        {
            return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "GuildSaber.UI.GuildSaber.View.ModViewController.bsml");
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public bool m_OnlyUnPassedMaps;
        public int GuildId = GSConfig.Instance.SelectedGuild;
        public delegate void OnOnlyPassedMapsChange(bool p_OnlyUnPassed);
        public event OnOnlyPassedMapsChange e_OnUnPassedOnlyValueChanged = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent("CategoryList")] public CustomCellListTableData m_CategoriesTableList = null;
        [UIComponent("Dropdown")] public BeatSaberMarkupLanguage.Components.Settings.DropDownListSetting m_GuildChoiceDropdown = null;
        [UIComponent("LoadingLayout")] public GridLayoutGroup m_LoadingGrid = null;
        [UIComponent("Elems")] public VerticalLayoutGroup m_ElemsLayout = null;

        [UIObject("BG")] GameObject m_BG = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override void OnViewCreation()
        {
            BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_BG, 0.5f);
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                return;
            UpdateCategories();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////s

        public void Init(int p_GuildId)
        {
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                return;
            GuildId = p_GuildId;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIAction("OnGuildChange")]
        public void OnGuildChange(string p_Selected)
        {
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                return;

            SetLoadingMode(LoadingMode.loading);
            DropdownSelectedGuild = p_Selected;
            SelectedGuild = GuildApi.GetGuildFromName(p_Selected).ID;
            GuildId = GuildApi.GetGuildFromName(p_Selected).ID;
            RefreshList();
            SetLoadingMode(LoadingMode.normal);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void UpdateCategories()
        {
            if (GuildSaberModule.IsStateError())
            {
                ShowMessageModal("<color=#ff0000>Error on getting info</color>");
                return;
            }

            //await WaitUtils.WaitUntil(() => m_LoadingGrid != null, 10);
            SetLoadingMode(LoadingMode.loading);

            RefreshDropdown();
            RefreshList();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void RefreshDropdown()
        {
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                return;
            m_AvailableGuilds.Clear();
            List<string> l_Guilds = new List<string>();
            foreach (GuildData l_Guild in GuildSaberModule.AvailableGuilds)
                l_Guilds.Add(l_Guild.Name);
            foreach (string l_Current in l_Guilds)
                m_AvailableGuilds.Add(l_Current);
            m_GuildChoiceDropdown.UpdateChoices();
            DropdownSelectedGuild = (string)m_GuildChoiceDropdown.Value;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void SetLoadingMode(LoadingMode p_Mode)
        {
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                return;

            m_LoadingGrid.gameObject.SetActive(p_Mode == LoadingMode.loading);
            m_GuildChoiceDropdown.interactable = p_Mode == LoadingMode.normal;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////s

        public async void RefreshList()
        {
            if (GuildSaberModule.IsStateError())
                return;

            foreach (CategoryUI l_Current in m_ListCategories)
            {
                l_Current.UnbindEvent();
            }
            m_ListCategories.Clear();
            List<ApiCategory> l_Categories = await GuildApi.GetCategoriesForGuild(GuildId);
            l_Categories.Add(default(ApiCategory));
            foreach (ApiCategory l_Current in l_Categories)
            {
                CategoryUI l_CategoryUI = new CategoryUI(l_Current, GuildId, m_OnlyUnPassedMaps);
                m_ListCategories.Add(l_CategoryUI);
            }
            if (m_CategoriesTableList != null)
                m_CategoriesTableList.tableView.ReloadData();
            SetLoadingMode(LoadingMode.normal);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public List<string> GetGuildsName()
        {
            if (GuildSaberModule.IsStateError())
                return new List<string> { "Undefined" };
            List<string> l_Temp = new List<string>();
            foreach (GuildData l_Current in GuildSaberModule.AvailableGuilds)
                l_Temp.Add(l_Current.Name);
            return l_Temp;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIValue("AvailableCategories")]
        public List<object> m_ListCategories = new List<object>();

        [UIValue("AvailableGuilds")]
        public List<object> m_AvailableGuilds
        {
            get
            {
                List<object> l_TempList = new List<object>();
                if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                {
                    l_TempList.Add("Undefined");
                    return l_TempList;
                }
                foreach (string l_Current in GetGuildsName())
                    l_TempList.Add(l_Current);
                return l_TempList;
            }
            set { }
        }

        [UIValue("SelectedGuild")]
        public string DropdownSelectedGuild
        {
            get => string.Empty;
            set { }
        }

        public int SelectedGuild = GSConfig.Instance.SelectedGuild;

        [UIValue("UnPassedMaps")]
        private bool DlUnPassedMaps
        {
            get => m_OnlyUnPassedMaps;
            set
            {
                m_OnlyUnPassedMaps = value;
                if (GuildSaberModule.IsStateError())
                    return;
                SetLoadingMode(LoadingMode.loading);
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

        public void RefreshFromUnpassed()
        {
            RefreshList();
            SetLoadingMode(LoadingMode.normal);
        }
    }
}

public enum PlaylistsVerificationType
{
    FolderOnly, PlaylistsOnly, All
}

public enum LoadingMode
{
    normal, loading
}
