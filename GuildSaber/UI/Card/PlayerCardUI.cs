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
using GuildSaber.BSPModule;
using BeatSaberPlus.SDK.Game;
using GuildSaber.Logger;

namespace GuildSaber.UI.Card;

public class PlayerLevelUI : CustomUIComponent
{
    protected override string m_ViewResourceName => "GuildSaber.UI.Card.View.PlayerLevelUI.bsml";

    [UIComponent("ElemsLayout")] private  VerticalLayoutGroup m_Elems = null;

    [UIComponent("LevelNameText")] private TextMeshProUGUI m_LevelNameText = null;
    [UIComponent("LevelText")] private TextMeshProUGUI m_LevelText = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public int FontSize { get; private set; } = 0;

    public string Level { get; set; } = string.Empty;

    public string LevelName { get; set; } = string.Empty;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Set values
    /// </summary>
    /// <param name="p_LevelName"></param>
    /// <param name="p_Level"></param>
    public void SetValues(string p_LevelName, string p_Level)
    {
        LevelName = p_LevelName;
        Level = p_Level;
        m_LevelNameText.text = p_LevelName;
        m_LevelText.text = p_Level;
    }

    /// <summary>
    /// Reset
    /// </summary>
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
    [UIComponent("ElemsLayout")] private readonly HorizontalLayoutGroup m_Elems = null;
    [UIComponent("CategoryText")] private readonly TextMeshProUGUI m_CategoryText = null;
    [UIComponent("PlayerRankText")] private readonly TextMeshProUGUI m_PlayerRankText = null;
    [UIComponent("Hastag")] private readonly TextMeshProUGUI m_Hastag = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public string PointsName { get; private set; } = string.Empty;

    public string PlayerRank { get; private set; } = string.Empty;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Set Values
    /// </summary>
    /// <param name="p_PointsName"></param>
    /// <param name="p_PlayerRank"></param>
    /// <param name="p_Color"></param>
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

    /// <summary>
    /// Reset
    /// </summary>
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
        //Plugin.Log.Info("Loading Player Card");

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

        BeatSaberPlus.SDK.Game.Logic.OnSceneChange += (p_SceneType) =>
        {
            UpdateCardPosition();
            m_Instance.UpdateCardVisibility();
        };

        RefreshCard(false);
    }

    /// <summary>
    /// Create Player Card
    /// </summary>
    /// <returns></returns>
    public async static Task<PlayerCardUI> CreateCard()
    {
        if (m_Instance != null) return m_Instance;

        if (GuildSaberModule.AvailableGuilds.Count == 0) return null;

        if (!GuildSaberUtils.GuildsListContainsId(GuildSaberModule.AvailableGuilds, GSConfig.Instance.SelectedGuild))
        {
            GSConfig.Instance.SelectedGuild = GuildSaberModule.AvailableGuilds[0].ID;
            BSPModule.GuildSaberModule.m_CardSelectedGuild = GuildSaberModule.AvailableGuilds[0];
        }

        ApiPlayerData l_Player = await GuildApi.GetPlayerInfoFromAPI(p_GuildFromConfig: false, GSConfig.Instance.SelectedGuild, p_UseGuild: true);

        if (l_Player.Equals(default(PlayerGuildsInfo)) || l_Player.Equals(null)) { GSLogger.Instance.Error(new Exception("Failed Getting Player Info"), nameof(PlayerCardUI), nameof(CreateCard)); return null; }

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
        if (m_Instance == null) {  return; }

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
        switch (Logic.ActiveScene)
        {
            case Logic.SceneType.Menu:
                FloatingScreen.gameObject.SetActive(GSConfig.Instance.ShowCardInMenu);
                break;
            case Logic.SceneType.Playing:
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
        switch (BeatSaberPlus.SDK.Game.Logic.ActiveScene)
        {
            case BeatSaberPlus.SDK.Game.Logic.SceneType.Menu:
                FloatingScreen.gameObject.transform.localPosition = GSConfig.Instance.CardPosition.ToUnityVector3();
                FloatingScreen.gameObject.transform.localRotation = GSConfig.Instance.CardRotation.ToUnityQuat();
                break;
            case BeatSaberPlus.SDK.Game.Logic.SceneType.Playing:
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
    public async static void RefreshCard(bool p_RegetPlayerInfo)
    {
        if (p_RegetPlayerInfo == true)
        {
            if (m_Instance == null) { await CreateCard(); return; }

            ApiPlayerData l_Player = await GuildApi.GetPlayerInfoFromAPI();
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
            m_Instance = null;
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