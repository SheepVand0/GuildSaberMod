using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberPlus.SDK;
using BeatSaberPlus.SDK.Game;
using CP_SDK;
using GuildSaber.API;
using GuildSaber.Configuration;
using GuildSaber.UI.BeatSaberPlusSettings;
using GuildSaber.UI.Card;
using GuildSaber.UI.Leaderboard;
using GuildSaber.Utils;
using HMUI;
using UnityEngine;

namespace GuildSaber.BSPModule
{
    public class GuildSaberModule : BSPModuleBase<GuildSaberModule>
    {
        public override EIModuleBaseType Type => EIModuleBaseType.Integrated;

        public override string Name => "GuildSaber";

        public override string Description => "Allows you to see your Guilds profile in game, see a leaderboard and play Guilds map on a custom play view";

        public override bool UseChatFeatures => false;

        public override bool IsEnabled
        {
            get => GSConfig.Instance.Enabled;
            set
            {
                GSConfig.Instance.Enabled = value;
                if (value)
                {
                    EnableLeader();
                }
            }
        }

        public override EIModuleBaseActivationType ActivationType => EIModuleBaseActivationType.OnMenuSceneLoaded;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

#nullable enable
        public static int? GSPlayerId { get; internal set; } = null;
        public static ulong SsPlayerId { get; internal set; } = 0;
        public static GuildData CardSelectedGuild { get; internal set; } = default(GuildData);
        public static GuildData LeaderboardSelectedGuild { get; internal set; } = default(GuildData);
        public static List<GuildData> AvailableGuilds { get; internal set; } = new List<GuildData>();
        public static EModState ModState { get; internal set; } = EModState.APIError;
        public static EModErrorState ModErrorState { get; internal set; } = EModErrorState.NoError;
        public static bool Restarting = false;
        public static HarmonyLib.Harmony HarmonyInstance => new HarmonyLib.Harmony("SheepVand.BeatSaber.GuildSaber");
        public static bool s_BeatLeaderInstalled = false;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public const int SCORES_BY_PAGE = 10;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        Settings m_SettingsView;

        protected override (ViewController?, ViewController?, ViewController?) GetSettingsUIImplementation()
        {
            if (m_SettingsView == null)
                m_SettingsView = BeatSaberUI.CreateViewController<Settings>();

            return (m_SettingsView, null, null);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private async void EnableLeader()
        {
            await WaitUtils.Wait(() => GameObject.Find("LeaderboardNavigationButtonsPanel") != null, 100);
            GuildSaberCustomLeaderboard.Instance.Initialize();
        }

        protected override async void OnEnable()
        {
            var l_BeatLeaderPlugin = IPA.Loader.PluginManager.GetPluginFromId("BeatLeader");
            s_BeatLeaderInstalled = l_BeatLeaderPlugin != null;

            AvailableGuilds = (await GuildApi.GetPlayerGuildsInfo()).AvailableGuilds;

            if (PlayerCardUI.m_Instance == null && GSConfig.Instance.CardEnabled && ModState == EModState.Functional)
                await PlayerCardUI.CreateCard();

            if (PlayerCardUI.m_Instance != null && ModState == EModState.Functional)
                PlayerCardUI.SetCardActive(GSConfig.Instance.CardEnabled);

            Events.m_EventsEnabled = true;
        }

        protected override void OnDisable()
        {
            Events.m_EventsEnabled = false;

            PlayerCardUI.DestroyCard();
            GuildSaberCustomLeaderboard.Instance.Dispose();

            HarmonyInstance.UnpatchSelf();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public static bool IsStateError()
        {
            return ModState == EModState.APIError;
        }

        internal static void SetState(EModState p_State)
        {
            ModState = p_State;
        }

        internal static void SetErrorState(Exception p_E)
        {
            ModErrorState = p_E.Message.Contains("400") ? EModErrorState.BadRequest_400 : EModErrorState.NotFound_404;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public enum EModState
        {
            APIError = 0,
            Functional = 1 << 1
        }

        public enum EModErrorState
        {
            NoError = 0,
            BadRequest_400 = 1 << 1,
            NotFound_404 = 1 << 2
        }
    }
}
