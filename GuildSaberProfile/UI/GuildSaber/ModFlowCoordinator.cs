using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaberProfile.Configuration;
using HMUI;

namespace GuildSaberProfile.UI.GuildSaber;

public struct GuildCategories
{
    public string GuildName { get; set; }
    public List<string> Categories { get; set; }

    public GuildCategories(string p_GuildName, List<string> p_Categories)
    {
        GuildName = p_GuildName;
        Categories = p_Categories;
    }
}

[HotReload(RelativePathToLayout = @"PlayerCard_UI.bsml")]
[ViewDefinition("GuildSaberProfile.UI.Card.View.PlayerCard_UI.bsml")]
public class ModFlowCoordinator : FlowCoordinator
{
    public ModViewController _modViewController;

    public LeftModViewController _LeftModViewController;

    private FlowCoordinator _LastFlow;

    public void Awake()
    {
        if (_modViewController == null)
            _modViewController = BeatSaberUI.CreateViewController<ModViewController>();

        if (_LeftModViewController == null)
            _LeftModViewController = BeatSaberUI.CreateViewController<LeftModViewController>();
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        if (!firstActivation)
            return;

        SetTitle("GuildSaber");
        showBackButton = true;
        _modViewController.Init(PluginConfig.Instance.SelectedGuild);
        ProvideInitialViewControllers(_modViewController, _LeftModViewController);
    }

    public void ShowFlow(bool p_IsFromChildFlowCoordinator)
    {
        if (!p_IsFromChildFlowCoordinator)
            _LastFlow = BeatSaberUI.MainFlowCoordinator;
        else
            _LastFlow = BeatSaberUI.MainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf();
        _LastFlow.PresentFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical);
    }

    protected override void BackButtonWasPressed(ViewController topViewController)
    {
        base.BackButtonWasPressed(topViewController);
        _LastFlow.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical);
    }
}
