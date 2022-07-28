using System.Collections.Generic;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using GuildSaberProfile.API;
using GuildSaberProfile.Configuration;
using GuildSaberProfile.Time;
using GuildSaberProfile.UI.Card.Settings;
using GuildSaberProfile.Utils;
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
    public FloatingScreen m_CardScreen;

    [UIComponent("PlayerNameText")] public TextMeshProUGUI m_PlayerNameText;

    public List<object> m_AvailableGuilds = new List<object>();
    [UIComponent("DetailsLevelsLayout")] private readonly GridLayoutGroup m_DetailsLevelsLayout = null;
    [UIComponent("ElemGrid")] private readonly GridLayoutGroup m_ElementsGrid = null;
    [UIComponent("NeonBackground")] private readonly Transform m_NeonBackground = null;
    [UIComponent("PlayTimeText")] private readonly TextMeshProUGUI m_PlayTimeText = null;

    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public List<PlayerLevelUI> Levels = new List<PlayerLevelUI>();

#pragma warning disable CS0169
    private int m_NumberOfPasses;
#pragma warning restore CS0169

    public PlayerApiReworkOutput m_PlayerInfo;

    public SettingsModal m_SettingsModal;

    // ReSharper disable once CollectionNeverQueried.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public List<PlayerRankUI> Ranks = new List<PlayerRankUI>();

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
        get => m_PlayerInfo.PassCount.ToString();
        set { }
    }

    public bool AllowCustomCardColors = false;

    #region References

    public void SetReferences(PlayerApiReworkOutput p_Player, FloatingScreen p_CardScreen)
    {
        m_PlayerInfo = p_Player;
        m_CardScreen = p_CardScreen;
    }
    #endregion

    public async Task<Task> UpdateCanPlayerUseCustomColors()
    {
        int l_MaxLevel = 0;
        string l_BestCategory = string.Empty;

        //-----------------------------------------Checking if Player level is at 97% of max level-----------------------------------------
        if (!(PluginConfig.Instance.SelectedGuild == "BSCC"))
            foreach (var l_Current in m_PlayerInfo.CategoryData)
                if (l_Current.Level == m_PlayerInfo.Level)
                    l_BestCategory = l_Current.Category;
                else
                    l_BestCategory = string.Empty;

        l_MaxLevel = GuildSaberUtils.GetStaticPlayerLevel(l_BestCategory);

        //If thats true Allowing Custom Card Colors
        AllowCustomCardColors = m_PlayerInfo.Level >= (int)(l_MaxLevel * 0.97f);

        //Returning
        return Task.CompletedTask;
    }

    #region Main Card Info and style Loading

    public async Task<Task> UpdateCardColor()
    {
        //If Player is allowed to use custom colors, updating ProfileColor to selected color
        if (AllowCustomCardColors == true)
            m_PlayerInfo.ProfileColor = GuildSaberProfile.Utils.Color.ToGSProfileColor(PluginConfig.Instance.CustomColor);

        //-----------------------------------------Gradient-----------------------------------------
        //Defaults
        UnityEngine.Color l_PlayerColor = m_PlayerInfo.ProfileColor.ToUnityColor();
        UnityEngine.Color l_BeforePlayerColor = new UnityEngine.Color(l_PlayerColor.r * 0.8f, l_PlayerColor.g * 0.8f, l_PlayerColor.b * 0.8f);
        UnityEngine.Color l_NewPlayerColor = new UnityEngine.Color(l_PlayerColor.r * 1.2f, l_PlayerColor.g * 1.2f, l_PlayerColor.b * 1.2f);

        //Applying Gradient
        m_PlayerNameText.enableVertexGradient = true;
        m_PlayerNameText.colorGradient = new VertexGradient(l_BeforePlayerColor, l_BeforePlayerColor, l_NewPlayerColor, l_NewPlayerColor);

        //-----------------------------------------Background editing-----------------------------------------
        ImageView l_CurrentImageView = m_NeonBackground.GetComponentInChildren<ImageView>();
        //Editing style
        l_CurrentImageView.SetField("_skew", 0.0f);
        l_CurrentImageView.overrideSprite = null;
        l_CurrentImageView.SetImage("#RoundRect10BorderFade");
        //Editing Colors
        l_CurrentImageView.color0 = l_BeforePlayerColor.ColorWithAlpha(1f);
        l_CurrentImageView.color1 = l_NewPlayerColor.ColorWithAlpha(1f);
        l_CurrentImageView.color = l_PlayerColor.ColorWithAlpha(1f);
        //idk
        l_CurrentImageView.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;

        //Explicit
        return Task.CompletedTask;
    }

    [UIAction("#post-parse")]
    public async void PostParse()
    {
        if (m_PlayerInfo.Equals(null)) return;

        await UpdateCanPlayerUseCustomColors();

        //Creating Settings Modal
        if (m_SettingsModal == null)
            m_SettingsModal = new SettingsModal(m_CardScreen.gameObject, AllowCustomCardColors);

        //Settings Modal Defaults
        m_SettingsModal.m_AvailableGuilds = m_AvailableGuilds;
        m_SettingsModal.m_ShowCustomColors = AllowCustomCardColors;


        //update
        await UpdateCardColor();
        m_SettingsModal.UpdateShowColors();

        Plugin.Log.Info("Card loaded");
    }

    [UIAction("OnPPClick")]
    private void OnPPClick()
    {
        //If player disabled settings modal don't showing
        if (!PluginConfig.Instance.ShowSettingsModal) return;

        if (m_SettingsModal is null) return;

        m_SettingsModal.ShowModal();
    }
    #endregion

    #region Card Updates

    public void UpdateLevelsDetails()
    {
        //Checking if Player enabled Details Levels and guild has differents categories
        bool l_ShowDetaislLevels = PluginConfig.Instance.ShowDetailsLevels && Levels.Count > 0;

        m_DetailsLevelsLayout.gameObject.SetActive(l_ShowDetaislLevels);

        if (m_CardScreen == null)
            return;

        float l_LevelsSize = Levels.Count;
        if (l_ShowDetaislLevels)
        {
            //When the details levels is visible
            m_CardScreen.ScreenSize = new Vector2((68 + m_PlayerInfo.Name.Length * 1.2f + l_LevelsSize) * 0.9f, 28 + l_LevelsSize * 0.6f + Ranks.Count * 2);
            m_ElementsGrid.cellSize = new Vector2((40 + m_PlayerInfo.Name.Length + l_LevelsSize) * 1.1f, 40);
            m_DetailsLevelsLayout.cellSize = new Vector2(12 - l_LevelsSize * 0.1f, 10.5f - l_LevelsSize * 0.1f);
            m_ElementsGrid.spacing = new Vector2(7, 7);
        }
        else
        {
            //When the details levels is hidden
            m_CardScreen.ScreenSize = new Vector2(33 + m_PlayerInfo.Name.Length, 28 + Ranks.Count * 2);
            m_ElementsGrid.cellSize = new Vector2(25 + m_PlayerInfo.Name.Length, 40);
            m_ElementsGrid.spacing = new Vector2(1, 7);
        }
    }

    public void UpdateTime(OptimizedDateTime p_Time)
    {
        //Let's go
        m_PlayTimeText.text = PluginConfig.Instance.ShowPlayTime ? string.Join(":", p_Time.Hours.ToString("00"), p_Time.Minutes.ToString("00"), p_Time.Seconds.ToString("00")) : " ";
    }

    public void UpdateToggleCardHandleVisibility()
    {
        if (m_SettingsModal != null)
            m_SettingsModal.UpdateToggleCardHandleVisibility();
    }

    #endregion
}
#endregion