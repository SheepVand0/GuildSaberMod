﻿#region Usings
using System.Collections.Generic;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using BS_Utils.Gameplay;
using GuildSaber.API;
using GuildSaber.Configuration;
using GuildSaber.Time;
using GuildSaber.UI.Card;
using GuildSaber.UI.GuildSaber;
using GuildSaber.UI;
using UnityEngine;
using IPA;
using IPA.Config.Stores;
using System;
using Config = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;
#endregion

namespace GuildSaber;

[Plugin(RuntimeOptions.SingleStartInit)]
// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin
{
    #region Properties
    private static Plugin Instance { get; set; } = null;

    internal static IPALogger Log { get; private set; } = null;
    public HarmonyLib.Harmony m_HarmonyInstance { get => new HarmonyLib.Harmony("SheepVand.BeatSaber.GuildSaber"); }

    public const string NOT_DEFINED = "Undefined";
    public static string CurrentSceneName = "MainMenu";
    public static List<GuildData> AvailableGuilds = new List<GuildData>();
    public static ModFlowCoordinator _modFlowCoordinator = null;
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
        Log.Info("GuildSaber initialized.");

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
    public PlayerGuildsInfo(ApiPlayerData p_Player = default(ApiPlayerData), List<GuildData> p_AvailableGuilds = null)
    {
        m_ReturnPlayer = p_Player;
        m_AvailableGuilds = p_AvailableGuilds;
    }

    public ApiPlayerData m_ReturnPlayer { get; set; } = default(ApiPlayerData);
    public List<GuildData> m_AvailableGuilds { get; set; } = default(List<GuildData>);
}