#region Usings
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
using GuildSaber.BSPModule;
using GuildSaber.Logger;
#endregion

namespace GuildSaber;

[Plugin(RuntimeOptions.SingleStartInit)]
// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin
{
    private static Plugin Instance { get; set; } = null;

    public const string NOT_DEFINED = "Undefined";

    public static ModFlowCoordinator _modFlowCoordinator = null;


    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [Init]
    /// <summary>
    /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
    /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
    /// Only use [Init] with one Constructor.
    /// </summary>
    public void Init(IPALogger p_Logger)
    {
        Instance = this;

        new GSLogger(p_Logger);

        MenuButtons.instance.RegisterButton(new MenuButton("GuildSaber", "GuildSaber things", ShowGuildFlow));
        GuildSaberModule.m_HarmonyInstance.PatchAll();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public void ShowGuildFlow()
    {
        if (_modFlowCoordinator == null)
            _modFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<ModFlowCoordinator>();

        _modFlowCoordinator.ShowFlow(false);
    }

    [OnExit]
    public void OnApplicationQuit()
    {
        CustomUIComponent[] l_Components = Resources.FindObjectsOfTypeAll<CustomUIComponent>();

        foreach (CustomUIComponent l_Current in l_Components)
        {
            GameObject.DestroyImmediate(l_Current.gameObject);
        }
    }

}
