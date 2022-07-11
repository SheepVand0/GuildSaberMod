using BeatSaberMarkupLanguage.GameplaySetup;
using BS_Utils.Utilities;
using GuildSaberProfile.API;
using GuildSaberProfile.Configuration;
using GuildSaberProfile.UI.Card;
using GuildSaberProfile.UI.Settings;
using IPA;
using IPA.Config.Stores;
using UnityEngine;
using GuildSaberProfile.Time;
using System.Collections.Generic;
using System.Threading.Tasks;
using Config = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace GuildSaberProfile;

[Plugin(RuntimeOptions.SingleStartInit)]
// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private static Plugin Instance { get; set; }

    internal static IPALogger Log { get; private set; }

    public const string NOT_DEFINED = "Undefined";
    public static string CurrentSceneName = "MainMenu";

    private static bool s_CardLoaded;

    public static PlayerCard_UI PlayerCard;
    public static List<object> AvailableGuilds = new List<object>() { "CS", "BSCC" };
    public static TimeManager m_TimeManager;
    public static readonly SettingTabViewController m_TabViewController = new SettingTabViewController();
    //Harmony m_Harmony = new Harmony("SheepVand.BeatSaber.GuildSaberProfile");

    #region On mod start
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

    [OnStart]
    public void OnApplicationStart()
    {
        Log.Debug("OnApplicationStart");

        GameplaySetup.instance.AddTab("GuildSaberProfile", "GuildSaberProfile.UI.Settings.SettingTabViewController.bsml", m_TabViewController);

        BSEvents.lateMenuSceneLoadedFresh += OnMenuSceneLoadedFresh;

    }

    #region BSIPA Config
    [Init]
    public void InitWithConfig(Config p_Conf)
    {
        PluginConfig.Instance = p_Conf.Generated<PluginConfig>();
        Log.Debug("Config loaded");

        //m_Harmony.PatchAll();
    }

    #endregion
    #endregion

    #region Events
    private static void OnSceneChanged(UnityEngine.SceneManagement.Scene p_CurrentScene, UnityEngine.SceneManagement.Scene p_NextScene)
    {
        if (p_NextScene == null) return;

        CurrentSceneName = p_NextScene.name;
        PlayerCard.UpdateCardVisibility();
        PlayerCard.UpdateCardPosition();
    }

    private void OnMenuSceneLoadedFresh(ScenesTransitionSetupDataSO p_Obj)
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
        if (m_TimeManager == null)
            m_TimeManager = new GameObject("CardPlayTime").AddComponent<TimeManager>();

        if (PlayerCard != null)
            DestroyCard();
        CreateCard();
    }

    #endregion

    #region Card Manager
    public static async Task CreateCard()
    {
        if (s_CardLoaded) return;
        Plugin.Log.Info("Trying to get Player ID");

        /// We don't care if it return null because this function is loaded on the MenuSceneLoadedFresh, and the UserID will most likely be fetched way before that happen.
#pragma warning disable CS0618
        string l_PlayerId =  BS_Utils.Gameplay.GetUserInfo.GetUserID();
#pragma warning restore CS0618

        if(string.IsNullOrEmpty(l_PlayerId))
        {
            Plugin.Log.Error("Cannot get PLayer ID, not creating card");
            return;
        }

        PlayerApiReworkOutput l_OutputPlayer = GuildApi.GetPlayerByScoreSaberIdAndGuild(l_PlayerId, PluginConfig.Instance.SelectedGuild);

        if (l_OutputPlayer.Name == null || l_OutputPlayer.Level == 0)
        {
            m_TabViewController.ShowError(true);
            return;
        }

        m_TabViewController.ShowError(false);
        PlayerCard = new PlayerCard_UI(l_OutputPlayer);
        s_CardLoaded = true;

        m_TimeManager.SetPlayerCardViewControllerRef((PlayerCard.CardViewController != null) ? PlayerCard.CardViewController : null);
    }

    public static async Task DestroyCard()
    {
        if (PlayerCard != null && PlayerCard.CardViewController != null)
        {
            PlayerCard.Destroy();
            PlayerCard = null;
        }
        s_CardLoaded = false;
    }
    #endregion

    #region On Game exit
    [OnExit]
    public void OnApplicationQuit()
    {
        Log.Debug("OnApplicationQuit");
        //m_Harmony.UnpatchSelf();
    }
    #endregion
}
