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
<<<<<<< Updated upstream
=======
using JetBrains.Annotations;
>>>>>>> Stashed changes
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CSProfile.UI.Card;

#region ViewController

//PlayerCard variables
[HotReload(RelativePathToLayout = @"PlayerCard_UI.bsml")]
[ViewDefinition("CSProfile.UI.Card.View.PlayerCard_UI.bsml")]
<<<<<<< Updated upstream
public partial class PlayerCardViewController : BSMLAutomaticViewController
=======
public class PlayerCardViewController : BSMLAutomaticViewController
>>>>>>> Stashed changes
{
    public TimeManager m_TimeManager;

    public FloatingScreen m_CardScreen;

<<<<<<< Updated upstream
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
=======
    [UIComponent("PlayerNameText")] public TextMeshProUGUI m_PlayerNameText;

    public List<PlayerLevelUI> Levels = new List<PlayerLevelUI>();
    [UIComponent("DetailsLevelsLayout")] private readonly GridLayoutGroup m_DetailsLevelsLayout = null;
    [UIComponent("ElemGrid")] private readonly GridLayoutGroup m_ElementsGrid = null;
    [UIComponent("NeonBackground")] private readonly Transform m_NeonBackground = null;

    private int m_NumberOfPasses;

    private PlayerApiReworkOutput m_PlayerInfo;
    [UIComponent("PlayerRankText")] private readonly TextMeshProUGUI m_PlayerRankText = null;
    [UIComponent("PlayTimeText")] private readonly TextMeshProUGUI m_PlayTimeText = null;
    [CanBeNull]
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
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
=======

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

        Color l_DivideColor = l_CurrentImageView.color;
        l_CurrentImageView.color0 = l_BeforePlayerColor.ColorWithAlpha(1f);
        l_CurrentImageView.color1 = l_NewPlayerColor.ColorWithAlpha(1f);
        l_CurrentImageView.color = l_PlayerColor.ColorWithAlpha(1f);

        l_CurrentImageView.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;

        Plugin.Log.Info("Card loaded");
>>>>>>> Stashed changes
    }

    #endregion

    #region References

    public void SetReferences(PlayerApiReworkOutput p_Player, FloatingScreen p_CardScreen)
    {
        m_PlayerInfo = p_Player;
        m_CardScreen = p_CardScreen;
    }

    #endregion
<<<<<<< Updated upstream

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
=======

    #region Card Updates

    public void UpdateLevelsDetails()
    {
        m_DetailsLevelsLayout.gameObject.SetActive(PluginConfig.Instance.ShowDetailsLevels);

        if (m_CardScreen == null)
            return;

        float l_LevelsSize = Levels.Count;
        if (PluginConfig.Instance.ShowDetailsLevels)
        {
            //When the details levels is visible
            m_CardScreen.ScreenSize = new Vector2((62 + m_PlayerInfo.Name.Length + l_LevelsSize) * 0.8f, 28 + l_LevelsSize * 0.4f);
            m_ElementsGrid.cellSize = new Vector2(35 + m_PlayerInfo.Name.Length + l_LevelsSize * 0.9f, 40);
            m_DetailsLevelsLayout.cellSize = new Vector2(7, 10.5f - l_LevelsSize * 0.1f);
>>>>>>> Stashed changes
            m_ElementsGrid.spacing = new Vector2(7, 7);
        }
        else
        {
            //When the details levels is hidden
<<<<<<< Updated upstream
            m_CardScreen.ScreenSize = new Vector2(38, 28);
            m_ElementsGrid.cellSize = new Vector2(30, 40);
=======
            m_CardScreen.ScreenSize = new Vector2(33 + m_PlayerInfo.Name.Length, 28);
            m_ElementsGrid.cellSize = new Vector2(25 + m_PlayerInfo.Name.Length, 40);
>>>>>>> Stashed changes
            m_ElementsGrid.spacing = new Vector2(1, 7);
        }
    }

    public void UpdateTime(OptimizedDateTime p_Time)
    {
<<<<<<< Updated upstream
        m_PlayTimeText.text = PluginConfig.Instance.ShowPlayTime ? string.Join(":", p_Time.m_Hours.ToString("00"), p_Time.m_Minutes.ToString("00"), p_Time.m_Seconds.ToString("00")) : " ";
=======
        m_PlayTimeText.text = PluginConfig.Instance.ShowPlayTime ? string.Join(":", p_Time.Hours.ToString("00"), p_Time.Minutes.ToString("00"), p_Time.Seconds.ToString("00")) : " ";
>>>>>>> Stashed changes
    }

    #endregion

}

#endregion
