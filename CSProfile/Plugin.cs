using BeatSaberMarkupLanguage.GameplaySetup;
using BS_Utils.Utilities;
using CSProfile.API;
using CSProfile.Configuration;
using CSProfile.UI.Card;
using CSProfile.UI.Settings;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using UnityEngine;
using Config = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace CSProfile;

[Plugin(RuntimeOptions.SingleStartInit)]
public class Plugin
{

    public const string NOT_DEFINED = "Undefined";

    public static PlayerCard_UI PlayerCard;

    private static bool s_CardLoaded;

    private readonly Harmony m_Harmony = new Harmony("SheepVand.BeatSaber.CSProfile");

    private readonly SettingTabViewController m_TabViewController = new SettingTabViewController();
    private static Plugin Instance { get; set; }
    internal static IPALogger Log { get; private set; }

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
        Log.Info("CSProfile initialized.");
    }

    #region BSIPA Config

    //Uncomment to use BSIPA's config

    [Init]
    public void InitWithConfig(Config p_Conf)
    {
        PluginConfig.Instance = p_Conf.Generated<PluginConfig>();
        Log.Debug("Config loaded");

        m_Harmony.PatchAll();
    }

    #endregion

    [OnStart]
    public void OnApplicationStart()
    {
        Log.Debug("OnApplicationStart");
        new GameObject("CSProfileController").AddComponent<CSProfileController>();

        GameplaySetup.instance.AddTab("Player Card", "CSProfile.UI.Settings.SettingTabViewController.bsml", m_TabViewController);

        BSEvents.lateMenuSceneLoadedFresh += OnMenuSceneLoadedFresh;
    }

    private static void OnMenuSceneLoadedFresh(ScenesTransitionSetupDataSO p_Obj)
    {
        CreateCard();
    }

    public static void CreateCard()
    {
        if (s_CardLoaded) return;

        string l_PlayerId = Authentication.GetPlayerIdFromSteam();
        if (l_PlayerId == NOT_DEFINED) return;

        PlayerApiReworkOutput l_OutputPlayer = CSApi.GetPlayerByScoreSaberId(l_PlayerId);

        PlayerCard = new PlayerCard_UI(l_OutputPlayer);
        s_CardLoaded = true;
    }

    public static void DestroyCard()
    {
        PlayerCard.Destroy();
        PlayerCard = null;
        s_CardLoaded = false;
    }

    [OnExit]
    public void OnApplicationQuit()
    {
        Log.Debug("OnApplicationQuit");
        m_Harmony.UnpatchSelf();
    }
}
