using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaber.API;
using GuildSaber.Logger;
using GuildSaber.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildSaber.UI.Components
{

    internal class CustomLevelStatsView : CustomUIComponent
    {
        protected override string m_ViewResourceName => "GuildSaber.UI.Components.Views.CustomLevelStatsView.bsml";

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent("MainHorizontal")] HorizontalLayoutGroup m_MainHorizontal = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent("Rank")]
        private TextMeshProUGUI m_CRank = null;
        [UIComponent("Name")]
        private TextMeshProUGUI m_CName = null;
        [UIComponent("PassState")]
        private TextMeshProUGUI m_CPassState = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override void AfterViewCreation()
        {
            m_CRank.fontSize = LeaderboardScoreCell.ScoreFontSize;
            m_CName.fontStyle = FontStyles.Italic;
            m_CName.fontSize = LeaderboardScoreCell.ScoreFontSize;
            m_CPassState.richText = true;
            m_CPassState.fontSize = LeaderboardScoreCell.ScoreFontSize;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void Init(int p_Rank, string p_Name, API.PassState.EState p_State)
        {
            m_CRank.SetText($"#{p_Rank}");
            m_CName.SetText(p_Name);
            m_CPassState.SetText($"State : <color=#{PassState.GetColorFromPassState(p_State)}>{p_State}</color>");
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void Clear()
        {
            m_CRank.SetText(string.Empty);
            m_CName.SetText(string.Empty);
            m_CPassState.SetText(string.Empty);
        }
    }
}
