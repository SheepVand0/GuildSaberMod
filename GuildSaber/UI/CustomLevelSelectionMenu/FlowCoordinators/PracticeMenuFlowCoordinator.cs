using BeatSaberMarkupLanguage;
using GuildSaber.UI.FlowCoordinator;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators
{
    internal class PracticeMenuFlowCoordinator : CustomFlowCoordinator
    {
        internal static bool IsPractice;

        internal static PracticeMenuFlowCoordinator Instance;

        protected override string Title => "Practice";

        protected ViewControllers.PracticeViewController m_ViewController;

        protected override (ViewController, ViewController, ViewController) GetUIImplementation()
        {
            if (m_ViewController == null)
                m_ViewController = BeatSaberUI.CreateViewController<ViewControllers.PracticeViewController>();
            return (m_ViewController, null, null);
        }

        public void ShowWithBeatmap(IDifficultyBeatmap p_Beatmap)
        {
            Present();
            m_ViewController.SetBeatmap(p_Beatmap);
        }
    }
}
