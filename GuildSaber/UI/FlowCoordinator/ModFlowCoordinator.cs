using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaber.API;
using GuildSaber.Configuration;
using HMUI;

namespace GuildSaber.UI.GuildSaber;

public class ModFlowCoordinator : FlowCoordinator
{
    public PlaylistViewController _modViewController = null;

    public LeftModViewController _LeftModViewController = null;

    private FlowCoordinator _LastFlow = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Awake
    /// </summary>
    public void Awake()
    {
        if (_modViewController == null)
            _modViewController = BeatSaberUI.CreateViewController<PlaylistViewController>();

        if (_LeftModViewController == null)
            _LeftModViewController = BeatSaberUI.CreateViewController<LeftModViewController>();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// On activate
    /// </summary>
    /// <param name="firstActivation"></param>
    /// <param name="addedToHierarchy"></param>
    /// <param name="screenSystemEnabling"></param>
    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        if (!firstActivation)
            return;

        SetTitle("GuildSaber");
        showBackButton = true;
        _modViewController.Init(GSConfig.Instance.SelectedGuild);
        ProvideInitialViewControllers(_modViewController, _LeftModViewController);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Show Flow
    /// </summary>
    /// <param name="p_IsFromChildFlowCoordinator"></param>
    public void ShowFlow(bool p_IsFromChildFlowCoordinator)
    {
        if (!p_IsFromChildFlowCoordinator)
            _LastFlow = BeatSaberUI.MainFlowCoordinator;
        else
            _LastFlow = BeatSaberUI.MainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf();
        _LastFlow.PresentFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// On Back button pressed
    /// </summary>
    /// <param name="topViewController"></param>
    protected override void BackButtonWasPressed(ViewController topViewController)
    {
        base.BackButtonWasPressed(topViewController);
        _LastFlow.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical);
    }
}
