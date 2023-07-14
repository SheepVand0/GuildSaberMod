using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers;
using CP_SDK.UI;
using GuildSaber.API;

namespace GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators
{
    internal class LevelsFlowCoordinator : FlowCoordinator<LevelsFlowCoordinator>
    {
        internal static LevelsFlowCoordinator Instance;

        LevelSelectionViewController m_ViewController = UISystem.CreateViewController<LevelSelectionViewController>();

        public override string Title => "Play";

        public void ShowWithLevels(int p_GuildId, ApiCategory p_Category)
        {
            m_ViewController.SetLevels(p_GuildId, p_Category);
            Present();
        }

        protected override (IViewController, IViewController, IViewController) GetInitialViewsController()
        {
            return (m_ViewController, null, null);
        }
    }
}
