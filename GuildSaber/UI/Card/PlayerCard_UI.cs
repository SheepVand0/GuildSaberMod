using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using GuildSaber.API;
using GuildSaber.Configuration;
using GuildSaber.Time;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using CP_SDK_WebSocketSharp;
using GuildSaber.Utils;
using System.Diagnostics;

namespace GuildSaber.UI.Card;

public class PlayerLevelUI : CustomUIComponent
{
    protected override string m_ViewResourceName => "GuildSaber.UI.Card.View.PlayerLevelUI.bsml";

    [UIComponent("ElemsLayout")] private  VerticalLayoutGroup m_Elems = null;

    [UIComponent("LevelNameText")] private TextMeshProUGUI m_LevelNameText = null;
    [UIComponent("LevelText")] private TextMeshProUGUI m_LevelText = null;

    #region Properties
    // ReSharper disable once MemberInitializerValueIgnored
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public int FontSize { get; set; }
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once MemberInitializerValueIgnored
    public string Level { get; set; }
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once MemberInitializerValueIgnored
    public string LevelName { get; set; }
    #endregion

    public void SetValues(string p_LevelName, string p_Level)
    {
        LevelName = p_LevelName;
        Level = p_Level;
        m_LevelNameText.text = p_LevelName;
        m_LevelText.text = p_Level;
    }

    public override void ResetComponent()
    {
        Level = string.Empty;
        LevelName = string.Empty;
        m_LevelNameText.text = string.Empty;
        m_LevelText.text = string.Empty;
    }
}

public class PlayerRankUI : CustomUIComponent
{
    protected override string m_ViewResourceName => "GuildSaber.UI.Card.View.PlayerRankUI.bsml";

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    ///
    [UIComponent("ElemsLayout")] HorizontalLayoutGroup m_Elems = null;
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    [UIComponent("CategoryText")] TextMeshProUGUI m_CategoryText = null;
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    [UIComponent("PlayerRankText")] TextMeshProUGUI m_PlayerRankText = null;
    [UIComponent("Hastag")] TextMeshProUGUI m_Hastag = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public string PointsName { get; private set; } = string.Empty;

    public string PlayerRank { get; private set; } = string.Empty;

    public async void SetValues(string p_PointsName, string p_PlayerRank, Color p_Color)
    {
        await WaitUtils.WaitUntil(() => m_CategoryText != null, 100, 20);

        PointsName = p_PointsName;
        PlayerRank = p_PlayerRank;
        /*if (m_Hastag != null && PointsName.IsNullOrEmpty() && PlayerRank.IsNullOrEmpty())
            GameObject.DestroyImmediate(m_Hastag.gameObject);*/

        Color l_PlayerColor = p_Color;
        Color l_BeforePlayerColor = new Color(l_PlayerColor.r * 0.8f, l_PlayerColor.g * 0.8f, l_PlayerColor.b * 0.8f);
        Color l_NewPlayerColor = new Color(l_PlayerColor.r * 1.2f, l_PlayerColor.g * 1.2f, l_PlayerColor.b * 1.2f);

        VertexGradient l_TextGradient = new VertexGradient(l_BeforePlayerColor, l_BeforePlayerColor, l_NewPlayerColor, l_NewPlayerColor);

        m_PlayerRankText.enableVertexGradient = true;
        m_CategoryText.enableVertexGradient = true;
        m_PlayerRankText.colorGradient = l_TextGradient;
        m_CategoryText.colorGradient = l_TextGradient;

        m_CategoryText.text = PointsName;
        m_PlayerRankText.text = PlayerRank;

        m_Hastag.text = "#";
    }

    public override void ResetComponent()
    {
        m_CategoryText.text = string.Empty;
        m_PlayerRankText.text = string.Empty;
        m_Hastag.text = string.Empty;
    }
}

internal class PlayerCardUI
{
    public static PlayerCardUI m_Instance = null;

    public static ApiPlayerData m_Player = default(ApiPlayerData);

    public static TimeManager m_TimeManager = null;

    private static bool IsCardActive = true;

    public PlayerCardViewController CardViewController { get; private set; }

    private FloatingScreen FloatingScreen = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Card Constructor
    /// </summary>
    public PlayerCardUI()
    {
        Plugin.Log.Info("Loading Player Card");

        if (m_TimeManager == null)
        {
            m_TimeManager = new GameObject("CardPlayTime").AddComponent<TimeManager>();
            GameObject.DontDestroyOnLoad(m_TimeManager);
        }

        CardViewController = BeatSaberUI.CreateViewController<PlayerCardViewController>();
        FloatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(40f, 40f), true, GSConfig.Instance.CardPosition.ToUnityVector3(), GSConfig.Instance.CardRotation.ToUnityQuat());
        FloatingScreen.HighlightHandle = true;
        FloatingScreen.HandleSide = FloatingScreen.Side.Right;
        FloatingScreen.HandleReleased += OnCardHandleReleased;

        CardViewController.SetReferences(FloatingScreen);

        #region Debug with a lot
        /// For debug purpose with lots of levels
        /*for (int l_i = 0; l_i < 50; l_i++)
        {
          CardViewController.Levels.Add(new PlayerLevelUI("Vibro", "31", 50));
        }
        }*/
        #endregion

        FloatingScreen.SetRootViewController(CardViewController, ViewController.AnimationType.None);

        m_TimeManager.SetPlayerCardViewControllerRef(CardViewController);

        GameObject.DontDestroyOnLoad(FloatingScreen);

        m_Instance = this;

        RefreshCard(false);
    }

    /// <summary>
    /// Create Player Card
    /// </summary>
    /// <returns></returns>
    public static PlayerCardUI CreateCard()
    {
        if (Plugin.AvailableGuilds.Count == 0) return null;

        if (!GuildSaberUtils.GuildsListContainsId(Plugin.AvailableGuilds, GSConfig.Instance.SelectedGuild))
        {
            GSConfig.Instance.SelectedGuild = Plugin.AvailableGuilds[0].ID;
            BSPModule.GuildSaberModule.m_CardSelectedGuild = Plugin.AvailableGuilds[0];
        }

        ApiPlayerData l_Player = GuildApi.GetPlayerInfoFromAPI(p_GuildFromConfig: false, GSConfig.Instance.SelectedGuild, p_UseGuild: true);

        if (l_Player.Equals(default(PlayerGuildsInfo)) || l_Player.Equals(null)) { Plugin.Log.Error("Failed Getting Player Info"); return null; }

        m_Player = l_Player;

        new PlayerCardUI();

        return m_Instance;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Change card active
    /// </summary>
    /// <param name="p_Active"></param>
    public static void SetCardActive(bool p_Active)
    {
        if (m_Instance == null) return;

        if (m_Instance.CardViewController != null) m_Instance.CardViewController.gameObject.SetActive(p_Active);
        else return;

        IsCardActive = p_Active;
        m_Instance.FloatingScreen.gameObject.SetActive(p_Active);
    }

    /// <summary>
    /// Update card need to show handle
    /// </summary>
    public void UpdateCardHandleVisibility()
    {
        if (FloatingScreen == null) return;
        FloatingScreen.ShowHandle = GSConfig.Instance.CardHandleVisible;
        FloatingScreen.UpdateHandle();
    }

    /// <summary>
    /// Udpate card visibility by current scene
    /// </summary>
    public void UpdateCardVisibility()
    {
        switch (Plugin.CurrentSceneName)
        {
            case "MainMenu":
                FloatingScreen.gameObject.SetActive(GSConfig.Instance.ShowCardInMenu);
                break;
            case "GameCore":
                FloatingScreen.gameObject.SetActive(GSConfig.Instance.ShowCardInGame);
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Update card position by scene
    /// </summary>
    public void UpdateCardPosition()
    {
        switch (Plugin.CurrentSceneName)
        {
            case "MainMenu":
                FloatingScreen.gameObject.transform.localPosition = GSConfig.Instance.CardPosition.ToUnityVector3();
                FloatingScreen.gameObject.transform.localRotation = GSConfig.Instance.CardRotation.ToUnityQuat();
                break;
            case "GameCore":
                FloatingScreen.gameObject.transform.localPosition = GSConfig.Instance.InGameCardPosition.ToUnityVector3();
                FloatingScreen.gameObject.transform.localRotation = GSConfig.Instance.InGameCardRotation.ToUnityQuat();
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Refresh card elements
    /// </summary>
    /// <param name="p_RegetPlayerInfo"></param>
    public static void RefreshCard(bool p_RegetPlayerInfo)
    {
        if (p_RegetPlayerInfo == true)
        {
            if (m_Instance == null) { CreateCard(); return; }

            ApiPlayerData l_Player = GuildApi.GetPlayerInfoFromAPI();
            if (l_Player.Equals(null)) { if (m_Instance != null) SetCardActive(false); return; }
            m_Player = l_Player;

            m_Instance.CardViewController.Refresh();
        }

        m_Instance.UpdateCardPosition();
        m_Instance.UpdateCardVisibility();
        m_Instance.UpdateCardHandleVisibility();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Event called when card handle is released
    /// </summary>
    /// <param name="p_Sender"></param>
    /// <param name="p_EventArgs"></param>
    private static void OnCardHandleReleased(object p_Sender, FloatingScreenHandleEventArgs p_EventArgs)
    {
        switch (BeatSaberPlus.SDK.Game.Logic.ActiveScene)
        {
            case BeatSaberPlus.SDK.Game.Logic.SceneType.Menu:
                GSConfig.Instance.CardPosition = SerializableVector3.FromUnityVector3(p_EventArgs.Position);
                GSConfig.Instance.CardRotation = SerializableQuaternion.FromUnityQuat(p_EventArgs.Rotation);
                break;
            case BeatSaberPlus.SDK.Game.Logic.SceneType.Playing:
                GSConfig.Instance.InGameCardPosition = SerializableVector3.FromUnityVector3(p_EventArgs.Position);
                GSConfig.Instance.InGameCardRotation = SerializableQuaternion.FromUnityQuat(p_EventArgs.Rotation);
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Reset card menu position
    /// </summary>
    public static void ResetMenuCardPosition()
    {
        GSConfig.Instance.CardPosition = SerializableVector3.FromUnityVector3(GSConfig.ConfigDefaults.DefaultCardPosition);
        GSConfig.Instance.CardRotation = SerializableQuaternion.FromUnityQuat(GSConfig.ConfigDefaults.DefaultCardRotation);
        if (PlayerCardUI.m_Instance != null)
            PlayerCardUI.m_Instance.UpdateCardPosition();
    }

    /// <summary>
    /// Reset card game position
    /// </summary>
    public static void ResetInGameCardPosition()
    {
        GSConfig.Instance.InGameCardPosition = SerializableVector3.FromUnityVector3(GSConfig.ConfigDefaults.DefaultInGameCardPosition);
        GSConfig.Instance.InGameCardRotation = SerializableQuaternion.FromUnityQuat(GSConfig.ConfigDefaults.DefaultInGameCardRotation);
        if (BeatSaberPlus.SDK.Game.Logic.ActiveScene == BeatSaberPlus.SDK.Game.Logic.SceneType.Playing && PlayerCardUI.m_Instance != null)
            PlayerCardUI.m_Instance.UpdateCardPosition();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Destroy card
    /// </summary>
    [Obsolete("No recommended to use this, can cause bugs")]
    public void Destroy()
    {
        GameObject.DestroyImmediate(CardViewController.gameObject);
        GameObject.DestroyImmediate(FloatingScreen.gameObject);
    }

    /// <summary>
    /// Destroy card
    /// </summary>
    public static void DestroyCard()
    {
        if (m_Instance != null && m_Instance.CardViewController != null)
        {
            m_Instance.Destroy();
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Get is card active
    /// </summary>
    /// <returns></returns>
    internal static bool GetIsCardActive() => IsCardActive;
}