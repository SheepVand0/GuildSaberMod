using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using CSProfile.API;
using CSProfile.Configuration;
using CSProfile.Time;
using HMUI;
using IPA.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CSProfile.UI.Card;

#region ViewController

//PlayerCard variables
[HotReload(RelativePathToLayout = @"PlayerCard_UI.bsml")]
[ViewDefinition("CSProfile.UI.Card.View.PlayerCard_UI.bsml")]
public partial class PlayerCardViewController : BSMLAutomaticViewController
{
    public TimeManager m_TimeManager;

    public FloatingScreen m_CardScreen;

    public int m_NumberOfPasses;

    [UIComponent("PlayerNameText")] public TextMeshProUGUI m_PlayerNameText;
    [UIComponent("PlayerRankText")] public TextMeshProUGUI m_PlayerRankText;
    [UIComponent("PlayTimeText")] public TextMeshProUGUI m_PlayTimeText;
    [UIComponent("DetailsLevelsLayout")] public VerticalLayoutGroup m_DetailsLevelsLayout;
    [UIComponent("NeonBackground")] public Transform m_NeonBackground;
    [UIComponent("ElemGrid")] public GridLayoutGroup m_ElementsGrid;

    public List<PlayerLevelUI> FirstLineLevels = new List<PlayerLevelUI>();

    private PlayerApiReworkOutput m_PlayerInfo;

    public List<PlayerLevelUI> SecondLineLevels = new List<PlayerLevelUI>();
    public string PlayerName
    {
        get => m_PlayerInfo.Name;
        set { }
    }
    public string PlayerGlobalLevel
    {
        get => m_PlayerInfo.Level.ToString();
        set { }
    }
    public string PlayerImageSrc
    {
        get => m_PlayerInfo.ProfilePicture;
        set { }
    }
    public string PlayerNumberOfPasses
    {
        get
        {
            m_NumberOfPasses = 0;
            for (int l_I = 0; l_I < m_PlayerInfo.CategoryData.Count; l_I++)
                m_NumberOfPasses += m_PlayerInfo.CategoryData[l_I].NumberOfPass;
            PlayerNumberOfPasses = m_NumberOfPasses.ToString();
            return m_NumberOfPasses.ToString();
        }
        set { }
    }
}

//PlayerCard Functions
public partial class PlayerCardViewController : BSMLAutomaticViewController
{

    #region Main Card Info and style Loading

    [UIAction("#post-parse")]
    public void PostParse()
    {
        m_PlayerRankText.SetText(m_PlayerInfo.RankData[0].Rank.ToString());

        Color l_PlayerColor = m_PlayerInfo.ProfileColor.ToUnityColor();
        Color l_BeforePlayerColor = new Color(l_PlayerColor.r * 0.8f, l_PlayerColor.g * 0.8f, l_PlayerColor.b * 0.8f);
        Color l_NewPlayerColor = new Color(l_PlayerColor.r * 1.2f, l_PlayerColor.g * 1.2f, l_PlayerColor.b * 1.2f);

        m_PlayerNameText.enableVertexGradient = true;
        m_PlayerRankText.enableVertexGradient = true;
        m_PlayerNameText.colorGradient = new VertexGradient(l_BeforePlayerColor, l_BeforePlayerColor, l_NewPlayerColor, l_NewPlayerColor);
        m_PlayerRankText.colorGradient = new VertexGradient(l_BeforePlayerColor, l_BeforePlayerColor, l_NewPlayerColor, l_NewPlayerColor);

        m_TimeManager = gameObject.AddComponent<TimeManager>();
        m_TimeManager.SetPlayerCardViewControllerRef(this);

        ImageView l_CurrentImageView = m_NeonBackground.GetComponentInChildren<ImageView>();

        l_CurrentImageView.SetField("_skew", 0.0f);
        l_CurrentImageView.overrideSprite = null;
        l_CurrentImageView.SetImage("#RoundRect10BorderFade");

        //UnityEngine.Color l_DivideColor = l_currentImageView.color;
        l_CurrentImageView.color0 = l_BeforePlayerColor.ColorWithAlpha(1f);
        l_CurrentImageView.color1 = l_NewPlayerColor.ColorWithAlpha(1f);
        l_CurrentImageView.color = l_PlayerColor.ColorWithAlpha(1f);

        l_CurrentImageView.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
    }

    #endregion

    #region References

    public void SetReferences(PlayerApiReworkOutput p_Player, FloatingScreen p_CardScreen)
    {
        m_PlayerInfo = p_Player;
        m_CardScreen = p_CardScreen;
    }

    #endregion

    #region Card Updates

    public void UpdateLevelsDetails()
    {
        m_DetailsLevelsLayout.gameObject.SetActive(PluginConfig.Instance.ShowDetailsLevels);

        if (m_CardScreen == null)
            return;

        if (PluginConfig.Instance.ShowDetailsLevels)
        {
            //When the details levels is visible
            m_CardScreen.ScreenSize = new Vector2(64, 28);
            m_ElementsGrid.cellSize = new Vector2(49, 40);
            m_ElementsGrid.spacing = new Vector2(7, 7);
        }
        else
        {
            //When the details levels is hidden
            m_CardScreen.ScreenSize = new Vector2(38, 28);
            m_ElementsGrid.cellSize = new Vector2(30, 40);
            m_ElementsGrid.spacing = new Vector2(1, 7);
        }
    }

    public void UpdateTime(OptimizedDateTime p_Time)
    {
        m_PlayTimeText.text = PluginConfig.Instance.ShowPlayTime ? string.Join(":", p_Time.m_Hours.ToString("00"), p_Time.m_Minutes.ToString("00"), p_Time.m_Seconds.ToString("00")) : " ";
    }

    #endregion

}

#endregion
