using System.Reflection;
using BeatSaberPlus.SDK.UI;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaber.UI.CustomLevelSelectionMenu.Components;
using GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;
using UnityEngine.UI;

namespace GuildSaber.UI.CustomLevelSelectionMenu
{
    internal class GuildSelectionMenuCell
    {
        [UIComponent("MainLayout")] private HorizontalLayoutGroup m_MainLayout = null;

        private readonly string m_Title;
        private readonly string m_Description;
        private readonly string m_Image;

        public GuildSelectionMenuCell(string p_Text, string p_Description, string p_Image)
        {
            m_Title = p_Text;
            m_Description = p_Description;
            m_Image = p_Image;
        }

        [UIAction("#post-parse")] private void _PostParse()
        {
            RoundedButton.Create(m_MainLayout.transform, m_Title, m_Description, () =>
            {
                if (GuildSelectionMenu.m_CategorySelectionFlowCoordinator == null)
                    GuildSelectionMenu.m_CategorySelectionFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<CategorySelectionFlowCoordinator>();

                GuildSelectionMenu.m_CategorySelectionFlowCoordinator.Init(m_Title, m_Description);
                GuildSelectionMenu.m_CategorySelectionFlowCoordinator.Show();
            }, m_Image);
        }
    }

    internal class GuildSelectionMenu : ViewController<GuildSelectionMenu>
    {

        internal const string VIEW_CONTROLLERS_PATH = "GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Views";

        internal static CategorySelectionFlowCoordinator m_CategorySelectionFlowCoordinator = null;

        protected override string GetViewContentDescription()
        {
            return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "GuildSaber.UI.CustomLevelSelectionMenu.Views.GuildSelectionMenu.bsml");
        }

        protected override void OnViewCreation()
        {

        }
    }
}
