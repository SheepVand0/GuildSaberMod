using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using UnityEngine.UI;

namespace GuildSaber.UI.Components
{

    internal class CustomLevelStatsView : CustomUIComponent
    {
        protected override string m_ViewResourceName => "GuildSaber.UI.Components.View.CustomLevelStatsView.bsml";

        [UIComponent("MainHorizontal")] HorizontalLayoutGroup m_MainHorizontal = null;

        protected override void AfterViewCreation()
        {

        }

    }
}
