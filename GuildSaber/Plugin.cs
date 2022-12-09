#region Usings

using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using GuildSaber.BSPModule;
using GuildSaber.Logger;
using GuildSaber.UI;
using GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;
using GuildSaber.UI.GuildSaber;
using IPA;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

#endregion

namespace GuildSaber;

[Plugin(RuntimeOptions.SingleStartInit)]
// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin
{
    private static Plugin Instance { get; set; } = null;

    public const string NOT_DEFINED = "Undefined";

    internal static ModFlowCoordinator ModFlowCoordinator = null;
    internal static GuildSelectionFlowCoordinator SelectionFlowCoordinator = null;

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

        _ = new GSLogger(p_Logger);

        MenuButtons.instance.RegisterButton(new MenuButton("GuildSaber", "GuildSaber things", ShowGuildFlow));
        //MenuButtons.instance.RegisterButton(new MenuButton("GuildSaber Playing Menu", "GuildSaber Playing Menu", ShowSelectionFlow));
        GuildSaberModule.HarmonyInstance.PatchAll();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private void ShowGuildFlow()
    {
        if (ModFlowCoordinator == null)
            ModFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<ModFlowCoordinator>();

        ModFlowCoordinator.Show();
    }

    private void ShowSelectionFlow()
    {
        if (SelectionFlowCoordinator == null)
            SelectionFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<GuildSelectionFlowCoordinator>();

        SelectionFlowCoordinator.Show();
    }

    [OnExit]
    public void OnApplicationQuit()
    {
        CustomUIComponent[] l_Components = Resources.FindObjectsOfTypeAll<CustomUIComponent>();

        foreach (CustomUIComponent l_Current in l_Components)
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            GameObject.DestroyImmediate(l_Current.gameObject);
        }
    }

}
