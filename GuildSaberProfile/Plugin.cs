#region Usings
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
using GuildSaberProfile.UI;
using UnityEngine;
using IPA;
using IPA.Config.Stores;
using System;
using Config = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;
#endregion

namespace GuildSaberProfile;

[Plugin(RuntimeOptions.SingleStartInit)]
// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin
{
    #region Properties
    private static Plugin Instance { get; set; }

    internal static IPALogger Log { get; private set; }

    public HarmonyLib.Harmony m_HarmonyInstance { get => new HarmonyLib.Harmony("SheepVand.BeatSaber.GuildSaberProfile"); }

    public const string NOT_DEFINED = "Undefined";
    public static string CurrentSceneName = "MainMenu";
    public static List<object> AvailableGuilds = new List<object>();
    public static ModFlowCoordinator _modFlowCoordinator;
    public static string m_PlayerId = string.Empty;
    public static int m_ScoresPerPage = 10;

    public static bool m_IsHsvInstalled = false;
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    #endregion

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
    #endregion

    #region On Game exit
    [OnExit]
    public void OnApplicationQuit()
    {
        Log.Debug("OnApplicationQuit");

        CustomUIComponent[] l_Components = Resources.FindObjectsOfTypeAll<CustomUIComponent>();

        foreach (CustomUIComponent l_Current in l_Components)
        {
            GameObject.DestroyImmediate(l_Current.gameObject);
        }

        m_HarmonyInstance.UnpatchSelf();
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
