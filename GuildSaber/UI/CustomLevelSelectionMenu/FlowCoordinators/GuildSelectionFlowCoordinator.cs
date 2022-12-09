using BeatSaberMarkupLanguage;
using HMUI;

namespace GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators
{
    internal class GuildSelectionFlowCoordinator : CustomFlowCoordinator
    {
        protected override string Title => "Select a guild";

        private GuildSelectionMenu m_SelectionMenu = null;

        protected override (ViewController?, ViewController?, ViewController?) GetUIImplementation()
        {
            if (m_SelectionMenu == null)
                m_SelectionMenu = BeatSaberUI.CreateViewController<GuildSelectionMenu>();
            return (m_SelectionMenu, null, null);
        }
    }
}
