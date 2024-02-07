using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberPlus.SDK.UI;
using CP_SDK.Config;
using GuildSaber.API;
using GuildSaber.Configuration;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


#nullable enable
namespace GuildSaber.UI
{
    public class PlaylistViewController : ViewController<PlaylistViewController>
    {
        public static PlaylistViewController PlaylistsDownloadViewControllerInstance;
        public bool m_OnlyUnPassedMaps;
        public int m_GuildId = JsonConfig<GSConfig>.Instance.SelectedGuild;

        ///////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        [UIComponent("CategoryList")] public CustomCellListTableData m_CategoriesTableList;
        [UIComponent("Dropdown")] public BeatSaberMarkupLanguage.Components.Settings.DropDownListSetting m_GuildChoiceDropdown;
        [UIComponent("LoadingLayout")] public GridLayoutGroup m_LoadingGrid;
        [UIComponent("Elems")] public VerticalLayoutGroup m_ElemsLayout;
        [UIObject("BG")] private readonly GameObject m_BG = (GameObject)null;
        [UIValue("AvailableCategories")] public List<object> m_ListCategories = new List<object>();

        ///////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        [UIValue("AvailableGuilds")]
        public List<object> AvailableGuilds
        {
            get
            {
                List<object> availableGuilds = new List<object>();
                if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                {
                    availableGuilds.Add((object)"Undefined");
                    return availableGuilds;
                }
                foreach (string str in GetGuildsName())
                    availableGuilds.Add((object)str);
                if (availableGuilds.Count == 0)
                    availableGuilds.Add((object)"Undefined");
                return availableGuilds;
            }
            set
            {
            }
        }

        [UIValue("SelectedGuild")]
        public string DropdownSelectedGuild
        {
            get => string.Empty;
            set
            {
            }
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
                PlaylistViewController.OnOnlyPassedMapsChange onlyValueChanged = e_OnUnPassedOnlyValueChanged;
                if (onlyValueChanged != null)
                    onlyValueChanged(m_OnlyUnPassedMaps);
                RefreshFromUnPassed();
            }
        }

        [UIValue("UwU")]
        private bool UwUMode
        {
            get => JsonConfig<GSConfig>.Instance.UwUMode;
            set => JsonConfig<GSConfig>.Instance.UwUMode = value;
        }

        //protected override string GetViewContentDescription() => Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "GuildSaber.UI.GuildSaber.View.ModViewController.bsml");

        public event PlaylistViewController.OnOnlyPassedMapsChange e_OnUnPassedOnlyValueChanged;

        protected override void OnViewCreation()
        {
            //BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_BG, 0.5f);
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                return;
            UpdateCategories();
            PlaylistViewController.PlaylistsDownloadViewControllerInstance = this;
        }

        public void Init(int p_GuildId)
        {
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                return;
            m_GuildId = p_GuildId;
        }

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

        public void UpdateCategories()
        {
            SetLoadingMode(LoadingMode.Loading);
            if (GuildSaberModule.IsStateError())
            {
                ShowMessageModal("<color=#ff0000>Error on getting info</color>");
            }
            else
            {
                RefreshDropdown();
                RefreshList();
            }
        }

        public void RefreshDropdown()
        {
            if (GuildSaberModule.IsStateError())
                return;
            AvailableGuilds.Clear();
            List<string> stringList = new List<string>();
            foreach (GuildData availableGuild in GuildSaberModule.AvailableGuilds)
                stringList.Add(string.IsNullOrEmpty(availableGuild.SmallName) ? availableGuild.Name : availableGuild.SmallName);
            foreach (object obj in stringList)
                AvailableGuilds.Add(obj);
            m_GuildChoiceDropdown.UpdateChoices();
            DropdownSelectedGuild = (string)m_GuildChoiceDropdown.Value;
        }

        public void SetLoadingMode(LoadingMode p_Mode)
        {
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                return;
            (m_LoadingGrid).gameObject.SetActive(p_Mode == LoadingMode.Loading);
            (m_GuildChoiceDropdown).gameObject.SetActive(p_Mode == LoadingMode.Normal);
        }

        public async void RefreshList()
        {
            List<ApiCategory> l_Categories;
            if (GuildSaberModule.IsStateError())
                l_Categories = (List<ApiCategory>)null;
            else
            {
                foreach (CategoryUI l_Current in m_ListCategories)
                    l_Current.UnbindEvent();
                m_ListCategories.Clear();
                l_Categories = await GuildApi.GetCategoriesForGuild(m_GuildId);
                l_Categories.Add(new ApiCategory());
                foreach (ApiCategory apiCategory in l_Categories)
                {
                    ApiCategory l_Current = apiCategory;
                    CategoryUI l_CategoryUI = new CategoryUI(l_Current, m_GuildId, m_OnlyUnPassedMaps);
                    m_ListCategories.Add((object)l_CategoryUI);
                    l_CategoryUI = (CategoryUI)null;
                    l_Current = new ApiCategory();
                }
                if (m_CategoriesTableList != null)
                    m_CategoriesTableList.tableView.ReloadData();
                SetLoadingMode(LoadingMode.Normal);
                l_Categories = (List<ApiCategory>)null;
            }
        }

        private List<string> GetGuildsName()
        {
            if (GuildSaberModule.IsStateError())
                return new List<string>() { "Undefined" };
            List<string> guildsName = new List<string>();
            foreach (GuildData availableGuild in GuildSaberModule.AvailableGuilds)
                guildsName.Add(string.IsNullOrEmpty(availableGuild.SmallName) ? availableGuild.Name : availableGuild.SmallName);
            return guildsName;
        }

        public void RefreshFromUnPassed()
        {
            RefreshList();
            SetLoadingMode(LoadingMode.Normal);
        }

        public delegate void OnOnlyPassedMapsChange(bool p_OnlyUnPassed);

        public enum LoadingMode
        {
            Normal, Loading
        }
    }
}
