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
using CSProfile.UI;
using IPALogger = IPA.Logging.Logger;

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
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            new GameObject("CSProfileController").AddComponent<CSProfileController>();

            BSEvents.lateMenuSceneLoadedFresh += OnMenuSceneLoadedFresh;
        }

        private void OnMenuSceneLoadedFresh(ScenesTransitionSetupDataSO obj)
        {
            var l_playerId = Authentification.GetPlayerIdFromSteam();
            if (l_playerId != m_notDefined)
            {
                m_currentPlayerId = l_playerId;
                PlayerApiReworkOutput l_outputPlayer = CSApi.GetPlayerByScoreSaberId(Plugin.m_currentPlayerId);

                Plugin.Log.Info($"R{l_outputPlayer.ProfileColor.R}");
                Plugin.Log.Info($"G{l_outputPlayer.ProfileColor.G}");
                Plugin.Log.Info($"B{l_outputPlayer.ProfileColor.B}");

                List<CustomApiPlayerCategory> l_playerCat = l_outputPlayer.CategoryData;
                for (int l_i = 0; l_i < l_outputPlayer.CategoryData.Count; l_i++)
                {
                    Plugin.Log.Info($"Level for {l_playerCat[l_i].Category.Normalize()} : {l_playerCat[l_i].Level}");
                }

                m_playerCard = new PlayerCard_UI(l_outputPlayer);
            }
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");

        }
    }
}
