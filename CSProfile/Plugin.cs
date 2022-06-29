using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ScoreSaberSharp;
using BS_Utils.Utilities;
using CSProfile.API;
using BSDiscordRanking.Formats.API;
using CSProfile.UI.Card;
using CSProfile.UI.Settings;
using BeatSaberMarkupLanguage.GameplaySetup;
using System.Threading.Tasks;
using IPALogger = IPA.Logging.Logger;
using HarmonyLib;

namespace CSProfile
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        public static string m_currentPlayerId = "";

        public static string m_notDefined = "Undefined";

        public static PlayerCard_UI m_playerCard = null;

        public SettingTabViewController m_tabViewController = new SettingTabViewController();

        public static bool m_cardLoaded = false;

        Harmony m_harmony = new Harmony("SheepVand.BeatSaber.CSProfile");

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Log.Info("CSProfile initialized.");


        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        
        [Init]
        public void InitWithConfig(IPA.Config.Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");

            m_harmony.PatchAll();
        }
        
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            new GameObject("CSProfileController").AddComponent<CSProfileController>();

            GameplaySetup.instance.AddTab("Player Card", "CSProfile.UI.Settings.SettingTabViewController.bsml", m_tabViewController);

            BSEvents.lateMenuSceneLoadedFresh += OnMenuSceneLoadedFresh;
        }

        private void OnMenuSceneLoadedFresh(ScenesTransitionSetupDataSO obj)
        {
            CreateCard();
        }

        public static void CreateCard()
        {
            if (m_cardLoaded == false)
            {
                var l_playerId = Authentification.GetPlayerIdFromSteam();
                if (l_playerId != m_notDefined)
                {
                    m_currentPlayerId = l_playerId;
                    PlayerApiReworkOutput l_outputPlayer = CSApi.GetPlayerByScoreSaberId(Plugin.m_currentPlayerId);

                    m_playerCard = new PlayerCard_UI(l_outputPlayer);
                    m_cardLoaded = true;
                }
            }
        }

        public static void DestroyCard()
        {
            m_playerCard.Destroy();
            m_playerCard = null;
            m_cardLoaded = false;
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");
            m_harmony.UnpatchSelf();
        }
    }
}
