using UnityEngine;
using UnityEngine.UI;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using System.Collections.Generic;
using System.Net;
using System.IO;
using GuildSaberProfile.Configuration;
using System.Threading.Tasks;
using System.Threading;
using HMUI;
using BeatSaberMarkupLanguage;

namespace GuildSaberProfile.UI.GuildSaber
{
    public class CategoryUI
    {
        [UIComponent("DownloadBut")] Button m_DownloadButton = null;

        public string CategoryName { get; set; } = Plugin.NOT_DEFINED;

        public int PlaylistsCountInCategory = 0;

        public int CurrentPlaylistIndex = 0;

        public string m_CategoryDirectory = Plugin.NOT_DEFINED;

        public string m_GuildName = "CS";

        public List<int> m_ValidPlaylists = new List<int>() { 1, 2, 3};

        public CategoryUI(string p_Name, string p_GuildName)
        {
            CategoryName = p_Name;
            m_GuildName = p_GuildName;
            PlaylistsCountInCategory = m_ValidPlaylists.Count;

            m_CategoryDirectory = $"IPA/Pending/Playlists/GuildSaber/{m_GuildName}/{CategoryName}";
        }

        [UIAction("DownloadPlaylist")]
        private async void DownloadPlaylist()
        {
            if (CategoryName == Plugin.NOT_DEFINED || m_ValidPlaylists.Count == 0) return;

            if (CurrentPlaylistIndex == PlaylistsCountInCategory)
            {
                m_DownloadButton.SetButtonText("Re download");
                return;
            }

            if (!Directory.Exists(m_CategoryDirectory))
                Directory.CreateDirectory(m_CategoryDirectory);

            using (WebClient l_Client = new WebClient())
            {
                m_DownloadButton.SetButtonText("Downloading");
                l_Client.DownloadFileAsync(
                    (CategoryName != "Main") ?
                    new System.Uri($"http://api.bsdr.fdom.eu/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}/{CategoryName}") :
                    new System.Uri($"https://api.jupilian.me/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}/{CategoryName}")
                    , $"{m_CategoryDirectory}/{CategoryName}_{CurrentPlaylistIndex}.bplist");
                l_Client.DownloadFileCompleted += (p_Sender, p_Event) =>
                {
                    if (p_Event.Error == null)
                    {
                        CurrentPlaylistIndex += 1;
                        DownloadPlaylist();
                    } else
                    {
                        m_DownloadButton.SetButtonText("Error during downloading playlists");
                    }
                };
            }
        }
    }

    [HotReload(RelativePathToLayout = @"ModViewController.bsml")]
    [ViewDefinition("GuildSaberProfile.UI.GuildSaber.View.ModViewController.bsml")]
    class ModViewController : BSMLAutomaticViewController
    {
//        ModFlowCoordinator m_ParentFlowCoordinator = null;

        public string GuildName = PluginConfig.Instance.SelectedGuild;

        public List<GuildCategorys> m_Guilds = new List<GuildCategorys>() {
            new GuildCategorys("CS", new List<string>() { "Tech", "Vibro", "Streams", "Jumps"}),
            new GuildCategorys("BSCC", new List<string>() {"Main"})
        };

        [UIComponent("CategoryList")]
        public CustomCellListTableData m_CategoriesTableList = null;

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
            } }

        public CategoryUI CategoryInterface;

        [UIValue("SelectedGuild")]
        public string SelectedGuild
        {
            get => GuildName;
            set { }
        }

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

        [UIAction("#post-parse")]
        protected void PostParse()
        {

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
