﻿using BeatSaberPlus.SDK;
using CP_SDK;
using HMUI;
using GuildSaberProfile.Configuration;
using GuildSaberProfile.UI.Card;
using System.Runtime.Remoting.Messaging;
using System.Net.Http;

namespace GuildSaberProfile
{
    internal class GuildSaber : BSPModuleBase<GuildSaber>
    {
        public override EIModuleBaseType Type => EIModuleBaseType.Integrated;

        public override string Name => "GuildSaber";

        public override string Description => "Allows you to see your Guilds profile in game, see a leaderboard and play Guilds map on a custom play view";

        public override bool UseChatFeatures => false;

        public override bool IsEnabled { get => GSConfig.Instance.Enabled; set => GSConfig.Instance.Enabled = value; }

        public override EIModuleBaseActivationType ActivationType => EIModuleBaseActivationType.OnMenuSceneLoaded;

        #nullable enable
        public static int? m_GSPlayerId = null;
        public static long m_SSPlayerId = 0;
        protected override (ViewController?, ViewController?, ViewController?) GetSettingsUIImplementation()
        {
            return (null, null, null);
        }

        protected override void OnEnable()
        {
            if (PlayerCardUI.m_Instance == null)
                PlayerCardUI.CreateCard();
        }

        protected override void OnDisable()
        {
            PlayerCardUI.DestroyCard();
        }
    }
}
