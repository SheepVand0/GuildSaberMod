using BeatSaberMarkupLanguage.Settings;
using BS_Utils.Utilities;
using GuildSaberProfile.API;
using GuildSaberProfile.Configuration;
using GuildSaberProfile.UI.Card;
using IPA;
using IPA.Config.Stores;
using UnityEngine;
using GuildSaberProfile.Time;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.MenuButtons;
using BeatSaberMarkupLanguage;
using GuildSaberProfile.UI.GuildSaber;
using GuildSaberProfile.UI.Card.Settings;
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
    public static List<object> AvailableGuilds = new List<object>();
    public static TimeManager m_TimeManager;
    static ModFlowCoordinator _modFlowCoordinator;
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

        MenuButtons.instance.RegisterButton(new MenuButton("GuildSaber", "GuildSaber things", ShowGuildFlow, true));
    }

    [OnStart]
    public void OnApplicationStart()
    {
        Log.Debug("OnApplicationStart");

        BSEvents.lateMenuSceneLoadedFresh += OnMenuSceneLoadedFresh;
    }

    public void ShowGuildFlow()
    {
        if (_modFlowCoordinator == null)
            _modFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<ModFlowCoordinator>();

        _modFlowCoordinator.ShowFlow(false);
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
        PlayerCard.CardViewController.UpdateToggleCardHandleVisibility();
    }

    private void OnMenuSceneLoadedFresh(ScenesTransitionSetupDataSO p_Obj)
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
        if (m_TimeManager == null)
        {
            m_TimeManager = new GameObject("CardPlayTime").AddComponent<TimeManager>();
            GameObject.DontDestroyOnLoad(m_TimeManager);
        }

        if (PlayerCard != null)
            DestroyCard();
        CreateCard();
    }
    #endregion

    #region Card Manager
    public static void CreateCard()
    {
        if (s_CardLoaded) return;
        Plugin.Log.Info("Trying to get Player ID");

        /// We don't care if it return null because this function is loaded on the MenuSceneLoadedFresh, and the UserID will most likely be fetched way before that happen.
#pragma warning disable CS0618
        string l_PlayerId =  BS_Utils.Gameplay.GetUserInfo.GetUserID();
#pragma warning restore CS0618

        if(string.IsNullOrEmpty(l_PlayerId))
        {
            Plugin.Log.Error("Cannot get Player ID, not creating card");
            _modFlowCoordinator._LeftModViewController.ShowError(true);
            return;
        }

        PlayerApiReworkOutput l_OutputPlayer = new PlayerApiReworkOutput();
        PlayerApiReworkOutput l_DefinedPlayer = new PlayerApiReworkOutput();
        PlayerApiReworkOutput l_LastValidPlayer = new PlayerApiReworkOutput();
        string l_LastValidGuild = string.Empty;

        List<string> l_TempAvailableGuilds = new List<string>() { "CS", "BSCC"};
        AvailableGuilds = new List<object>();

        for (int l_i = 0;l_i < l_TempAvailableGuilds.Count;l_i++)
        {
            l_OutputPlayer = GuildApi.GetPlayerByScoreSaberIdAndGuild(l_PlayerId, l_TempAvailableGuilds[l_i]);

            if (l_TempAvailableGuilds[l_i] == PluginConfig.Instance.SelectedGuild)
                l_DefinedPlayer = l_OutputPlayer;

            if (!l_OutputPlayer.Equals(null) && l_OutputPlayer.Level > 0)
            {
                l_LastValidPlayer = l_OutputPlayer;
                l_LastValidGuild = l_TempAvailableGuilds[l_i];
                AvailableGuilds.Add(l_TempAvailableGuilds[l_i]);
            }
        }

        if (AvailableGuilds.Count == 0) return;

        if (!IsGuildValidForPlayer(PluginConfig.Instance.SelectedGuild))
        {
            PluginConfig.Instance.SelectedGuild = l_LastValidGuild;
            l_DefinedPlayer = l_LastValidPlayer;
        }

        //m_TabViewController.ShowError(false);
        PlayerCard = new PlayerCard_UI(l_DefinedPlayer, AvailableGuilds);
        s_CardLoaded = true;

        m_TimeManager.SetPlayerCardViewControllerRef((PlayerCard.CardViewController != null) ? PlayerCard.CardViewController : null);
    }

    public static bool IsGuildValidForPlayer(string p_Guild)
    {
        bool l_IsValid = false;
        foreach (string l_Current in AvailableGuilds)
        {
            if (l_Current == p_Guild)
            {
                Plugin.Log.Info("Selected guild is valid for this player not changing");
                l_IsValid = true;
                break;
            }
        }
        return l_IsValid;
    }

    public static async Task<Task> DestroyCard()
    {
        if (PlayerCard != null && PlayerCard.CardViewController != null)
        {
            PlayerCard.CardViewController.m_SettingsModal = null;
            PlayerCard.Destroy();
            PlayerCard = null;
        }
        s_CardLoaded = false;

        return Task.CompletedTask;
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
