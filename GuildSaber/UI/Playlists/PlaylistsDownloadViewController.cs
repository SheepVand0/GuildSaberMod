using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberPlus.SDK.UI;
using GuildSaber.API;
using GuildSaber.BSPModule;
using GuildSaber.Configuration;
using GuildSaber.Logger;
using GuildSaber.Utils;
using HMUI;
using IPA.Utilities;
using Newtonsoft.Json;
using PlaylistManager.UI;
using PlaylistManager.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Backgroundable = BeatSaberPlus.SDK.UI.Backgroundable;
using Button = UnityEngine.UI.Button;
using DropDownListSetting = BeatSaberMarkupLanguage.Components.Settings.DropDownListSetting;

namespace GuildSaber.UI.GuildSaber
{

    public class CategoryUI
    {
        public int CurrentPlaylistIndex = 0;
        public bool DownloadOnlyUnPassed = false;
        public string m_CategoryDirectory = Plugin.NOT_DEFINED;
        public int m_GuildId = 1;
        internal List<ApiRankingLevel> m_CategoryLevels = new()
        {
        };
        public int PlaylistsCountInCategory = 0;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent("CategoryNameText")]
        private readonly TextMeshProUGUI m_CategoryNameText = null;

        [UIComponent("DownloadBut")] private readonly Button m_DownloadButton = null;
        [UIComponent("ElemsHorizontal")] public readonly HorizontalLayoutGroup m_HorizontalElems = null;
        [UIComponent("ClearCategoryButton")] private readonly Button m_ClearCategoryButton = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public CategoryUI(ApiCategory p_Category, int p_GuildId, bool p_DownloadOnlyUnPassed)
        {
            Category = p_Category;
            m_GuildId = p_GuildId;
            DownloadOnlyUnPassed = p_DownloadOnlyUnPassed;
        }

        private async void Init()
        {
            using HttpClient l_Client = new HttpClient();
            try
            {
                string l_CategoryQueryString = Category.ID == 0 ? string.Empty : $"&category-id={Category.ID}";
                string l_SerializedObject =
                    await l_Client.GetStringAsync($"https://api.guildsaber.com/levels/data/all?guild-id={m_GuildId}{l_CategoryQueryString}");
                List<ApiRankingLevel> l_TempValidLevels = JsonConvert.DeserializeObject<List<ApiRankingLevel>>(l_SerializedObject);
                foreach (var l_Index in l_TempValidLevels)
                    m_CategoryLevels.Add(l_Index);
            }
            catch (HttpRequestException l_E)
            {
                m_CategoryLevels = new List<ApiRankingLevel>();
                GSLogger.Instance.Error(l_E, nameof(PlaylistDownloaderViewController), nameof(Init));
            }

            PlaylistsCountInCategory = m_CategoryLevels.Count;
            m_CategoryDirectory = $"./Playlists/GuildSaber/{GuildApi.GetGuildFromId(m_GuildId).Name}/{Category.Name}";

            if (PlaylistsCountInCategory == 0)
            {
                m_DownloadButton.interactable = false;
                m_DownloadButton.SetButtonText("Error");
            }

            CheckCategoryForButton();
        }

        private async void CheckCategoryForButton()
        {
            await WaitUtils.Wait(() => m_ClearCategoryButton != null, 1);

            m_ClearCategoryButton.gameObject.SetActive(Directory.Exists(m_CategoryDirectory));
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void UnbindEvent()
        {
            Plugin._modFlowCoordinator._modViewController.e_OnUnPassedOnlyValueChanged -= OnUnPassedOnlyChanged;
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

            l_Background.color = Color.white.ColorWithAlpha(0.5f);
            l_Background.color0 = Color.white.ColorWithAlpha(0.4f);
            l_Background.color1 = Color.white.ColorWithAlpha(0.4f);
            l_Background.overrideSprite = null;

            Plugin._modFlowCoordinator._modViewController.e_OnUnPassedOnlyValueChanged += OnUnPassedOnlyChanged;

            m_CategoryNameText.text = (Category.ID == 0) ? "Default" : Category.Name;

            new ButtonBinder().AddBinding(m_DownloadButton, PreDownloadPlaylist);
        }

        [UIAction("ClearCategoryFolder")]
        private void ClearCategory()
        {
            if (!Directory.Exists(m_CategoryDirectory))
                return;

            foreach (string l_File in Directory.GetFiles(m_CategoryDirectory))
            {
                File.Delete(l_File);
            }

            Directory.Delete(m_CategoryDirectory);

            CheckCategoryForButton();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private void OnUnPassedOnlyChanged(bool p_UnPassedOnly)
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
                CheckCategoryForButton();
                PlaylistLibUtils.playlistManager.RefreshPlaylists(true);
                return;
            }

            using WebClient l_Client = new();
            //Plugin.Log.Info((CurrentPlaylistIndex / (float)PlaylistsCountInCategory).ToString());
            m_DownloadButton.SetButtonText($"Downloading {CurrentPlaylistIndex / (float)PlaylistsCountInCategory * 100:00}%");
            m_DownloadButton.interactable = false;
            string l_CategoryQueryString = Category.ID == 0 ? string.Empty : $"category-id={Category.ID}";
            string l_PlayerIDQuery = (DownloadOnlyUnPassed) ? $"?player-id={GuildSaberModule.GSPlayerId}&" : "?";

            string l_QueryString = $"https://api.guildsaber.com/playlists/data/by-id/{m_CategoryLevels[CurrentPlaylistIndex].ID}{l_PlayerIDQuery}{l_CategoryQueryString}";
            //Plugin.Log.Info(l_QueryString);
            try
            {
                CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType.PlaylistsOnly);
                l_Client.DownloadFileAsync(new Uri(l_QueryString), $"{m_CategoryDirectory}/{m_CategoryLevels[CurrentPlaylistIndex].LevelNumber:000}_{Category.Name}.bplist");
                l_Client.DownloadFileCompleted += (p_Sender, p_Event) =>
                {
                    if (p_Event.Error == null)
                    {
                        CurrentPlaylistIndex += 1;
                        DownloadPlaylist();
                    }
                    else if (DownloadOnlyUnPassed)
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
            catch (WebException l_E)
            {
                GSLogger.Instance.Error(l_E, nameof(CategoryUI), nameof(DownloadPlaylist));
                m_DownloadButton.interactable = false;
                m_DownloadButton.SetButtonText("Error");
            }
        }

        private void CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType p_VerificationType)
        {
            while (true)
            {
                switch (p_VerificationType)
                {
                    case PlaylistsVerificationType.FolderOnly:
                        if (!Directory.Exists(m_CategoryDirectory)) Directory.CreateDirectory(m_CategoryDirectory);
                        break;
                    case PlaylistsVerificationType.PlaylistsOnly:
                        string l_Path = BuildLevelPath(m_CategoryDirectory, m_CategoryLevels[CurrentPlaylistIndex].LevelNumber, m_GuildId, Category.Name);
                        if (File.Exists(l_Path)) File.Delete(l_Path);
                        break;
                    case PlaylistsVerificationType.All:
                        CheckAndDeleteIfPlaylistOrFolderExists(PlaylistsVerificationType.FolderOnly);
                        p_VerificationType = PlaylistsVerificationType.PlaylistsOnly;
                        continue;
                    default:
                        return;
                }
                break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        static string BuildLevelPath(string p_CategoryDirectory, float p_LevelNumber, int p_GuildId, string p_CategoryName)
        {
            return $"{p_CategoryDirectory}/{p_LevelNumber:000}_{p_GuildId}_{p_CategoryName}.bplist";
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
        public int m_GuildId = GSConfig.Instance.SelectedGuild;

        public delegate void OnOnlyPassedMapsChange(bool p_OnlyUnPassed);

        public event OnOnlyPassedMapsChange e_OnUnPassedOnlyValueChanged = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent("CategoryList")] public CustomCellListTableData m_CategoriesTableList = null;
        [UIComponent("Dropdown")] public DropDownListSetting m_GuildChoiceDropdown = null;
        [UIComponent("LoadingLayout")] public GridLayoutGroup m_LoadingGrid = null;
        [UIComponent("Elems")] public VerticalLayoutGroup m_ElemsLayout = null;

        [UIObject("BG")] private readonly GameObject m_BG = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override void OnViewCreation()
        {
            Backgroundable.SetOpacity(m_BG, 0.5f);
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                return;
            UpdateCategories();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void Init(int p_GuildId)
        {
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                return;
            m_GuildId = p_GuildId;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIAction("OnGuildChange")]
        public void OnGuildChange(string p_Selected)
        {
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                return;

            SetLoadingMode(LoadingMode.Loading);
            DropdownSelectedGuild = p_Selected;
            m_GuildId = GuildApi.GetGuildFromName(p_Selected).ID;
            RefreshList();
            SetLoadingMode(LoadingMode.Normal);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void UpdateCategories()
        {
            SetLoadingMode(LoadingMode.Loading);

            if (GuildSaberModule.IsStateError())
            {
                ShowMessageModal("<color=#ff0000>Error on getting info</color>");
                return;
            }

            RefreshDropdown();
            RefreshList();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void RefreshDropdown()
        {
            if (GuildSaberModule.IsStateError())
                return;
            AvailableGuilds.Clear();
            List<string> l_Guilds = new();
            foreach (GuildData l_Guild in GuildSaberModule.AvailableGuilds)
                l_Guilds.Add(l_Guild.Name);
            foreach (string l_Current in l_Guilds)
                AvailableGuilds.Add(l_Current);
            m_GuildChoiceDropdown.UpdateChoices();
            DropdownSelectedGuild = (string)m_GuildChoiceDropdown.Value;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void SetLoadingMode(LoadingMode p_Mode)
        {
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                return;

            m_LoadingGrid.gameObject.SetActive(p_Mode == LoadingMode.Loading);
            m_GuildChoiceDropdown.gameObject.SetActive(p_Mode == LoadingMode.Normal);
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
            List<ApiCategory> l_Categories = await GuildApi.GetCategoriesForGuild(m_GuildId);
            l_Categories.Add(default(ApiCategory));
            foreach (ApiCategory l_Current in l_Categories)
            {
                CategoryUI l_CategoryUI = new CategoryUI(l_Current, m_GuildId, m_OnlyUnPassedMaps);
                m_ListCategories.Add(l_CategoryUI);
            }
            if (m_CategoriesTableList != null)
                m_CategoriesTableList.tableView.ReloadData();
            SetLoadingMode(LoadingMode.Normal);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private List<string> GetGuildsName()
        {
            if (GuildSaberModule.IsStateError())
                return new List<string>
                {
                    "Undefined"
                };
            List<string> l_Temp = new List<string>();
            foreach (GuildData l_Current in GuildSaberModule.AvailableGuilds)
                l_Temp.Add(l_Current.Name);
            return l_Temp;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIValue("AvailableCategories")]
        public List<object> m_ListCategories = new();

        [UIValue("AvailableGuilds")]
        public List<object> AvailableGuilds
        {
            get
            {
                List<object> l_TempList = new();
                if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                {
                    l_TempList.Add("Undefined");
                    return l_TempList;
                }
                foreach (string l_Current in GetGuildsName())
                    l_TempList.Add(l_Current);
                return l_TempList;
            }
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        [UIValue("SelectedGuild")]
        public string DropdownSelectedGuild
        {
            get => string.Empty;
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        [UIValue("UnPassedMaps")]
        private bool DlUnPassedMaps
        {
            get => m_OnlyUnPassedMaps;
            set
            {
                m_OnlyUnPassedMaps = value;
                if (GuildSaberModule.IsStateError())
                    return;
                SetLoadingMode(LoadingMode.Loading);
                e_OnUnPassedOnlyValueChanged?.Invoke(m_OnlyUnPassedMaps);
                RefreshFromUnPassed();
            }
        }

        [UIValue("UwU")]
        private bool UwUMode
        {
            get => GSConfig.Instance.UwUMode;
            set => GSConfig.Instance.UwUMode = value;
        }

        public void RefreshFromUnPassed()
        {
            RefreshList();
            SetLoadingMode(LoadingMode.Normal);
        }
    }
}

public enum PlaylistsVerificationType
{
    FolderOnly,
    PlaylistsOnly,
    All
}

public enum LoadingMode
{
    Normal,
    Loading
}
