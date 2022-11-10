using BeatSaberPlus.SDK;
using CP_SDK;
using HMUI;
using GuildSaber.Configuration;
using GuildSaber.UI.Card;
using System.Runtime.Remoting.Messaging;
using System.Net.Http;
using GuildSaber.API;
using UnityEngine;
using GuildSaber.UI.GuildSaber.Leaderboard;
using GuildSaber.UI.BeatSaberPlusSettings;
using BeatSaberMarkupLanguage;
using System.Collections.Generic;
using System.Security.Permissions;

namespace GuildSaber.BSPModule
{
    public class GuildSaberModule : BSPModuleBase<GuildSaberModule>
    {
        public override EIModuleBaseType Type => EIModuleBaseType.Integrated;

        public override string Name => "GuildSaber";

        public override string Description => "Allows you to see your Guilds profile in game, see a leaderboard and play Guilds map on a custom play view";

        public override bool UseChatFeatures => false;

        public override bool IsEnabled { get => GSConfig.Instance.Enabled; set => GSConfig.Instance.Enabled = value; }

        public override EIModuleBaseActivationType ActivationType => EIModuleBaseActivationType.OnMenuSceneLoaded;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        #nullable enable
        public static int? m_GSPlayerId { get; internal set; } = null;
        public static ulong m_SSPlayerId { get; internal set; } = 0;

        public static GuildData m_CardSelectedGuild { get; internal set; } = default(GuildData);
        public static GuildData m_PlaylistDownloadSelectedGuild { get; internal set; } = default(GuildData);
        public static GuildData m_LeaderboardSelectedGuild { get; internal set; } = default(GuildData);
        public static GuildData m_GuildSaberPlayingMenuSelectedGuild { get; internal set; } = default(GuildData);
        public static GuildData m_LevelSelectionMenuSelectedGuild { get; internal set; } = default(GuildData);

        public static List<GuildData> AvailableGuilds { get; internal set; } = new List<GuildData>();

        public static EModState ModState { get; internal set; } = EModState.Fonctionnal;
        public static EModErrorState ModErrorState { get; internal set; } = EModErrorState.NoError;

        public static HarmonyLib.Harmony m_HarmonyInstance { get => new HarmonyLib.Harmony("SheepVand.BeatSaber.GuildSaber"); }

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

        protected override void OnEnable()
        {
            AvailableGuilds = GuildApi.GetPlayerGuildsInfo().m_AvailableGuilds;

            if (PlayerCardUI.m_Instance == null && GSConfig.Instance.CardEnabled && ModState == EModState.Fonctionnal)
                PlayerCardUI.CreateCard();

            if (PlayerCardUI.m_Instance != null && ModState == EModState.Fonctionnal)
                PlayerCardUI.SetCardActive(GSConfig.Instance.CardEnabled);

            Events.m_EventsEnabled = true;
        }

        protected override void OnDisable()
        {
            Events.m_EventsEnabled = false;

            PlayerCardUI.DestroyCard();
            GuildSaberCustomLeaderboard.Instance.Dispose();

            m_HarmonyInstance.UnpatchSelf();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public enum EModState
        {
            APIError = 0,
            Fonctionnal = 1 << 1
        }

        public enum EModErrorState
        {
            NoError = 0,
            BadRequest_400 = 1 << 1,
            NotFound_404 = 1 << 2
        }
    }
}
