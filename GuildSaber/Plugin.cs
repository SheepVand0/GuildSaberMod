#region Usings

using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using GuildSaber.BSPModule;
using GuildSaber.Logger;
using GuildSaber.UI;
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

        _ = new GSLogger(p_Logger);

        MenuButtons.instance.RegisterButton(new MenuButton("GuildSaber", "GuildSaber things", ShowGuildFlow));
        GuildSaberModule.HarmonyInstance.PatchAll();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private void ShowGuildFlow()
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
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            GameObject.DestroyImmediate(l_Current.gameObject);
        }
    }

}
