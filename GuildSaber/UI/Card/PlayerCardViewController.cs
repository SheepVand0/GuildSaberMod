﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using GuildSaber.API;
using GuildSaber.Configuration;
using GuildSaber.Time;
using GuildSaber.Utils;
using GuildSaber.Utils.Color;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using IPA.Utilities;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using GuildSaber.BSPModule;

namespace GuildSaber.UI.Card;
//PlayerCard variables
[HotReload(RelativePathToLayout = @"PlayerCard_UI.bsml")]
[ViewDefinition("GuildSaber.UI.Card.View.PlayerCard_UI.bsml")]
internal class PlayerCardViewController : BSMLAutomaticViewController
{
    [UIComponent("PlayerNameText")] public TextMeshProUGUI m_PlayerNameText;
    [UIComponent("DetailsLevelsLayout")] private GridLayoutGroup m_DetailsLevelsLayout = null;
    [UIComponent("ElemGrid")] private GridLayoutGroup m_ElementsGrid = null;
    [UIComponent("NeonBackground")] private Transform m_NeonBackground = null;
    [UIComponent("PlayTimeText")] private TextMeshProUGUI m_PlayTimeText = null;
    [UIComponent("RankUIVertical")] private VerticalLayoutGroup m_RankUIVertical = null;
    [UIComponent("PlayerNumberOfPasses")] private TextMeshProUGUI m_PlayerNumberOfPasses = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [UIValue("AvailableGuilds")]
    private List<object> m_DropdownAvailableGuilds = new List<object>() { "UwU" };

    [CanBeNull]
    public string PlayerName
    {
        get => PlayerCardUI.m_Player.Name;
        set { }
    }
    public string PlayerGlobalLevel
    {
        get => PlayerCardUI.m_Player.LevelValue.ToString();
        set { }
    }
    public string PlayerImageSrc
    {
        get => PlayerCardUI.m_Player.Avatar;
        set { }
    }
    public string PlayerNumberOfPasses
    {
        get => PlayerCardUI.m_Player.GuildValidPassCount.ToString();
        set { }
    }

    // ReSharper disable once CollectionNeverQueried.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public List<PlayerRankUI> Ranks = new List<PlayerRankUI>();
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public List<PlayerLevelUI> Levels = new List<PlayerLevelUI>();
    public FloatingScreen m_CardScreen;
    public bool AllowCustomCardColors = false;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Post parse
    /// </summary>
    [UIAction("#post-parse")]
    public void PostParse()
    {
        if (PlayerCardUI.m_Player.Equals(null)) return;

        m_DropdownAvailableGuilds.Clear();
        foreach (GuildData l_Current in Plugin.AvailableGuilds)
        {
            m_DropdownAvailableGuilds.Add(l_Current.Name);
        }
        m_GuildSelector.UpdateChoices();

        m_GuildSelector.Value = GuildSaberUtils.GetGuildFromId(GSConfig.Instance.SelectedGuild).Name;
        m_GuildSelector.ApplyValue();

        BSPModule.GuildSaberModule.m_CardSelectedGuild = GuildSaberUtils.GetGuildFromId(GSConfig.Instance.SelectedGuild);

        Refresh();

        Plugin.Log.Debug("Card loaded");
    }

    ////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Update colors
    /// </summary>
    public void UpdateCardColor()
    {
        if (AllowCustomCardColors == true)
            PlayerCardUI.m_Player.Color = GSConfig.Instance.CustomColor.ToGSProfileColor();

        UnityEngine.Color l_PlayerColor = PlayerCardUI.m_Player.Color.ToUnityColor32();
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

    /// <summary>
    /// When click on player image
    /// </summary>
    [UIAction("OnPPClick")]
    private void OnPPClick()
    {
        //If player disabled settings modal don't showing
        if (!GSConfig.Instance.ShowSettingsModal) return;

        UpdateShowPlayerCustomColorUISetting();
        if (GSConfig.Instance.ShowSettingsModal)
            ShowSettings();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Set card screen reference
    /// </summary>
    /// <param name="p_CardScreen"></param>
    public void SetReferences(FloatingScreen p_CardScreen)
    {
        m_CardScreen = p_CardScreen;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Check if player have required level to choose is own colors
    /// </summary>
    public void UpdateCanPlayerUseCustomColors()
    {
        float l_MaxLevel = 0;

        //Checking if Player level is at 97% of max level
        if (PlayerCardUI.m_Player.CategoryData.Count > 0)
        {
            foreach (var l_Current in PlayerCardUI.m_Player.CategoryData)
            {
                if (l_MaxLevel < l_Current.MaxLevelValue) l_MaxLevel = l_Current.MaxLevelValue;
            }
        }

        AllowCustomCardColors = PlayerCardUI.m_Player.LevelValue >= (float)(l_MaxLevel * 0.97f);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Refresh cards Visuals
    /// </summary>
    public void Refresh()
    {
        try
        {
            ///Checking
            bool l_SetCardActive = !BSPModule.GuildSaberModule.m_CardSelectedGuild.Equals(default(GuildData));
            if (l_SetCardActive)
                PlayerCardUI.SetCardActive(true);
            else
            {
                PlayerCardUI.SetCardActive(false);
                return;
            }

            ///If ranks are empty create a default list
            if (Ranks.Count == 0) for (int l_i = 0; l_i < 3; l_i++)
                    Ranks.Add(CustomUIComponent.CreateItem<PlayerRankUI>(m_RankUIVertical.transform, true, true));

            ///Reset all
            foreach (PlayerRankUI l_Current in Ranks) l_Current.ResetComponent();

            foreach (PlayerLevelUI l_Current in Levels) l_Current.ResetComponent();

            ///Set rank
            int l_RankDataCount = PlayerCardUI.m_Player.RankData.Count;
            for (int l_i = 0; l_i < l_RankDataCount;l_i++)
            {
                RankData l_Rank = PlayerCardUI.m_Player.RankData[l_i];
                Ranks[l_i].SetValues(l_Rank.PointsName, l_Rank.Rank.ToString(), PlayerCardUI.m_Player.Color.ToUnityColor());
            }

            ///Set Levels
            int l_CategoryDataCount = PlayerCardUI.m_Player.CategoryData.Count;
            for (int l_i = 0; l_i < l_CategoryDataCount;l_i++)
            {
                CategoryData l_Cat = PlayerCardUI.m_Player.CategoryData[l_i];
                int l_FontSize = (int)(2 / (l_CategoryDataCount * 0.11f));
                if (l_FontSize < 1) l_FontSize = 2;
                if (l_FontSize == 5) l_FontSize = 4;

                ///Add to list if there is more categories than objects in Levels
                if (l_i > Levels.Count - 1)
                {
                    List<ItemParam> l_Params = new List<ItemParam>()
                        {
                            new ItemParam("Level", l_Cat.LevelValue.ToString("0.0")),
                            new ItemParam("LevelName", l_Cat.CategoryName),
                            new ItemParam("FontSize", l_FontSize)
                        };
                    PlayerLevelUI l_Temp = CustomUIComponent.CreateItemWithParams<PlayerLevelUI>(m_DetailsLevelsLayout.transform, true, true, l_Params);
                    Levels.Add(l_Temp);
                }
                else
                {
                    ///Else just set the value
                    Levels[l_i].SetValues(l_Cat.CategoryName, l_Cat.LevelValue.ToString("0.0"));
                }
            }

            m_PlayerNumberOfPasses.text = PlayerCardUI.m_Player.GuildValidPassCount.ToString();
            m_PlayerNameText.text = GuildSaberUtils.GetPlayerNameToFit(PlayerCardUI.m_Player.Name, 16);
            UpdateCanPlayerUseCustomColors();
            UpdateCardColor();
            UpdateLevelsDetails();

        }
        catch (Exception l_E)
        {
            Plugin.Log.Error(l_E);
        }
    }

    public void UpdateLevelsDetails()
    {
        bool l_ShowDetaislLevels = GSConfig.Instance.ShowDetailsLevels;
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
        m_PlayTimeText.text = GSConfig.Instance.ShowPlayTime ? string.Join(":", p_Time.Hours.ToString("00"), p_Time.Minutes.ToString("00"), p_Time.Seconds.ToString("00")) : " ";
    }



    #region Settings
    #region UIComponents
    [UIComponent("ToggleShowHandle")] private ToggleSetting m_ToggleShowHandle = null;
    [UIComponent("CustomColorSettings")] private ColorSetting m_CustomColorSettings = null;
    [UIComponent("SettingsModal")] public ModalView m_ModalView = null;
    [UIComponent("GuildList")] public DropDownListSetting m_GuildSelector = null;
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
            m_ToggleShowHandle.gameObject.SetActive(GSConfig.Instance.CardHandleVisible);
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
        get => (string)m_DropdownAvailableGuilds[0];
        set { }
    }

    [UIValue("ShowCardHandle")]
    protected bool ShowCardHandle
    {
        get => GSConfig.Instance.CardHandleVisible;
        set
        {
            GSConfig.Instance.CardHandleVisible = value;
            PlayerCardUI.m_Instance.UpdateCardHandleVisibility();
            UpdateToggleCardHandleUISettingVisibility();
        }
    }

    [UIValue("DetailLevels")]
    protected bool ShowDetailedLevels
    {
        get => GSConfig.Instance.ShowDetailsLevels;
        set
        {
            GSConfig.Instance.ShowDetailsLevels = value;
            PlayerCardUI.m_Instance.CardViewController.UpdateLevelsDetails();
        }
    }

    [UIValue("ShowPlayTime")]
    protected bool ShowPlayTime
    {
        get => GSConfig.Instance.ShowPlayTime;
        set => GSConfig.Instance.ShowPlayTime = value;
    }

    [UIValue("CustomColor")]
    protected UnityEngine.Color CustomColor
    {
        get => GSConfig.Instance.CustomColor;
        set
        {
            GSConfig.Instance.CustomColor = value;
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
        if (!Plugin.AvailableGuilds.ElementAt(m_GuildSelector.dropdown.selectedIndex).Equals(null))
        {
            GuildData l_CurrentGuild = Plugin.AvailableGuilds[m_GuildSelector.dropdown.selectedIndex];
            GSConfig.Instance.SelectedGuild = l_CurrentGuild.ID;
            BSPModule.GuildSaberModule.m_CardSelectedGuild = l_CurrentGuild;
        }
        else
        {
            GSConfig.Instance.SelectedGuild = 0;
            BSPModule.GuildSaberModule.m_CardSelectedGuild = default(GuildData);
        }
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