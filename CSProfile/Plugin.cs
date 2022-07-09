using BeatSaberMarkupLanguage.GameplaySetup;
using BS_Utils.Utilities;
using CSProfile.API;
using CSProfile.Configuration;
using CSProfile.UI.Card;
using CSProfile.UI.Settings;
<<<<<<< Updated upstream
using HarmonyLib;
=======
>>>>>>> Stashed changes
using IPA;
using IPA.Config.Stores;
using UnityEngine;
using Config = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace CSProfile;
<<<<<<< Updated upstream

[Plugin(RuntimeOptions.SingleStartInit)]
public class Plugin
{

=======

[Plugin(RuntimeOptions.SingleStartInit)]
// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin
{
>>>>>>> Stashed changes
    public const string NOT_DEFINED = "Undefined";

    public static PlayerCard_UI PlayerCard;

<<<<<<< Updated upstream
    private static bool s_CardLoaded;

    private readonly Harmony m_Harmony = new Harmony("SheepVand.BeatSaber.CSProfile");

    private readonly SettingTabViewController m_TabViewController = new SettingTabViewController();
    private static Plugin Instance { get; set; }
    internal static IPALogger Log { get; private set; }
=======
    public static bool CardLoaded;

    public readonly SettingTabViewController TabViewController = new SettingTabViewController();
    internal static Plugin Instance { get; private set; }
    internal static IPALogger Log { get; private set; }

    //Harmony m_Harmony = new Harmony("SheepVand.BeatSaber.CSProfile");
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
        m_Harmony.PatchAll();
=======
        //m_Harmony.PatchAll();
>>>>>>> Stashed changes
    }

    #endregion

    [OnStart]
    public void OnApplicationStart()
    {
        Log.Debug("OnApplicationStart");
        new GameObject("CSProfileController").AddComponent<CSProfileController>();

<<<<<<< Updated upstream
        GameplaySetup.instance.AddTab("Player Card", "CSProfile.UI.Settings.SettingTabViewController.bsml", m_TabViewController);
=======
        GameplaySetup.instance.AddTab("CSProfile", "CSProfile.UI.Settings.SettingTabViewController.bsml", TabViewController);
>>>>>>> Stashed changes

        BSEvents.lateMenuSceneLoadedFresh += OnMenuSceneLoadedFresh;
    }

<<<<<<< Updated upstream
    private static void OnMenuSceneLoadedFresh(ScenesTransitionSetupDataSO p_Obj)
=======
    private void OnMenuSceneLoadedFresh(ScenesTransitionSetupDataSO p_Obj)
>>>>>>> Stashed changes
    {
        CreateCard();
    }

    public static void CreateCard()
    {
<<<<<<< Updated upstream
        if (s_CardLoaded) return;
=======
        if (CardLoaded) return;
>>>>>>> Stashed changes

        string l_PlayerId = Authentication.GetPlayerIdFromSteam();
        if (l_PlayerId == NOT_DEFINED) return;

        PlayerApiReworkOutput l_OutputPlayer = CSApi.GetPlayerByScoreSaberId(l_PlayerId);

        PlayerCard = new PlayerCard_UI(l_OutputPlayer);
<<<<<<< Updated upstream
        s_CardLoaded = true;
=======
        CardLoaded = true;
>>>>>>> Stashed changes
    }

    public static void DestroyCard()
    {
        PlayerCard.Destroy();
        PlayerCard = null;
<<<<<<< Updated upstream
        s_CardLoaded = false;
=======
        CardLoaded = false;
>>>>>>> Stashed changes
    }

    [OnExit]
    public void OnApplicationQuit()
    {
        Log.Debug("OnApplicationQuit");
<<<<<<< Updated upstream
        m_Harmony.UnpatchSelf();
=======
        //m_Harmony.UnpatchSelf();
>>>>>>> Stashed changes
    }
}
