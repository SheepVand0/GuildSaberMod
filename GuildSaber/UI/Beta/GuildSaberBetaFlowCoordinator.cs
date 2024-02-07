using BeatSaberMarkupLanguage;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.UI.Beta
{
    internal class GuildSaberBetaFlowCoordinator : GuildSaber.UI.FlowCoordinator.CustomFlowCoordinator
    {
        public static GuildSaberBetaFlowCoordinator Instance;

        protected override string Title => "Beta menu";

        GuildSaberBetaViewController m_ViewController = BeatSaberUI.CreateViewController<GuildSaberBetaViewController>();

        protected override void OnCreation()
        {
            Instance = this;
        }

        protected override (ViewController, ViewController, ViewController) GetUIImplementation()
        {
            return (m_ViewController, null, null);
        }
    }
}
