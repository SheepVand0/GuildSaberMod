using BeatSaberMarkupLanguage;
using GuildSaber.Utils;
using HMUI;

namespace GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators
{
    internal class CategorySelectionFlowCoordinator : CustomFlowCoordinator
    {
        private GuildDescriptionViewController m_GuildDescriptionViewController = null;

        protected override string Title => "Categories";

        protected override (ViewController?, ViewController?, ViewController?) GetUIImplementation()
        {
            if (m_GuildDescriptionViewController == null)
                m_GuildDescriptionViewController = BeatSaberUI.CreateViewController<GuildDescriptionViewController>();

            return (null, m_GuildDescriptionViewController, null);
        }

        private string Name { get; set; }
        private string Description { get; set; }

        public void Init(string p_Name, string p_Description)
        {
            Name = p_Name;
            Description = p_Description;
        }

        protected override async void OnShow()
        {
            await WaitUtils.Wait(() => m_GuildDescriptionViewController != null, 1, p_CodeLine: 32);

            m_GuildDescriptionViewController.SetNameAndDescription(Name, Description);

            Name = string.Empty;
            Description = string.Empty;
        }

    }
}
