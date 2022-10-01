using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace GuildSaber.UI.Components
{
    class CustomText : CustomUIComponent
    {
        protected override string m_ViewResourceName => "GuildSaber.UI.Components.Views.CustomText.bsml";

        #region UIComponents
        [UIComponent("Text")] TextMeshProUGUI m_CText = null;
        [UIComponent("Horizontal")] HorizontalLayoutGroup m_CHorizontal = null;
        #endregion

        #region properties
        public float FontSize { get; set; }
        public float AnchorPosX { get; set; }
        public string Text { get => string.Empty; set { } }
        public string RichText { get; set; }
        public TextAlignmentOptions Alignment { get; set; }
        public TextAnchor LayoutAlignment { get; set; }
        public Color Color { get; set; }
        public bool Italic { get; set; }
        #endregion

        protected override void PostCreate()
        {
            m_CText.fontSize = FontSize;
            m_CText.text = $"{RichText}{Text}";
            m_CText.alignment = Alignment;
            m_CText.color = Color;
            m_CText.overflowMode = TextOverflowModes.Overflow;
            if (Italic)
                m_CText.fontStyle = FontStyles.Italic;
            m_CHorizontal.childAlignment = LayoutAlignment;
        }

        public void SetText(string p_Text)
        {
            m_CText.text = p_Text;
        }

        public void SetColor(Color p_Color)
        {
            m_CText.richText = true;
            m_CText.color = p_Color;
        }
    }
}
