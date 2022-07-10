using BeatSaberMarkupLanguage.GameplaySetup;
using BS_Utils.Utilities;
using GuildSaberProfile.API;
using GuildSaberProfile.Configuration;
using GuildSaberProfile.UI.Card;
using GuildSaberProfile.UI.Settings;
using IPA;
using IPA.Config.Stores;
using UnityEngine;
using System.Collections.Generic;
using Config = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace GuildSaberProfile;

[Plugin(RuntimeOptions.SingleStartInit)]
// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin
{
    public const string NOT_DEFINED = "Undefined";

    public static PlayerCard_UI PlayerCard;

    private static bool s_CardLoaded;

    public static readonly SettingTabViewController m_TabViewController = new SettingTabViewController();
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private static Plugin Instance { get; set; }
    public static string CurrentSceneName = "MainMenu";
    public static List<string> m_AvaibleGuilds = new List<string>() { "CS", "BSCC"};
    internal static IPALogger Log { get; private set; }

    //Harmony m_Harmony = new Harmony("SheepVand.BeatSaber.GuildSaberProfile");

    [Init]
    /// <summary>
    /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
    /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
    /// Only use [Init] with one Constructor.
    /// </summary>
    public void Init(IPALogger p_Logger)
    {
        Instance = this;
        Log = p_Logger;
        Log.Info("GuildSaberProfile initialized.");
    }

    #region BSIPA Config

    //Uncomment to use BSIPA's config

    [Init]
    public void InitWithConfig(Config p_Conf)
    {
        PluginConfig.Instance = p_Conf.Generated<PluginConfig>();
        Log.Debug("Config loaded");

        //m_Harmony.PatchAll();
    }

    #endregion

    [OnStart]
    public void OnApplicationStart()
    {
        Log.Debug("OnApplicationStart");

        GameplaySetup.instance.AddTab("GuildSaberProfile", "GuildSaberProfile.UI.Settings.SettingTabViewController.bsml", m_TabViewController);

        BSEvents.lateMenuSceneLoadedFresh += OnMenuSceneLoadedFresh;

    }

    private static void OnSceneChanged(UnityEngine.SceneManagement.Scene p_CurrentScene, UnityEngine.SceneManagement.Scene p_NextScene)
    {
        if (p_NextScene == null) return;

        CurrentSceneName = p_NextScene.name;
        PlayerCard.UpdateCardVisibility();
    }

    private static void OnMenuSceneLoadedFresh(ScenesTransitionSetupDataSO p_Obj)
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
        CreateCard();
    }

    public static void CreateCard()
    {
        if (s_CardLoaded) return;

        string l_PlayerId = Authentication.GetPlayerIdFromSteam();
        if (l_PlayerId == NOT_DEFINED) return;

        PlayerApiReworkOutput l_OutputPlayer = GuildApi.GetPlayerByScoreSaberIdAndGuild(l_PlayerId, PluginConfig.Instance.SelectedGuild);

        if (l_OutputPlayer.Name == null)
        {
            m_TabViewController.ShowError(true);
            return;
        };

        m_TabViewController.ShowError(false);
        PlayerCard = new PlayerCard_UI(l_OutputPlayer);
        s_CardLoaded = true;
    }

    public static void DestroyCard()
    {
        if (PlayerCard != null && PlayerCard.CardViewController != null)
        {
            PlayerCard.Destroy();
            PlayerCard = null;
        }
        s_CardLoaded = false;
    }

    [OnExit]
    public void OnApplicationQuit()
    {
        Log.Debug("OnApplicationQuit");
        //m_Harmony.UnpatchSelf();
    }
}
