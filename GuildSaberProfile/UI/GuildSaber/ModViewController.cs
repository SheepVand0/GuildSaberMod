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
using GuildSaberProfile.Configuration;
using HMUI;
using IPA.Utilities;
using Newtonsoft.Json;
using SongCore;
using UnityEngine;
using UnityEngine.UI;

namespace GuildSaberProfile.UI.GuildSaber
{

    public sealed class CategoryUI
    {

        public int CurrentPlaylistIndex;

        public bool DownloadOnlyUnPassed;

        public string m_CategoryDirectory = Plugin.NOT_DEFINED;
        [UIComponent("DownloadBut")] private readonly Button m_DownloadButton = null;

        public string m_GuildName = "CS";
        [UIComponent("ElemsHorizontal")] private readonly HorizontalLayoutGroup m_HorizontalElems = null;

        public List<int> m_ValidPlaylists = new List<int>
            { 1, 2, 3 };

        public int PlaylistsCountInCategory;

        public CategoryUI(string p_Name, string p_GuildName)
        {
            CategoryName = p_Name;
            m_GuildName = p_GuildName;

            using (HttpClient l_Client = new HttpClient())
            {
                Task<string> l_SerializedObject = l_Client.GetStringAsync(m_GuildName == "CS" ? $"http://api.bsdr.fdom.eu/levelcache/{CategoryName}" : "https://api.jupilian.me/levelcache/");
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

        public string CategoryName { get; set; } = Plugin.NOT_DEFINED;

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

            Plugin._modFlowCoordinator._modViewController.e_OnUnPassedOnlyValueChanged += p_UnPassedOnly =>
            {
                Plugin.Log.Info($"Updating On Un Passed Only: {p_UnPassedOnly}");
                DownloadOnlyUnPassed = p_UnPassedOnly;
            };
        }
        [UIAction("DownloadPlaylist")] private async void DownloadPlaylist()
        {
            if (CategoryName == Plugin.NOT_DEFINED || m_ValidPlaylists.Count == 0) return;

            if (CurrentPlaylistIndex == PlaylistsCountInCategory)
            {
                m_DownloadButton.SetButtonText("Finished");
                m_DownloadButton.interactable = false;
                Loader.Instance.RefreshSongs();
                return;
            }

            CheckIfCategoryFolderExists();

            using (WebClient l_Client = new WebClient())
            {
                m_DownloadButton.SetButtonText("Downloading");
                m_DownloadButton.interactable = false;
                string l_QueryString = string.Empty;
                if (!DownloadOnlyUnPassed)
                {
                    l_QueryString = m_GuildName != "BSCC"
                        ? $"http://api.bsdr.fdom.eu/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}/{CategoryName}"
                        : $"https://api.jupilian.me/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}";
                }
                else
                    l_QueryString = m_GuildName != "BSCC"
                        ? $"http://api.bsdr.fdom.eu/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}/{CategoryName}/{Plugin.m_PlayerId}"
                        : $"https://api.jupilian.me/playlist/{m_ValidPlaylists[CurrentPlaylistIndex]}/{Plugin.m_PlayerId}";

                Plugin.Log.Info(l_QueryString);
                l_Client.DownloadFileAsync(new Uri(l_QueryString), $"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex]:D3}_{m_GuildName}_{CategoryName}.bplist");

                l_Client.DownloadFileCompleted += (p_Sender, p_Event) =>
                {
                    if (p_Event.Error == null)
                    {
                        CurrentPlaylistIndex += 1;
                        DownloadPlaylist();
                    }
                    else
                    {
                        if (DownloadOnlyUnPassed)
                        {
                            if (File.Exists($"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex]:D3}_{m_GuildName}_{CategoryName}.bplist"))
                            {
                                File.Delete($"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex]:D3}_{m_GuildName}_{CategoryName}.bplist");
                            }

                            CurrentPlaylistIndex += 1;
                            DownloadPlaylist();
                        }
                        else
                        {
                            m_DownloadButton.SetButtonText("Error");
                            m_DownloadButton.interactable = false;
                        }
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

            if (File.Exists($"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex]:D3}_{m_GuildName}_{CategoryName}.bplist"))
            {
                File.Delete($"{m_CategoryDirectory}/{m_ValidPlaylists[CurrentPlaylistIndex]:D3}_{m_GuildName}_{CategoryName}.bplist");
            }
        }
    }

    [HotReload(RelativePathToLayout = @"ModViewController.bsml")]
    [ViewDefinition("GuildSaberProfile.UI.GuildSaber.View.ModViewController.bsml")]
    public class ModViewController : BSMLAutomaticViewController
    {

        public delegate void OnOnlyPassedMapsChange(bool p_OnlyUnPassed);

        public string GuildName = PluginConfig.Instance.SelectedGuild;

        public bool m_OnlyUnPassedMaps;

        [UIComponent("CategoryList")]
        public CustomCellListTableData m_CategoriesTableList;

        public CategoryUI CategoryInterface;

        public List<GuildCategories> m_Guilds = new List<GuildCategories>
        {
            new GuildCategories("CS", new List<string>
                { "Tech", "Vibro", "Streams", "Jumps", "Shitpost" }),
            new GuildCategories("BSCC", new List<string>
                { "Main" })
        };

        public void Awake()
        {
            RefreshList();
        }

        public event OnOnlyPassedMapsChange e_OnUnPassedOnlyValueChanged;

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
            GuildCategories l_CurrentGuild = GetGuildFromName(m_Guilds, GuildName);
            Plugin.Log.Info(l_CurrentGuild.GuildName);
            foreach (string l_Current in l_CurrentGuild.Categories)
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

        public GuildCategories GetGuildFromName(List<GuildCategories> p_Guilds, string p_Name)
        {
            foreach (GuildCategories l_Current in p_Guilds)
            {
                if (l_Current.GuildName == p_Name)
                    return l_Current;
            }

            return new GuildCategories(Plugin.NOT_DEFINED, new List<string>
                { Plugin.NOT_DEFINED });
        }

        public List<string> GetGuildsName()
        {

            List<string> l_Temp = new List<string>();
            foreach (GuildCategories l_Current in m_Guilds)
            {
                l_Temp.Add(l_Current.GuildName);
            }
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

        [UIValue("UnPassedMaps")]
        private bool DlUnPassedMaps
        {
            get => m_OnlyUnPassedMaps;
            set
            {
                m_OnlyUnPassedMaps = value;
                e_OnUnPassedOnlyValueChanged?.Invoke(m_OnlyUnPassedMaps);
            }
        }

        #endregion

    }
}

public class LevelIDs
{
    public List<int> LevelID { get; set; }
}
