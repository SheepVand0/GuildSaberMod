using System.Collections.Generic;
using System.Reflection;
using BeatSaberPlus.SDK.UI;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using GuildSaber.BSPModule;
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

<<<<<<< Updated upstream
        [UIAction("#post-parse")] private void _PostParse()
=======
    [UIAction("#post-parse")] private void _PostParse()
    {
        RoundedButton.Create(m_MainLayout.transform, m_Title, m_Description, 85f, 25f, () =>
>>>>>>> Stashed changes
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

        [UIComponent("GuildList")] private CustomCellListTableData m_CustomCellListTableData = null;

        [UIValue("Guilds")] private List<object> m_Guilds = new List<object>();

<<<<<<< Updated upstream
        protected override string GetViewContentDescription()
        {
            return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), $"{VIEW_CONTROLLERS_PATH}.GuildSelectionMenu.bsml");
        }
=======
   protected override string GetViewContentDescription() { return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), $"{VIEW_CONTROLLERS_PATH}.GuildSelectionMenu.bsml"); }
>>>>>>> Stashed changes

        protected override void OnViewActivation()
        {
            m_Guilds.Clear();
            foreach (var l_Guild in GuildSaberModule.AvailableGuilds)
                m_Guilds.Add(new GuildSelectionMenuCell(l_Guild.Name, l_Guild.Description, l_Guild.Logo));
            m_CustomCellListTableData.tableView.ReloadData();
        }
    }
}
