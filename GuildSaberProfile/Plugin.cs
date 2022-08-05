using System.Collections.Generic;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using BS_Utils.Gameplay;
using GuildSaberProfile.API;
using GuildSaberProfile.Configuration;
using GuildSaberProfile.Time;
using GuildSaberProfile.UI.Card;
using GuildSaberProfile.UI.GuildSaber;
using GuildSaberProfile.Utils;
using IPA;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using Config = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace GuildSaberProfile;

[Plugin(RuntimeOptions.SingleStartInit)]
// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin
{

    private static Plugin Instance { get; set; }

    internal static IPALogger Log { get; private set; }

    public HarmonyLib.Harmony m_HarmonyInstance { get => new HarmonyLib.Harmony("SheepVand.BeatSaber.GuildSaberProfile"); }


    private static bool s_CardLoaded = false;

    public const string NOT_DEFINED = "Undefined";
    public static string CurrentSceneName = "MainMenu";
    public static List<object> AvailableGuilds = new List<object>();
    public static TimeManager m_TimeManager;
    public static ModFlowCoordinator _modFlowCoordinator;
    public static PlayerCard_UI PlayerCard;
    public static string m_PlayerId = string.Empty;
    public static IRefresh m_Refresher = new Refresher();
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
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

        MenuButtons.instance.RegisterButton(new MenuButton("GuildSaber", "GuildSaber things", ShowGuildFlow));
    }

    [OnStart]
    public void OnApplicationStart()
    {
        Log.Debug("OnApplicationStart");

        m_HarmonyInstance.PatchAll();

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

    #region On Game exit
    [OnExit]
    public void OnApplicationQuit()
    {
        Log.Debug("OnApplicationQuit");
        m_HarmonyInstance.UnpatchSelf();
    }
    #endregion

    #region Card Manager
    public static PlayerGuildsInfo GetPlayerInfoFromAPI(bool p_GuildFromConfig = true, string p_Guild = null)
    {
        Log.Info("Trying to get Player ID");

        //-----------------------------------------Gettings Player Id-----------------------------------------

        /// We don't care if it return null because this function is loaded on the MenuSceneLoadedFresh, and the UserID will most likely be fetched way before that happen.
#pragma warning disable CS0618
        m_PlayerId = GetUserInfo.GetUserID();
        #pragma warning restore CS0618

        if (string.IsNullOrEmpty(m_PlayerId))
        {
            Log.Error("Cannot get Player ID, not creating card");
            _modFlowCoordinator._LeftModViewController.ShowError(true);
            return new PlayerGuildsInfo();
        }

        //-----------------------------------------Defaults-----------------------------------------

        string l_SelectedGuild = (p_GuildFromConfig == true) ? PluginConfig.Instance.SelectedGuild : p_Guild;

        //Temp Player in for
        PlayerApiReworkOutput l_OutputPlayer = new PlayerApiReworkOutput();
        //This value will returned as final Player
        PlayerApiReworkOutput l_DefinedPlayer = new PlayerApiReworkOutput();
        //Last valid Player detected in for
        PlayerApiReworkOutput l_LastValidPlayer = new PlayerApiReworkOutput();
        //Same but for Guild
        string l_LastValidGuild = string.Empty;

        List<string> l_TempAvailableGuilds = new List<string>
            { "CS", "BSCC" };
        AvailableGuilds = new List<object>();

        //-----------------------------------------Finding valid Player for Available Guilds-----------------------------------------

        for (int l_i = 0; l_i < l_TempAvailableGuilds.Count; l_i++)
        {
            //Getting Player from Selected guild
            l_OutputPlayer = GuildApi.GetPlayerByScoreSaberIdAndGuild(m_PlayerId, l_TempAvailableGuilds[l_i]);

            /*If Current Player from guild is valid settings l_LastValidPlayer to l_OutputPlayer and adding guild to AvailableGuilds,
            l_LastValidGuild is defined to the current guild too*/
            if (!l_OutputPlayer.Equals(null) && l_OutputPlayer.Level > 0)
            {
                l_LastValidPlayer = l_OutputPlayer;
                l_LastValidGuild = l_TempAvailableGuilds[l_i];
                AvailableGuilds.Add(l_TempAvailableGuilds[l_i]);
            }

            //If the current guild in for is the selected, l_DefinedPlayer will be set to OutputPlayer (Current Player)
            if (l_TempAvailableGuilds[l_i] == l_SelectedGuild)
                l_DefinedPlayer = l_OutputPlayer;
        }

        //-----------------------------------------Player found for guilds verification-----------------------------------------

        //If there is no valid guilds returning empty PlayerGuildsInfo
        if (AvailableGuilds.Count == 0) return new();

        //If the selected guild is not valid for current Player settings, settings SelectedGuild to l_LastValidGuild and DefinedPlayer to l_LastValidPlayer
        if (!IsGuildValidForPlayer(PluginConfig.Instance.SelectedGuild))
        {
            PluginConfig.Instance.SelectedGuild = l_LastValidGuild;
            l_DefinedPlayer = l_LastValidPlayer;
        }

        //If the processes succeffully end, returning l_DefinedPlayer and AvailableGuilds for Player
        return new(l_DefinedPlayer, AvailableGuilds);
    }

    public static PlayerGuildsInfo GetPlayerInfoFromCurrent()
    {
        if (PlayerCard.CardViewController != null)
            return new PlayerGuildsInfo(PlayerCard.CardViewController.m_PlayerInfo, AvailableGuilds);
        else
            return new PlayerGuildsInfo();
    }

    public static void CreateCard()
    {
        if (s_CardLoaded) return;

        //-----------------------------------------Defaults-----------------------------------------

        //The Final Player Returned
        PlayerApiReworkOutput l_DefinedPlayer = new PlayerApiReworkOutput();
        //Return PlayerGuildsInfo From API
        PlayerGuildsInfo l_PlayerGuildsInfo = GetPlayerInfoFromAPI();

        l_DefinedPlayer = l_PlayerGuildsInfo.m_ReturnPlayer;


        //The name of the function enough explicit
        if (string.IsNullOrEmpty(l_DefinedPlayer.Name))
        {
            if (_modFlowCoordinator._LeftModViewController != null)
                _modFlowCoordinator._LeftModViewController.ShowError(true);

            s_CardLoaded = false;
            return;
        }

        //Explicit too
        PlayerCard = new PlayerCard_UI(l_DefinedPlayer, AvailableGuilds);
        s_CardLoaded = true;

        m_TimeManager.SetPlayerCardViewControllerRef(PlayerCard.CardViewController != null ? PlayerCard.CardViewController : null);
    }

    public static bool IsGuildValidForPlayer(string p_Guild)
    {
        bool l_IsValid = false;
        foreach (string l_Current in AvailableGuilds)
        {
            if (l_Current == p_Guild)
            {
                Log.Info("Selected guild is valid for this player not changing");
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
}

public struct PlayerGuildsInfo
{
    public PlayerGuildsInfo(PlayerApiReworkOutput p_Player = new PlayerApiReworkOutput(), List<string> p_AvailableGuilds = null)
    {
        m_ReturnPlayer = p_Player;
        m_AvailableGuilds = p_AvailableGuilds;
    }

    public PlayerGuildsInfo(PlayerApiReworkOutput p_Player = new PlayerApiReworkOutput(), List<object> p_AvailableGuilds = null)
    {
        m_ReturnPlayer = p_Player;

        //Converting List<object> to List<string>
        List<string> l_Temp = new List<string>();
        foreach (object l_Current in p_AvailableGuilds)
        {
            if(l_Current.GetType() == typeof(string))
            {
                l_Temp.Add((string)l_Current);
            }
        }

        m_AvailableGuilds = l_Temp;
    }
    public PlayerApiReworkOutput m_ReturnPlayer { get; set; }
    public List<string> m_AvailableGuilds { get; set; }
}
