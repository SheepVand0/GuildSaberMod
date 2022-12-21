using BeatSaberMarkupLanguage;
using GuildSaber.UI.FlowCoordinator;
using HMUI;

namespace GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;

internal class GuildSelectionFlowCoordinator : CustomFlowCoordinator
{

    private GuildSelectionMenu m_SelectionMenu;
    protected override string Title
    {
        get => "Select a guild";
    }

    protected override (ViewController?, ViewController?, ViewController?) GetUIImplementation()
    {
        if (m_SelectionMenu == null) m_SelectionMenu = BeatSaberUI.CreateViewController<GuildSelectionMenu>();
        return (m_SelectionMenu, null, null);
    }
}