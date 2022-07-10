using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using GuildSaberProfile.API;
using GuildSaberProfile.Configuration;
using GuildSaberProfile.Time;
using HMUI;
using IPA.Utilities;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildSaberProfile.UI.Card;

#region ViewController

//PlayerCard variables
[HotReload(RelativePathToLayout = @"PlayerCard_UI.bsml")]
[ViewDefinition("GuildSaberProfile.UI.Card.View.PlayerCard_UI.bsml")]
public class PlayerCardViewController : BSMLAutomaticViewController
{
    public TimeManager m_TimeManager;

    public FloatingScreen m_CardScreen;

    [UIComponent("PlayerNameText")] public TextMeshProUGUI m_PlayerNameText;
    [UIComponent("DetailsLevelsLayout")] private readonly GridLayoutGroup m_DetailsLevelsLayout = null;
    [UIComponent("ElemGrid")] private readonly GridLayoutGroup m_ElementsGrid = null;
    [UIComponent("NeonBackground")] private readonly Transform m_NeonBackground = null;
    [UIComponent("ErrorText")] private readonly TextMeshProUGUI m_ErrorText = null;

    [UIComponent("PlayTimeText")] private readonly TextMeshProUGUI m_PlayTimeText = null;

    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public List<PlayerLevelUI> Levels = new List<PlayerLevelUI>();

    // ReSharper disable once CollectionNeverQueried.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public List<PlayerRankUI> Ranks = new List<PlayerRankUI>();

    private int m_NumberOfPasses;

    private PlayerApiReworkOutput m_PlayerInfo;
    [CanBeNull]
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

    #region Main Card Info and style Loading

    [UIAction("#post-parse")]
    public void PostParse()
    {
        if (m_PlayerInfo.Equals(null)) return;

        Color l_PlayerColor = m_PlayerInfo.ProfileColor.ToUnityColor();
        Color l_BeforePlayerColor = new Color(l_PlayerColor.r * 0.8f, l_PlayerColor.g * 0.8f, l_PlayerColor.b * 0.8f);
        Color l_NewPlayerColor = new Color(l_PlayerColor.r * 1.2f, l_PlayerColor.g * 1.2f, l_PlayerColor.b * 1.2f);

        m_PlayerNameText.enableVertexGradient = true;

        m_PlayerNameText.colorGradient = new VertexGradient(l_BeforePlayerColor, l_BeforePlayerColor, l_NewPlayerColor, l_NewPlayerColor);

        ImageView l_CurrentImageView = m_NeonBackground.GetComponentInChildren<ImageView>();

        l_CurrentImageView.SetField("_skew", 0.0f);
        l_CurrentImageView.overrideSprite = null;
        l_CurrentImageView.SetImage("#RoundRect10BorderFade");

        l_CurrentImageView.color0 = l_BeforePlayerColor.ColorWithAlpha(1f);
        l_CurrentImageView.color1 = l_NewPlayerColor.ColorWithAlpha(1f);
        l_CurrentImageView.color = l_PlayerColor.ColorWithAlpha(1f);

        l_CurrentImageView.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;

        m_TimeManager = gameObject.AddComponent<TimeManager>();
        m_TimeManager.SetPlayerCardViewControllerRef(this);

        Plugin.Log.Info("Card loaded");
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

        float l_LevelsSize = Levels.Count;
        if (PluginConfig.Instance.ShowDetailsLevels)
        {
            //When the details levels is visible
            m_CardScreen.ScreenSize = new Vector2((68 + m_PlayerInfo.Name.Length + l_LevelsSize)*0.9f, 28 + l_LevelsSize * 0.6f);
            m_ElementsGrid.cellSize = new Vector2((40 + m_PlayerInfo.Name.Length + l_LevelsSize)*1.1f, 40);
            m_DetailsLevelsLayout.cellSize = new Vector2(12 - l_LevelsSize*0.1f, 10.5f - l_LevelsSize * 0.1f);
            m_ElementsGrid.spacing = new Vector2(7, 7);
        }
        else
        {
            //When the details levels is hidden
            m_CardScreen.ScreenSize = new Vector2(33 + m_PlayerInfo.Name.Length, 28);
            m_ElementsGrid.cellSize = new Vector2(25 + m_PlayerInfo.Name.Length, 40);
            m_ElementsGrid.spacing = new Vector2(1, 7);
        }
    }

    public void UpdateTime(OptimizedDateTime p_Time)
    {
        m_PlayTimeText.text = PluginConfig.Instance.ShowPlayTime ? string.Join(":", p_Time.Hours.ToString("00"), p_Time.Minutes.ToString("00"), p_Time.Seconds.ToString("00")) : " ";
    }
    #endregion
}
#endregion
