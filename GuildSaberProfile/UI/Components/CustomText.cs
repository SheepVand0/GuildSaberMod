using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace GuildSaberProfile.UI.Components
{
    class CustomText : CustomUIComponent
    {
        protected override string m_ViewResourceName => "GuildSaberProfile.UI.Components.Views.CustomText.bsml";

        #region UIComponents
        [UIComponent("Text")] TextMeshProUGUI m_CText = null;
        [UIComponent("Horizontal")] HorizontalLayoutGroup m_CHorizontal = null;
        #endregion

        #region properties
        public int FontSize { get; set; }
        public float AnchorPosX { get; set; }
        public string Text { get; set; }
        public TextAlignmentOptions Alignment { get; set; }
        public TextAnchor LayoutAlignment { get; set; }
        #endregion

        public override void PostCreate()
        {
            m_CText.fontSize = FontSize;
            m_CText.text = Text;
            m_CText.alignment = Alignment;
            m_CHorizontal.childAlignment = LayoutAlignment;
        }

    }
}
