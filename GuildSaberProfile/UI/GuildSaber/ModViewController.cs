using UnityEngine;
using UnityEngine.UI;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.IO;
using GuildSaberProfile.Configuration;
using GuildSaberProfile.API;
using System.Threading.Tasks;
using System.Threading;
using HMUI;
using BeatSaberMarkupLanguage;
using IPA.Utilities;
using Newtonsoft.Json;

namespace GuildSaberProfile.UI.GuildSaber
{

    public sealed class CategoryUI
    {
        [UIComponent("DownloadBut")] Button m_DownloadButton = null;
        [UIComponent("ElemsHorizontal")] HorizontalLayoutGroup m_HorizontalElems = null;

        public string CategoryName { get; set; } = Plugin.NOT_DEFINED;

        public int PlaylistsCountInCategory = 0;

        public int CurrentPlaylistIndex = 0;

        public string m_CategoryDirectory = Plugin.NOT_DEFINED;

        public string m_GuildName = "CS";

        public List<int> m_ValidPlaylists = new List<int>() { 1, 2, 3 };

        public bool m_DownloadOnlyUnpassed = false;

        public CategoryUI(string p_Name, string p_GuildName)
        {
            CategoryName = p_Name;
            m_GuildName = p_GuildName;

            using (HttpClient l_Client = new HttpClient())
            {
                Task<string> l_SerializedObject = l_Client.GetStringAsync((m_GuildName == "CS") ? $"http://api.bsdr.fdom.eu/levelcache/{CategoryName}" : $"https://api.jupilian.me/levelcache/");
                l_SerializedObject.Wait();
                LevelIDs l_TempValidLevels = JsonConvert.DeserializeObject<LevelIDs>(l_SerializedObject.Result);
                m_ValidPlaylists = l_TempValidLevels.LevelID;
            }

            PlaylistsCountInCategory = m_ValidPlaylists.Count;
            m_CategoryDirectory = $"./Playlists/GuildSaber/{m_GuildName}/{CategoryName}";

            if (PlaylistsCountInCategory == 0)
            {
                m_DownloadButton.interactable = false;
                m_DownloadButton.SetButtonText("Error");
            }
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            ImageView l_Background = m_HorizontalElems.GetComponent<ImageView>();

            l_Background.SetField("_skew", 0.0f);
            l_Background.SetImage("#RoundRect10BorderFade");

            l_Background.color = Color.white.ColorWithAlpha(0.5f);
            l_Background.color0 = Color.white.ColorWithAlpha(0.4f);
            l_Background.color1 = Color.white.ColorWithAlpha(0.4f);
            l_Background.overrideSprite = null;

            Plugin._modFlowCoordinator._modViewController.e_OnUnpassOnlyValueChanged += (p_UnpassOnly) =>
            {
                Plugin.Log.Info("Updating On Un Passe Only");
                m_DownloadOnlyUnpassed = p_UnpassOnly;
            };
        }
        [UIAction("DownloadPlaylist")] private async void DownloadPlaylist()
        {
            if (CategoryName == Plugin.NOT_DEFINED || m_ValidPlaylists.Count == 0) return;

            if (CurrentPlaylistIndex == PlaylistsCountInCategory)
            {
                m_DownloadButton.SetButtonText("Finished");
                m_DownloadButton.interactable = false;
                SongCore.Loader.Instance.RefreshSongs(true);
                return;
            }

            CheckIfCategoryFolderExists();

            using (WebClient l_Client = new WebClient())
            {
                m_DownloadButton.SetButtonText("Downloading");
                m_DownloadButton.interactable = false;
                if (!m_DownloadOnlyUnpassed)
                    l_Client.DownloadFileAsync(
                        (m_GuildName != "BSCC") ?
                        new System.Uri($"http://api.bsdr.fdom.eu/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}/{CategoryName}") :
                        new System.Uri($"https://api.jupilian.me/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}")
                        , $"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex]:D3}_{m_GuildName}_{CategoryName}.bplist");
                else
                    l_Client.DownloadFileAsync(
                        (m_GuildName != "BSCC") ?
                        new System.Uri($"http://api.bsdr.fdom.eu/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}/{CategoryName}/{Plugin.m_PlayerId}") :
                        new System.Uri($"https://api.jupilian.me/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}/{Plugin.m_PlayerId}")
                        , $"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex]:D3}_{m_GuildName}_{CategoryName}.bplist");

                Plugin.Log.Info($"https://api.jupilian.me/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}");
                l_Client.DownloadFileCompleted += (p_Sender, p_Event) =>
                {
                    if (p_Event.Error == null)
                    {
                        CurrentPlaylistIndex += 1;
                        DownloadPlaylist();
                    } else
                    {
                        m_DownloadButton.SetButtonText("Error");
                        m_DownloadButton.interactable = false;
                    }
                };
            }
        }

        private void CheckIfCategoryFolderExists()
        {
            if (!Directory.Exists(m_CategoryDirectory))
            {
                Directory.CreateDirectory(m_CategoryDirectory);
            }

            if (File.Exists($"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex].ToString("000")}/{CategoryName}"))
            {
                File.Delete($"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex].ToString("000")}/{CategoryName}");
            }
        }
    }

    [HotReload(RelativePathToLayout = @"ModViewController.bsml")]
    [ViewDefinition("GuildSaberProfile.UI.GuildSaber.View.ModViewController.bsml")]
    public class ModViewController : BSMLAutomaticViewController
    {
        public string GuildName = PluginConfig.Instance.SelectedGuild;

        public List<GuildCategorys> m_Guilds = new List<GuildCategorys>() {
            new GuildCategorys("CS", new List<string>() { "Tech", "Vibro", "Streams", "Jumps", "Shitpost"}),
            new GuildCategorys("BSCC", new List<string>() { "Main" })
        };

        public bool OnlyUnpassedMaps = false;

        public delegate void OnOnlyPassedMapsChange(bool p_OnlyUnpassed);

        public event OnOnlyPassedMapsChange e_OnUnpassOnlyValueChanged;

        public CategoryUI CategoryInterface;

        [UIComponent("CategoryList")]
        public CustomCellListTableData m_CategoriesTableList = null;

        #region UIValues
        [UIValue("AvailableCategories")]
        public List<object> m_ListCategories = new List<object>() { };

        [UIValue("AvailableGuilds")]
        public List<object> m_AvailableGuilds { get {
                List<object> l_TempList = new List<object>();
                foreach (string l_Current in GetGuildsName())
                {
                    l_TempList.Add(l_Current);
                }
                return l_TempList;
            }
        }

        [UIValue("SelectedGuild")]
        public string SelectedGuild
        {
            get => GuildName;
            set { }
        }

        [UIValue("UnpassedMaps")]
        private bool DlUnpassedMaps
        {
            get => OnlyUnpassedMaps;
            set
            {
                OnlyUnpassedMaps = value;
                e_OnUnpassOnlyValueChanged?.Invoke(OnlyUnpassedMaps);
            }
        }
        #endregion

        public void Awake()
        {
            RefreshList();
        }

        [UIAction("OnGuildChange")]
        public void OnGuildChange(string p_Selected)
        {
            SelectedGuild = p_Selected;
            GuildName = p_Selected;
            RefreshList();
        }

        public void RefreshList()
        {
            m_ListCategories.Clear();
            GuildCategorys l_CurrentGuild = GetGuildFromName(m_Guilds, this.GuildName);
            Plugin.Log.Info(l_CurrentGuild.GuildName);
            foreach (string l_Current in l_CurrentGuild.Categorys)
            {
                m_ListCategories.Add(new CategoryUI(l_Current, GuildName));
            }
            if (m_CategoriesTableList != null)
                m_CategoriesTableList.tableView.ReloadData();
        }

        public void Init(string p_GuildName, ModFlowCoordinator p_ParentFlowCoordinator)
        {
            GuildName = p_GuildName;
            //m_ParentFlowCoordinator = p_ParentFlowCoordinator;
        }

        public GuildCategorys GetGuildFromName(List<GuildCategorys> p_Guilds, string p_Name)
        {
            foreach (GuildCategorys l_Current in p_Guilds)
            {
                if (l_Current.GuildName == p_Name)
                    return l_Current;
            }

            return new GuildCategorys(Plugin.NOT_DEFINED, new List<string>() { Plugin.NOT_DEFINED });
        }

        public List<string> GetGuildsName() {

            List<string> l_Temp = new List<string>();
            foreach (GuildCategorys l_Current in m_Guilds)
            {
                l_Temp.Add(l_Current.GuildName);
            }
            return l_Temp;
        }
    }
}

public class LevelIDs
{
    public List<int> LevelID { get; set; }
}