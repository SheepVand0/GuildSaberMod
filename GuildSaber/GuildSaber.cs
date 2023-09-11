// Decompiled with JetBrains decompiler
// Type: GuildSaber.GuildSaberModule
// Assembly: GuildSaber, Version=0.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E8F8B5B-092B-47A3-9F65-4C90A48B7328
// Assembly location: C:\Users\user\Desktop\GuildSaber.dll

using BeatSaberMarkupLanguage;
using BeatSaberPlus.SDK;
using CP_SDK;
using CP_SDK.Config;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.Configuration;
using GuildSaber.UI.BeatSaberPlusSettings;
using GuildSaber.UI.Card;
using GuildSaber.UI.Defaults;
using GuildSaber.UI.FlowCoordinator;
using GuildSaber.UI.Leaderboard;
using GuildSaber.Utils;
using HarmonyLib;
using HMUI;
using IPA.Loader;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


#nullable enable
namespace GuildSaber
{
    public class GuildSaberModule : BSPModuleBase<GuildSaberModule>
    {
        public const int SCORES_BY_PAGE = 10;
        public static bool Restarting = false;
        public static bool s_BeatLeaderInstalled;
        private Settings m_SettingsView;

        public override EIModuleBaseType Type => EIModuleBaseType.Integrated;

        public override string Name => "GuildSaber";

        public override string Description => "Allows you to see your Guilds profile in game, see a leaderboard and play Guilds map on a custom play view";

        public override bool UseChatFeatures => false;

        public override bool IsEnabled
        {
            get => JsonConfig<GSConfig>.Instance.Enabled;
            set
            {
                JsonConfig<GSConfig>.Instance.Enabled = value;
                if (!value)
                    return;
                EnableLeader();
            }
        }

        public override EIModuleBaseActivationType ActivationType => EIModuleBaseActivationType.OnMenuSceneLoaded;

        public static int? GSPlayerId { get; internal set; } = new int?();

        public static ulong SsPlayerId { get; internal set; } = 0;

        public static GuildData CardSelectedGuild { get; internal set; } = new GuildData();

        public static GuildData LeaderboardSelectedGuild { get; internal set; } = new GuildData();

        public static List<GuildData> AvailableGuilds { get; internal set; } = new List<GuildData>();

        public static GuildSaberModule.EModState ModState { get; internal set; } = GuildSaberModule.EModState.APIError;

        public static GuildSaberModule.EModErrorState ModErrorState { get; internal set; } = GuildSaberModule.EModErrorState.NoError;

        public static HarmonyLib.Harmony HarmonyInstance => new("SheepVand.BeatSaber.GuildSaber");

        protected override (ViewController?, ViewController?, ViewController?) GetSettingsUIImplementation()
        {
            if (m_SettingsView == null)
                m_SettingsView = BeatSaberUI.CreateViewController<Settings>();
            return (m_SettingsView, null, null);
        }

        private async void EnableLeader()
        {
            Task task = await WaitUtils.Wait(() =>GameObject.Find("LeaderboardNavigationButtonsPanel") != null, 100);
            GuildSaberCustomLeaderboard.Instance.Initialize();
        }

        private static bool m_PatchedPlayingButtonsPanel = false;

        internal static bool IsInCustomPlayMenu = false;

        internal static ModFlowCoordinator m_ModFlowCoordinator;

        private static void ShowGuildFlow()
        {
            if (m_ModFlowCoordinator == null)
                m_ModFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<ModFlowCoordinator>();
            m_ModFlowCoordinator.Show();
        }

        internal static async void PatchPlayingButtonsPanel(UnityEngine.SceneManagement.Scene p_Old, UnityEngine.SceneManagement.Scene p_New)
        {
            if (p_New.name == "MainMenu" && !m_PatchedPlayingButtonsPanel)
            {
                UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= PatchPlayingButtonsPanel;
                Transform l_SoloButtonParent = null;
                await WaitUtils.Wait(() =>
                {
                    GameObject l_SoloButton = GameObject.Find("SoloButton");
                    if (l_SoloButton == null) return false;
                    l_SoloButtonParent = l_SoloButton.transform.parent;
                    return true;
                }, 10, p_MaxTryCount: 5000);

                XUISecondaryButton l_Button = null;
                UI.Defaults.GSSecondaryButton.Make  ("<i>Guild Saber", /*27, 48*/27, 48, p_OnClick: new Action(ShowGuildFlow)).Bind(ref l_Button).BuildUI(l_SoloButtonParent);
                //l_Button.Element.ButtonC.GetComponent<TextMeshProUGUI>();
                m_PatchedPlayingButtonsPanel = true;
            }
        }

        protected override async void OnEnable()
        {
            PluginMetadata l_BeatLeaderPlugin = PluginManager.GetPluginFromId("BeatLeader");
            s_BeatLeaderInstalled = l_BeatLeaderPlugin != null;
            AvailableGuilds = (await GuildApi.GetPlayerGuildsInfo()).AvailableGuilds;

            if (PlayerCardUI.m_Instance == null && JsonConfig<GSConfig>.Instance.CardEnabled && GuildSaberModule.ModState == GuildSaberModule.EModState.Functional)
            {
                _ = await PlayerCardUI.CreateCard();
            }

            if (PlayerCardUI.m_Instance != null && ModState == EModState.Functional)
                PlayerCardUI.SetCardActive(GSConfig.Instance.CardEnabled);

            Events.m_EventsEnabled = true;
        }

        protected override void OnDisable()
        {
            Events.m_EventsEnabled = false;
            PlayerCardUI.DestroyCard();
            GuildSaberCustomLeaderboard.Instance.Dispose();
            GuildSaberModule.HarmonyInstance.UnpatchSelf();
        }

        public static bool IsStateError() => ModState == EModState.APIError;

        internal static void SetState(EModState p_State) => ModState = p_State;

        internal static void SetErrorState(Exception p_E) => ModErrorState = p_E.Message.Contains("400") ? GuildSaberModule.EModErrorState.BadRequest_400 : GuildSaberModule.EModErrorState.NotFound_404;

        public enum EModErrorState
        {
            NoError = 0,
            BadRequest_400 = 2,
            NotFound_404 = 4,
        }

        public enum EModState
        {
            APIError = 0,
            Functional = 2,
        }
    }
}
