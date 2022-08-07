using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage;
using UnityEngine.UI;
using TMPro;

namespace GuildSaberProfile.UI.Components
{
    class CustomText : CustomUIComponent
    {
        protected override string m_ViewResourceName => "GuildSaberProfile.UI.Components.Views.CustomText.bsml";

        #region UICOomponents
        [UIComponent("Text")] TextMeshProUGUI m_CText = null;
        [UIComponent("Horizontal")] VerticalLayoutGroup m_CHorizontal;
        #endregion

        #region properties
        public int m_FontSize { get; set; }
        public float AnchorPosX { get; set; }
        public string m_Text { get; set; }
        public TextAlignmentOptions m_Alignment { get; set; }
        #endregion

        public override void PostCreate()
        {
            m_CText.fontSize = m_FontSize;
            m_CText.text = m_Text;
            m_CText.alignment = m_Alignment;
        }
    }
}
