using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberPlus.SDK.Game;
using GuildSaber.API;
using GuildSaber.BSPModule;
using GuildSaber.Configuration;
using GuildSaber.Logger;
using GuildSaber.Time;
using GuildSaber.Utils;
using GuildSaber.Utils.Color;
using HMUI;
using IPA.Utilities;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace GuildSaber.UI.Card;

internal class PlayerCardViewController : BeatSaberPlus.SDK.UI.ViewController<PlayerCardViewController>
{
    protected override string GetViewContentDescription()
    {
        return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "GuildSaber.UI.Card.View.PlayerCard_UI.bsml");
    }

    [UIComponent("PlayerNameText")] public TextMeshProUGUI m_PlayerNameText = null;
    [UIComponent("PlayerGlobalLevelText")] public TextMeshProUGUI m_PlayerGlobalLevelText = null;
    [UIComponent("DetailsLevelsLayout")] private readonly GridLayoutGroup m_DetailsLevelsLayout = null;
    [UIComponent("ElemGrid")] private readonly GridLayoutGroup m_ElementsGrid = null;
    [UIComponent("NeonBackground")] private readonly Transform m_NeonBackground = null;
    [UIComponent("PlayTimeText")] private readonly TextMeshProUGUI m_PlayTimeText = null;
    [UIComponent("RankUIVertical")] private readonly VerticalLayoutGroup m_RankUIVertical = null;
    [UIComponent("PlayerNumberOfPasses")] private readonly TextMeshProUGUI m_PlayerNumberOfPasses = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    /// Settings
    [UIComponent("ToggleShowHandle")] private readonly ToggleSetting m_ToggleShowHandle = null;
    [UIComponent("ToggleDetailedLevels")] private readonly ToggleSetting m_ToggleDetailedLevels = null;
    [UIComponent("ToggleShowPlayTime")] private readonly ToggleSetting m_ToggleShowPlayTime = null;
    [UIComponent("ToggleCustomCardColors")]  private readonly ToggleSetting m_ToggleCustomCardColors = null;
    [UIComponent("ToggleCustomCardGradient")] private readonly ToggleSetting m_ToggleCustomCardGradient = null;
    [UIComponent("CustomColorSettings")] private readonly ColorSetting m_CustomColorSettings = null;
    [UIComponent("CustomColorSettings1")] private readonly ColorSetting m_CustomColorSettings1 = null;
    [UIComponent("NameGradientMultiplier")] private readonly SliderSetting m_NameGradientMutiliplier = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [UIValue("AvailableGuilds")]
    private readonly List<object> m_DropdownAvailableGuilds = new()
    {
        "UwU"
    };

    //[CanBeNull]
    public string PlayerName
    {
        get => PlayerCardUI.m_Player.Name;
        // ReSharper disable once ValueParameterNotUsed
        set { }
    }
    public string PlayerGlobalLevel
    {
        get => PlayerCardUI.m_Player.LevelValue.ToString();
        // ReSharper disable once ValueParameterNotUsed
        set { }
    }
    public string PlayerImageSrc
    {
        get => PlayerCardUI.m_Player.Avatar;
        // ReSharper disable once ValueParameterNotUsed
        set { }
    }
    public string PlayerNumberOfPasses
    {
        get => PlayerCardUI.m_Player.GuildValidPassCount.ToString();
        // ReSharper disable once ValueParameterNotUsed
        set { }
    }


    public List<PlayerRankUI> m_Ranks = new List<PlayerRankUI>();
    public List<PlayerLevelUI> m_Levels = new List<PlayerLevelUI>();
    public FloatingScreen m_CardScreen;
    public bool m_AllowCustomCardColors = false;
    public bool m_AllowCustomCardGradient = false;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Post parse
    /// </summary>
    protected override void OnViewCreation()
    {
        if (PlayerCardUI.m_Player.Equals(null)) return;

        m_DropdownAvailableGuilds.Clear();
        foreach (GuildData l_Current in GuildSaberModule.AvailableGuilds)
        {
            m_DropdownAvailableGuilds.Add(l_Current.Name);
        }
        m_GuildSelector.UpdateChoices();

        m_GuildSelector.Value = GuildSaberUtils.GetGuildFromId(GSConfig.Instance.SelectedGuild).Name;
        m_GuildSelector.ApplyValue();

        GuildSaberModule.CardSelectedGuild = GuildSaberUtils.GetGuildFromId(GSConfig.Instance.SelectedGuild);

        ///Settings setup
        BSMLAction l_SettingAction = new BSMLAction(this, GetType().GetMethod(nameof(OnSettingChanged), BindingFlags.Instance | BindingFlags.NonPublic));

        BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_ToggleShowHandle, l_SettingAction, GSConfig.Instance.CardHandleVisible, false);
        BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_ToggleDetailedLevels, l_SettingAction, GSConfig.Instance.ShowDetailsLevels, false);
        BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_ToggleShowPlayTime, l_SettingAction, GSConfig.Instance.ShowPlayTime, false);
        BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_ToggleCustomCardColors, l_SettingAction, GSConfig.Instance.UseCustomColor, false);
        BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_ToggleCustomCardGradient, l_SettingAction, GSConfig.Instance.UseCustomColorGradient, false);

        BeatSaberPlus.SDK.UI.SliderSetting.Setup(m_NameGradientMutiliplier, l_SettingAction, null,GSConfig.Instance.NameGradientColor0Multiplier, false);

        BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_CustomColorSettings, l_SettingAction, GSConfig.Instance.CustomColor, false);
        BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_CustomColorSettings1, l_SettingAction, GSConfig.Instance.CustomColor1, false);

        Refresh();

        //Plugin.Log.Debug("Card loaded");
    }

    ////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Update colors
    /// </summary>
    public void UpdateCardColor()
    {
        if (m_AllowCustomCardColors && GSConfig.Instance.UseCustomColor)
            PlayerCardUI.m_Player.Color = GSConfig.Instance.CustomColor.ToGSProfileColor();

        Color l_PlayerColor = PlayerCardUI.m_Player.Color.ToUnityColor32();

        m_PlayerNameText.enableVertexGradient = true;

        Color l_Color0 = (GSConfig.Instance.UseCustomColor) ?
            GSConfig.Instance.CustomColor : l_PlayerColor;
        Color l_Color1 = (GSConfig.Instance.UseCustomColorGradient) ? GSConfig.Instance.CustomColor1 : l_PlayerColor;

        VertexGradient l_Gradient =
            (GSConfig.Instance.UseCustomColorGradient && m_AllowCustomCardGradient) ?
                GuildSaberUtils.Equilibrate(l_Color0, l_Color1, GSConfig.Instance.NameGradientColor0Multiplier).GenerateGradient(0.2f) :
                l_PlayerColor.GenerateGradient(0.2f);

        m_PlayerNameText.colorGradient = l_Gradient;

        ImageView l_CurrentImageView = m_NeonBackground.GetComponentInChildren<ImageView>();
        l_CurrentImageView.SetField("_skew", 0.0f);
        l_CurrentImageView.overrideSprite = null;
        l_CurrentImageView.SetImage("#RoundRect10BorderFade");
        l_CurrentImageView.color0 = l_Color0;
        l_CurrentImageView.color1 = (l_Color1.IsIn(l_Color0)) ? l_Color1 : l_Color1.Add(0.5f);
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

        //Checking if Player level is at 90% of max level, and if his level is under of 2 levels of max level
        if (PlayerCardUI.m_Player.CategoryData.Count > 0)
        {
            foreach (var l_Current in PlayerCardUI.m_Player.CategoryData)
            {
                if (l_MaxLevel < l_Current.MaxLevelValue) l_MaxLevel = l_Current.MaxLevelValue ?? 0;
            }
        }

        m_AllowCustomCardColors = PlayerCardUI.m_Player.LevelValue >= (float)(l_MaxLevel * 0.9f);
        m_AllowCustomCardGradient = l_MaxLevel - PlayerCardUI.m_Player.LevelValue <= 2;

        UpdateShowPlayerCustomColorUISetting();
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
            if (GuildSaberModule.CardSelectedGuild.Equals(default(GuildData))) return;

            ///Checking
            bool l_SetCardActive = !GuildSaberModule.CardSelectedGuild.Equals(default(GuildData));
            if (l_SetCardActive)
                PlayerCardUI.SetCardActive(true);
            else
            {
                PlayerCardUI.SetCardActive(false);
                return;
            }

            ///If ranks are empty create a default list
            if (m_Ranks.Count == 0)
                for (int l_i = 0; l_i < 3; l_i++)
                    m_Ranks.Add(CustomUIComponent.CreateItem<PlayerRankUI>(m_RankUIVertical.transform, true, true));

            ///Reset all
            foreach (PlayerRankUI l_Current in m_Ranks) l_Current.ResetComponent();

            foreach (PlayerLevelUI l_Current in m_Levels) l_Current.ResetComponent();

            ///Set rank
            int l_RankDataCount = PlayerCardUI.m_Player.RankData.Count;
            for (int l_i = 0; l_i < l_RankDataCount; l_i++)
            {
                RankData l_Rank = PlayerCardUI.m_Player.RankData[l_i];
                m_Ranks[l_i].SetValues(l_Rank.PointsName, l_Rank.Rank.ToString(), PlayerCardUI.m_Player.Color.ToUnityColor());
            }

            ///Set Levels
            int l_CategoryDataCount = PlayerCardUI.m_Player.CategoryData.Count;
            for (int l_i = 0; l_i < l_CategoryDataCount; l_i++)
            {
                ApiPlayerCategory l_Cat = PlayerCardUI.m_Player.CategoryData[l_i];
                int l_FontSize = (int)(2 / (l_CategoryDataCount * 0.11f));
                if (l_FontSize < 1) l_FontSize = 2;
                if (l_FontSize == 5) l_FontSize = 4;

                ///Add to list if there is more categories than objects in Levels
                if (l_i > m_Levels.Count - 1)
                {
                    List<ItemParam> l_Params = new List<ItemParam>()
                    {
                        new ItemParam("Level", l_Cat.LevelValue?.ToString("0.0")),
                        new ItemParam("LevelName", l_Cat.CategoryName),
                        new ItemParam("FontSize", l_FontSize)
                    };
                    PlayerLevelUI l_Temp = CustomUIComponent.CreateItemWithParams<PlayerLevelUI>(m_DetailsLevelsLayout.transform, true, true, l_Params);
                    m_Levels.Add(l_Temp);
                }
                else
                {
                    ///Else just set the value
                    m_Levels[l_i].SetValues(l_Cat.CategoryName, l_Cat.LevelValue?.ToString("0.0"));
                }
            }

            m_PlayerNumberOfPasses.text = PlayerCardUI.m_Player.GuildValidPassCount.ToString();
            m_PlayerNameText.text = GuildSaberUtils.GetPlayerNameToFit(PlayerCardUI.m_Player.Name, 16);

            m_PlayerGlobalLevelText.text = PlayerCardUI.m_Player.LevelValue?.ToString("0.0");
            UpdateCanPlayerUseCustomColors();
            UpdateCardColor();
            UpdateLevelsDetails();
        }
        catch (Exception l_E)
        {
            GSLogger.Instance.Error(l_E, nameof(PlayerCardViewController), nameof(Refresh));
        }
    }

    /// <summary>
    /// Update levels details
    /// </summary>
    public void UpdateLevelsDetails()
    {
        bool l_ShowDetaislLevels = GSConfig.Instance.ShowDetailsLevels;
        if (m_Levels.Count == 0) l_ShowDetaislLevels = false;
        m_DetailsLevelsLayout.gameObject.SetActive(l_ShowDetaislLevels);
        if (m_CardScreen == null) return;
        float l_LevelsSize = m_Levels.Count;
        if (l_ShowDetaislLevels)
        {
            //When the details levels is visible
            m_CardScreen.ScreenSize = new Vector2((68 + PlayerCardUI.m_Player.Name.Length * 1.2f + l_LevelsSize) * 0.9f, 28 + l_LevelsSize * 0.65f + m_Ranks.Count * 2);
            m_ElementsGrid.cellSize = new Vector2((40 + PlayerCardUI.m_Player.Name.Length + l_LevelsSize) * 1.1f, 40);
            m_DetailsLevelsLayout.cellSize = new Vector2(12 - l_LevelsSize * 0.1f, 10.5f - l_LevelsSize * 0.1f);
            m_ElementsGrid.spacing = new Vector2(7, 7);
        }
        else
        {
            //When the details levels is hidden
            m_CardScreen.ScreenSize = new Vector2(33 + PlayerCardUI.m_Player.Name.Length, 28 + m_Ranks.Count * 2);
            m_ElementsGrid.cellSize = new Vector2(25 + PlayerCardUI.m_Player.Name.Length, 40);
            m_ElementsGrid.spacing = new Vector2(1, 7);
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Time
    /// </summary>
    /// <param name="p_Time"></param>
    public void UpdateTime(OptimizedDateTime p_Time)
    {
        //Let's go
        m_PlayTimeText.text = GSConfig.Instance.ShowPlayTime ? string.Join(":", p_Time.Hours.ToString("00"), p_Time.Minutes.ToString("00"), p_Time.Seconds.ToString("00")) : " ";
    }

    ////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    /// Settings
    [UIComponent("SettingsModal")] public ModalView m_ModalView = null;
    [UIComponent("GuildList")] public DropDownListSetting m_GuildSelector = null;

    ////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Show color setting if player can use it
    /// </summary>
    public void UpdateShowPlayerCustomColorUISetting()
    {
        m_CustomColorSettings.interactable = m_AllowCustomCardColors && GSConfig.Instance.UseCustomColor;
        m_CustomColorSettings1.interactable = m_AllowCustomCardGradient && GSConfig.Instance.UseCustomColorGradient;
        m_ToggleCustomCardColors.interactable = m_AllowCustomCardColors;
        m_ToggleCustomCardGradient.interactable = m_AllowCustomCardGradient;
    }

    /// <summary>
    /// Cannot show an handle in game
    /// </summary>
    public void UpdateToggleCardHandleUISettingVisibility()
    {
        if (Logic.ActiveScene == Logic.SceneType.Playing)
            m_ToggleShowHandle.gameObject.SetActive(GSConfig.Instance.CardHandleVisible);
        else
            m_ToggleShowHandle.gameObject.SetActive(true);
    }

    /// <summary>
    /// Show
    /// </summary>
    public void ShowSettings()
    {
        m_ModalView.Show(true, false, null);
    }

    ////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    private void OnSettingChanged(object p_Value)
    {
        GSConfig.Instance.CardHandleVisible = m_ToggleShowHandle.Value;
        GSConfig.Instance.ShowDetailsLevels = m_ToggleDetailedLevels.Value;
        GSConfig.Instance.ShowPlayTime = m_ToggleShowPlayTime.Value;
        GSConfig.Instance.UseCustomColor = m_ToggleCustomCardColors.Value;
        GSConfig.Instance.UseCustomColorGradient = m_ToggleCustomCardGradient.Value;
        GSConfig.Instance.CustomColor = m_CustomColorSettings.CurrentColor;
        GSConfig.Instance.CustomColor1 = m_CustomColorSettings1.CurrentColor;
        GSConfig.Instance.NameGradientColor0Multiplier = m_NameGradientMutiliplier.Value;

        PlayerCardUI.m_Instance.CardViewController.UpdateLevelsDetails();
        PlayerCardUI.m_Instance.UpdateCardHandleVisibility();
        UpdateToggleCardHandleUISettingVisibility();
        UpdateShowPlayerCustomColorUISetting();
        UpdateCardColor();
    }

    [UIValue("SelectedGuild")]
    protected string SelectedGuild
    {
        get => (string)m_DropdownAvailableGuilds[0];
        // ReSharper disable once ValueParameterNotUsed
        set { }
    }

    ////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    [UIAction("RefreshCard")]
    // ReSharper disable once MemberCanBeMadeStatic.Local
    private void OnButtonRefreshCardClicked()
    {
        PlayerCardUI.RefreshCard(true);
    }

    [UIAction("UpdateCard")]
    private void UpdateCard(string p_Selected)
    {
        if (!GuildSaberModule.AvailableGuilds.ElementAt(m_GuildSelector.dropdown.selectedIndex).Equals(null))
        {
            GuildData l_CurrentGuild = GuildSaberModule.AvailableGuilds[m_GuildSelector.dropdown.selectedIndex];
            GSConfig.Instance.SelectedGuild = l_CurrentGuild.ID;
            GuildSaberModule.CardSelectedGuild = l_CurrentGuild;
        }
        else
        {
            GSConfig.Instance.SelectedGuild = 0;
            GuildSaberModule.CardSelectedGuild = default(GuildData);
        }
        OnButtonRefreshCardClicked();
    }

    [UIAction("ResetPosMenu")] private void ResetPosMenu()
    {
        PlayerCardUI.ResetMenuCardPosition();
    }

    [UIAction("ResetPosGame")] private void ResetPosInGame()
    {
        PlayerCardUI.ResetInGameCardPosition();
    }
}
