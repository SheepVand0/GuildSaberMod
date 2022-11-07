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

namespace GuildSaber.BSPModule
{
    internal class GuildSaberModule : BSPModuleBase<GuildSaberModule>
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
        public static int? m_GSPlayerId = null;
        public static ulong m_SSPlayerId = 0;

        public static GuildData m_CardSelectedGuild = default(GuildData);
        public static GuildData m_PlaylistDownloadSelectedGuild = default(GuildData);
        public static GuildData m_LeaderboardSelectedGuild = default(GuildData);
        public static GuildData m_GuildSaberPlayingMenuSelectedGuild = default(GuildData);
        public static GuildData m_LevelSelectionMenuSelectedGuild = default(GuildData);

        public static List<GuildData> AvailableGuilds = new List<GuildData>();

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

            if (PlayerCardUI.m_Instance == null && GSConfig.Instance.CardEnabled)
                PlayerCardUI.CreateCard();

            if (PlayerCardUI.m_Instance != null)
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
    }
}
