using System.Threading.Tasks;
using GuildSaber.BSPModule;
using GuildSaber.UI.Leaderboard.Components;
using GuildSaber.Utils;
using HMUI;
using TMPro;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GuildSaber.UI.Leaderboard
{
    public class LeaderboardHeaderManager
    {
        public static LeaderboardHeaderManager m_Instance = null;

        public const string HEADER_PANEL_PATH = "RightScreen.PlatformLeaderboardViewController.HeaderPanel";

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public static GameObject m_Header = null;
        public static ImageView m_HeaderImageView = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public static Color m_Color0 = new Color(1, 0.5f, 0, 3);
        public static readonly Color s_Color1 = new Color(0.2f, 0.1f, 1, 0.75f);

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        static UpdateView s_UpdatesModal = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Make manager get all required components
        /// </summary>
        private static async Task<bool> GetPanel()
        {
            if (!GuildSaberCustomLeaderboard.Initialized) return false;

            GameObject l_Current = null;
            bool l_MoveNext = false;
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (true)
            {
                l_Current = GuildSaberUtils.FindGm(HEADER_PANEL_PATH) ?? null;
                await WaitUtils.Wait(() =>
                {
                    l_MoveNext = l_Current != null;
                    if (l_MoveNext)
                        l_MoveNext = l_Current.gameObject.activeSelf;
                    return true;
                }, 10, 0, 10);
                if (l_MoveNext) break;

                return false;
            }

            if (l_Current != null)
            {
                m_Header = l_Current;
                m_HeaderImageView = m_Header.GetComponentInChildren<ImageView>();
            }
            else
                return false;

            return true;
        }

        public static async void CreateUpdateView()
        {
            await WaitUtils.Wait(() => m_Header != null, 10, p_CodeLine: 64);
            s_UpdatesModal = CustomUIComponent.CreateItem<UpdateView>(m_Header.transform, true, true);
            s_UpdatesModal.CheckUpdates();
        }

        public static async void SetColors(Color p_Color0, Color p_Color1)
        {
            if (!GuildSaberCustomLeaderboard.Initialized) return;

            await WaitUtils.Wait(() => m_Header != null, 100);

            m_HeaderImageView.color0 = p_Color0;
            m_HeaderImageView.color1 = p_Color1;
        }

        public static void ResetColors()
        {
            if (!GuildSaberCustomLeaderboard.Initialized || m_HeaderImageView == null || GuildSaberModule.ModState == GuildSaberModule.EModState.APIError) return;

            m_HeaderImageView.color0 = Color.gray;
            m_HeaderImageView.color1 = Color.clear;
        }

        public static async void ChangeText(string p_Text)
        {
            if (!GuildSaberCustomLeaderboard.Initialized) return;
            if (!await GetPanel()) return;

            await WaitUtils.Wait(() => m_Header != null, 100);

            TextMeshProUGUI l_Text = m_Header.GetComponentInChildren<TextMeshProUGUI>();
            if (l_Text)
                l_Text.text = p_Text;
        }

        public static async void ChangeTextForced(string p_Text, bool p_ChangeReallyForce = false)
        {
            if (!p_ChangeReallyForce)
                if (!GuildSaberCustomLeaderboard.Initialized)
                    return;
            if (GuildSaberModule.IsStateError())
                return;

            if (!await GetPanel())
                return;
            TextMeshProUGUI l_Text = m_Header.GetComponentInChildren<TextMeshProUGUI>();
            if (l_Text)
                l_Text.text = p_Text;
        }

    }
}
