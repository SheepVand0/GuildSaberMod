using BeatSaberMarkupLanguage.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildSaber.UI.Leaderboard.Components;

internal class CustomText : CustomUIComponent
{
    [UIComponent("Horizontal")] private readonly HorizontalLayoutGroup m_CHorizontal = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [UIComponent("Text")] private readonly TextMeshProUGUI m_CText = null;
    protected override string ViewResourceName
    {
        get => "GuildSaber.UI.Leaderboard.Components.Views.CustomText.bsml";
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public float FontSize { get; set; }
    public float AnchorPosX { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool EnableRichText { get; set; }
    public string RichText { get; set; } = string.Empty;
    public TextAlignmentOptions Alignment { get; set; } = TextAlignmentOptions.Center;
    public TextAnchor LayoutAlignment { get; set; } = TextAnchor.MiddleCenter;
    public Color Color { get; set; } = Color.clear;
    public bool Italic { get; set; }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     On Post Create
    /// </summary>
    protected override void PostCreate()
    {
        m_CText.fontSize = FontSize;
        m_CText.richText = EnableRichText;
        m_CText.text = $"{(EnableRichText ? RichText : string.Empty)}{Text}";
        m_CText.alignment = Alignment;
        m_CText.color = Color;
        m_CText.overflowMode = TextOverflowModes.Overflow;
        if (Italic) m_CText.fontStyle = FontStyles.Italic;
        m_CHorizontal.childAlignment = LayoutAlignment;
    }

    /// <summary>
    ///     Set Text
    /// </summary>
    /// <param name="p_Text"></param>
    public void SetText(string p_Text)
    {
        Text = p_Text;
        m_CText.text = p_Text;
    }

    /// <summary>
    ///     Set Color
    /// </summary>
    /// <param name="p_Color"></param>
    public void SetColor(Color p_Color)
    {
        m_CText.richText = EnableRichText;
        m_CText.color = p_Color;
    }

    /*public void SetEnableRichText(bool p_RichText)
    {
        EnableRichText = p_RichText;
        m_CText.richText = p_RichText;
    }*/
}