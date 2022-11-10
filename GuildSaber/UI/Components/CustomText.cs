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

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent("Text")] TextMeshProUGUI m_CText = null;
        [UIComponent("Horizontal")] HorizontalLayoutGroup m_CHorizontal = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public float FontSize { get; set; } = 0f;
        public float AnchorPosX { get; set; } = 0f;
        public string Text { get; set; } = string.Empty;
        public bool EnableRichText { get; set; } = false;
        public string RichText { get; set; } = string.Empty;
        public TextAlignmentOptions Alignment { get; set; } = TextAlignmentOptions.Center;
        public TextAnchor LayoutAlignment { get; set; } = TextAnchor.MiddleCenter;
        public Color Color { get; set; } = Color.clear;
        public bool Italic { get; set; } = false;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On Post Create
        /// </summary>
        protected override void PostCreate()
        {
            m_CText.fontSize = FontSize;
            m_CText.text = $"{(EnableRichText ? RichText : string.Empty)}{Text}";
            m_CText.alignment = Alignment;
            m_CText.color = Color;
            m_CText.overflowMode = TextOverflowModes.Overflow;
            if (Italic)
                m_CText.fontStyle = FontStyles.Italic;
            m_CHorizontal.childAlignment = LayoutAlignment;
        }

        /// <summary>
        /// Set Text
        /// </summary>
        /// <param name="p_Text"></param>
        public void SetText(string p_Text)
        {
            Text = p_Text;
            m_CText.text = p_Text;
        }

        /// <summary>
        /// Set Color
        /// </summary>
        /// <param name="p_Color"></param>
        public void SetColor(Color p_Color)
        {
            m_CText.richText = EnableRichText;
            m_CText.color = p_Color;
        }
    }
}
