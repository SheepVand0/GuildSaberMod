using BeatSaberMarkupLanguage;
using GuildSaber.Configuration;
using HMUI;

namespace GuildSaber.UI.GuildSaber;

internal class ModFlowCoordinator : CustomFlowCoordinator
{
    public PlaylistViewController _modViewController = null;

    public LeftModViewController _LeftModViewController = null;

    private FlowCoordinator _LastFlow = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override string Title => "Guild Saber";
    protected override (ViewController?, ViewController?, ViewController?) GetUIImplementation()
    {
        if (_modViewController == null)
            _modViewController = BeatSaberUI.CreateViewController<PlaylistViewController>();

        if (_LeftModViewController == null)
            _LeftModViewController = BeatSaberUI.CreateViewController<LeftModViewController>();

        return (_modViewController, _LeftModViewController, null);
    }

    protected override void OnShow()
    {
        _modViewController.Init(GSConfig.Instance.SelectedGuild);
    }

}
