using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage;
using TMPro;
using GuildSaberProfile.UI.Card;
using UnityEngine.UI;
using UnityEngine.U2D;

namespace GuildSaberProfile.UI.GuildSaber.Leaderboard
{
    [HotReload(RelativePathToLayout = @"LeaderboardPanel.bsml")]
    [ViewDefinition("GuildSaberProfile.UI.GuildSaber.View.LeaderboardPanel.bsml")]
    class GuildSaberLeaderboardPanel : BSMLAutomaticViewController
    {
        [UIComponent("PlayerErrorTxt")] private TextMeshProUGUI m_ErrorText = null;
        [UIComponent("Elems")] HorizontalLayoutGroup m_ElemsLayout = null;
        [UIComponent("LoadingGrid")] GridLayoutGroup m_LoadingLayout = null;
        [UIComponent("PlayerName")] TextMeshProUGUI m_PlayerName = null;
        [UIComponent("PPImage")] Image m_PlayerImage = null;
        [UIComponent("GSImage")] Image m_GSImage = null;

        PlayerGuildsInfo m_PlayerGuildsInfo = new PlayerGuildsInfo();

        [UIAction("#post-parse")]
        private void PostParse()
        {
            Reload(ReloadMode.FromCurrent, false);
        }

        public void Reload(ReloadMode p_ReloadMode, bool p_SetLoadingModeBeforeGettingData)
        {
            if (p_SetLoadingModeBeforeGettingData)
                SetLeaderboardPanelViewMode(LeaderboardPanelViewMode.Loading);

            switch (p_ReloadMode) {
                case ReloadMode.FromCurrent:
                    m_PlayerGuildsInfo = Plugin.GetPlayerInfoFromCurrent();
                    break;
                case ReloadMode.FromApi:
                    m_PlayerGuildsInfo = Plugin.GetPlayerInfoFromAPI();
                    break;
                default: return;
            }

            if (string.IsNullOrEmpty(m_PlayerGuildsInfo.m_ReturnPlayer.Name))
            {
                SetLeaderboardPanelViewMode(LeaderboardPanelViewMode.Error);
                if (p_ReloadMode == ReloadMode.FromCurrent)
                    Reload(ReloadMode.FromApi, true);
                else
                    return;
            }

            m_PlayerName.text = m_PlayerGuildsInfo.m_ReturnPlayer.Name;
            m_PlayerImage.SetImage(m_PlayerGuildsInfo.m_ReturnPlayer.ProfilePicture);

            SetLeaderboardPanelViewMode(LeaderboardPanelViewMode.Normal);
        }

        public void SetLeaderboardPanelViewMode(LeaderboardPanelViewMode p_ViewMode)
        {
            switch (p_ViewMode)
            {
                case LeaderboardPanelViewMode.Normal:
                    m_ElemsLayout.gameObject.SetActive(true);
                    m_ErrorText.gameObject.SetActive(false);
                    m_LoadingLayout.gameObject.SetActive(false);
                    break;
                case LeaderboardPanelViewMode.Loading:
                    m_ElemsLayout.gameObject.SetActive(false);
                    m_ErrorText.gameObject.SetActive(false);
                    m_LoadingLayout.gameObject.SetActive(true);
                    break;
                case LeaderboardPanelViewMode.Error:
                    m_ElemsLayout.gameObject.SetActive(false);
                    m_ErrorText.gameObject.SetActive(true);
                    m_LoadingLayout.gameObject.SetActive(false);
                    break;
                default: return;
            }
        }
    }

    public enum LeaderboardPanelViewMode
    {
        Normal,
        Loading,
        Error
    }

    public enum ReloadMode
    {
        FromCurrent,
        FromApi
    }
}
