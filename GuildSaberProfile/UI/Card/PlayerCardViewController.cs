using System.Collections.Generic;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using GuildSaberProfile.API;
using GuildSaberProfile.Configuration;
using GuildSaberProfile.Time;
using GuildSaberProfile.Utils;
using GuildSaberProfile.Utils.Color;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using IPA.Utilities;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace GuildSaberProfile.UI.Card;
//PlayerCard variables
[HotReload(RelativePathToLayout = @"PlayerCard_UI.bsml")]
[ViewDefinition("GuildSaberProfile.UI.Card.View.PlayerCard_UI.bsml")]
public class PlayerCardViewController : BSMLAutomaticViewController
{
    #region UIComponents
    [UIComponent("PlayerNameText")] public TextMeshProUGUI m_PlayerNameText;
    [UIComponent("DetailsLevelsLayout")] private readonly GridLayoutGroup m_DetailsLevelsLayout = null;
    [UIComponent("ElemGrid")] private readonly GridLayoutGroup m_ElementsGrid = null;
    [UIComponent("NeonBackground")] private readonly Transform m_NeonBackground = null;
    [UIComponent("PlayTimeText")] private readonly TextMeshProUGUI m_PlayTimeText = null;
    [UIComponent("RankUIVertical")] private readonly VerticalLayoutGroup m_RankUIVertical = null;
    #endregion

    [UIValue("AvailableGuilds")]
    public List<object> m_AvailableGuilds = new List<object>();

    #region Properties
    [CanBeNull]
    public string PlayerName
    {
        get => PlayerCardUI.m_Player.Name;
        set { }
    }
    public string PlayerGlobalLevel
    {
        get => PlayerCardUI.m_Player.Level.ToString();
        set { }
    }
    public string PlayerImageSrc
    {
        get => PlayerCardUI.m_Player.ProfilePicture;
        set { }
    }
    public string PlayerNumberOfPasses
    {
        get => PlayerCardUI.m_Player.PassCount.ToString();
        set { }
    }
    // ReSharper disable once CollectionNeverQueried.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public List<PlayerRankUI> Ranks = new List<PlayerRankUI>();
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public List<PlayerLevelUI> Levels = new List<PlayerLevelUI>();
    public FloatingScreen m_CardScreen;
    public bool AllowCustomCardColors = false;
    #endregion

    [UIAction("#post-parse")]
    public void PostParse()
    {
        if (PlayerCardUI.m_Player.Equals(null)) return;

        Refresh();

        Plugin.Log.Info("Card loaded");
    }

    #region Main Card Info and style Loading


    public void UpdateCardColor()
    {
        if (AllowCustomCardColors == true)
            PlayerCardUI.m_Player.ProfileColor = PluginConfig.Instance.CustomColor.ToGSProfileColor();

        UnityEngine.Color l_PlayerColor = PlayerCardUI.m_Player.ProfileColor.ToUnityColor();
        UnityEngine.Color l_BeforePlayerColor = new UnityEngine.Color(l_PlayerColor.r * 0.8f, l_PlayerColor.g * 0.8f, l_PlayerColor.b * 0.8f);
        UnityEngine.Color l_NewPlayerColor = new UnityEngine.Color(l_PlayerColor.r * 1.2f, l_PlayerColor.g * 1.2f, l_PlayerColor.b * 1.2f);

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
    }

    [UIAction("OnPPClick")]
    private void OnPPClick()
    {
        //If player disabled settings modal don't showing
        if (!PluginConfig.Instance.ShowSettingsModal) return;

        UpdateShowPlayerCustomColorUISetting();
        if (PluginConfig.Instance.ShowSettingsModal)
            ShowSettings();
    }
    #endregion

    #region References
    public void SetReferences(PlayerApiReworkOutput p_Player, FloatingScreen p_CardScreen)
    {
        PlayerCardUI.m_Player = p_Player;
        m_CardScreen = p_CardScreen;
    }
    #endregion

    #region Other
    public void UpdateCanPlayerUseCustomColors()
    {
        string l_BestCategory = string.Empty;

        //Checking if Player level is at 97% of max level
        if (PlayerCardUI.m_Player.CategoryData.Count > 0)
            foreach (var l_Current in PlayerCardUI.m_Player.CategoryData)
                if (l_Current.Level == PlayerCardUI.m_Player.Level)
                    l_BestCategory = l_Current.Category;
                else
                    l_BestCategory = string.Empty;
        else
            l_BestCategory = null;

        int l_MaxLevel = GuildSaberUtils.GetStaticPlayerLevel(l_BestCategory);
        AllowCustomCardColors = PlayerCardUI.m_Player.Level >= (int)(l_MaxLevel * 0.97f);
    }
    #endregion

    #region Card Updates
    public void Refresh()
    {
        foreach (PlayerRankUI l_Current in Ranks)
            GameObject.DestroyImmediate(l_Current.gameObject, true);

        foreach (PlayerLevelUI l_Current in Levels)
            GameObject.DestroyImmediate(l_Current.gameObject, true);

        Ranks.Clear();
        Levels.Clear();

        foreach (CustomApiPlayerCategory l_Cat in PlayerCardUI.m_Player.CategoryData)
        {
            int l_FontSize = (int)(2 / (PlayerCardUI.m_Player.CategoryData.Count * 0.11f));
            if (l_FontSize < 1) l_FontSize = 2;
            if (l_FontSize == 5) l_FontSize = 4;

             List<ItemParam> l_Params = new List<ItemParam>()
                {
                    new ItemParam("Level", l_Cat.Level.ToString()),
                    new ItemParam("LevelName", l_Cat.Category),
                    new ItemParam("FontSize", l_FontSize)
                };
            PlayerLevelUI l_Temp = CustomUIComponent.CreateItemWithParams<PlayerLevelUI>(m_DetailsLevelsLayout.transform, true, true, l_Params);
            Levels.Add(l_Temp);
        }

        foreach (RankData l_Rank in PlayerCardUI.m_Player.RankData)
        {
            List<ItemParam> l_Params = new List<ItemParam>()
                {
                    new ItemParam("PointsName", l_Rank.PointsName),
                    new ItemParam("PlayerRank", l_Rank.Rank.ToString())
                };
            PlayerRankUI l_Temp = CustomUIComponent.CreateItemWithParams<PlayerRankUI>(m_RankUIVertical.transform, true, true, l_Params);
            Ranks.Add(l_Temp);
        }

        Plugin.Log.Info($"{PlayerCardUI.m_Player.Name}");
        m_PlayerNameText.text = GuildSaberUtils.GetPlayerNameToFit(PlayerCardUI.m_Player.Name, 16);
        UpdateCanPlayerUseCustomColors();
        UpdateCardColor();
        UpdateLevelsDetails();
    }

    public void UpdateLevelsDetails()
    {
        bool l_ShowDetaislLevels = PluginConfig.Instance.ShowDetailsLevels;
        if (Levels.Count == 0) l_ShowDetaislLevels = false;
        m_DetailsLevelsLayout.gameObject.SetActive(l_ShowDetaislLevels);
        if (m_CardScreen == null) return;
        float l_LevelsSize = Levels.Count;
        if (l_ShowDetaislLevels)
        {
            //When the details levels is visible
            m_CardScreen.ScreenSize = new Vector2((68 + PlayerCardUI.m_Player.Name.Length * 1.2f + l_LevelsSize) * 0.9f, 28 + l_LevelsSize * 0.65f + Ranks.Count * 2);
            m_ElementsGrid.cellSize = new Vector2((40 + PlayerCardUI.m_Player.Name.Length + l_LevelsSize) * 1.1f, 40);
            m_DetailsLevelsLayout.cellSize = new Vector2(12 - l_LevelsSize * 0.1f, 10.5f - l_LevelsSize * 0.1f);
            m_ElementsGrid.spacing = new Vector2(7, 7);
        }
        else
        {
            //When the details levels is hidden
            m_CardScreen.ScreenSize = new Vector2(33 + PlayerCardUI.m_Player.Name.Length, 28 + Ranks.Count * 2);
            m_ElementsGrid.cellSize = new Vector2(25 + PlayerCardUI.m_Player.Name.Length, 40);
            m_ElementsGrid.spacing = new Vector2(1, 7);
        }
    }

    public void UpdateTime(OptimizedDateTime p_Time)
    {
        //Let's go
        m_PlayTimeText.text = PluginConfig.Instance.ShowPlayTime ? string.Join(":", p_Time.Hours.ToString("00"), p_Time.Minutes.ToString("00"), p_Time.Seconds.ToString("00")) : " ";
    }

    #endregion

    #region Settings
    #region UIComponents
    [UIComponent("ToggleShowHandle")] private readonly ToggleSetting m_ToggleShowHandle = null;
    [UIComponent("CustomColorSettings")] private readonly ColorSetting m_CustomColorSettings = null;
    [UIComponent("SettingsModal")] public ModalView m_ModalView = null;
    [UIComponent("SettingsGuildSelector")] public DropDownListSetting m_GuildSelector = null;
    #endregion

    #region Updates
    public void UpdateShowPlayerCustomColorUISetting()
    {
        m_CustomColorSettings.interactable = AllowCustomCardColors;
        m_CustomColorSettings.gameObject.SetActive(AllowCustomCardColors);

        RectTransform l_Rect = m_ModalView.GetComponent<RectTransform>();

        if (AllowCustomCardColors)
            l_Rect.sizeDelta = new Vector2(100, 70);
        else
            l_Rect.sizeDelta = new Vector2(100, 60);
    }

    public void UpdateToggleCardHandleUISettingVisibility()
    {
        if (Plugin.CurrentSceneName == "GameCore")
            m_ToggleShowHandle.gameObject.SetActive(PluginConfig.Instance.CardHandleVisible);
        else
            m_ToggleShowHandle.gameObject.SetActive(true);
    }
    #endregion

    #region Show
    public void ShowSettings()
    {
        m_ModalView.Show(true, false, null);
    }
    #endregion

    #region UIValues
    [UIValue("SelectedGuild")]
    protected string SelectedGuild
    {
        get => PluginConfig.Instance.SelectedGuild;
        set { }
    }

    [UIValue("ShowCardHandle")]
    protected bool ShowCardHandle
    {
        get => PluginConfig.Instance.CardHandleVisible;
        set
        {
            PluginConfig.Instance.CardHandleVisible = value;
            PlayerCardUI.m_Instance.UpdateCardHandleVisibility();
            UpdateToggleCardHandleUISettingVisibility();
        }
    }

    [UIValue("DetailLevels")]
    protected bool ShowDetailedLevels
    {
        get => PluginConfig.Instance.ShowDetailsLevels;
        set
        {
            PluginConfig.Instance.ShowDetailsLevels = value;
            PlayerCardUI.m_Instance.CardViewController.UpdateLevelsDetails();
        }
    }

    [UIValue("ShowPlayTime")]
    protected bool ShowPlayTime
    {
        get => PluginConfig.Instance.ShowPlayTime;
        set => PluginConfig.Instance.ShowPlayTime = value;
    }

    [UIValue("CustomColor")]
    protected UnityEngine.Color CustomColor
    {
        get => PluginConfig.Instance.CustomColor;
        set
        {
            PluginConfig.Instance.CustomColor = value;
            UpdateCardColor();
        }
    }

    #endregion

    #region UIActions
    [UIAction("RefreshCard")]
    protected void OnButtonRefreshCardClicked()
    {
        PlayerCardUI.RefreshCard(true);
    }

    [UIAction("UpdateCard")]
    private void UpdateCard(string p_Selected)
    {
        PluginConfig.Instance.SelectedGuild = p_Selected;
        OnButtonRefreshCardClicked();
    }

    [UIAction("ResetPosMenu")]
    private void ResetPosMenu()
    {
        PlayerCardUI.ResetMenuCardPosition();
    }

    [UIAction("ResetPosGame")]
    private void ResetPosInGame()
    {
        PlayerCardUI.ResetInGameCardPosition();
    }

    #endregion
    #endregion
}