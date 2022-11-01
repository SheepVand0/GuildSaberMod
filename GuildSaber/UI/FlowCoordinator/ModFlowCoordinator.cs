using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaber.API;
using GuildSaber.Configuration;
using HMUI;

namespace GuildSaber.UI.GuildSaber;

public struct GuildCategories
{
    public int GuildId { get; set; } = 0;
    public List<CategoryData> Categories { get; set; } = default(List<CategoryData>);

    public GuildCategories(int p_GuildId, List<CategoryData> p_Categories)
    {
        GuildId = p_GuildId;
        Categories = p_Categories;
    }
}

[HotReload(RelativePathToLayout = @"PlayerCard_UI.bsml")]
[ViewDefinition("GuildSaber.UI.Card.View.PlayerCard_UI.bsml")]
public class ModFlowCoordinator : FlowCoordinator
{
    public ModViewController _modViewController = null;

    public LeftModViewController _LeftModViewController = null;

    private FlowCoordinator _LastFlow = null;

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
        _modViewController.Init(GSConfig.Instance.SelectedGuild);
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
