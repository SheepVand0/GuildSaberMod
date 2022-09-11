using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using GuildSaberProfile.API;
using GuildSaberProfile.Configuration;
using GuildSaberProfile.Time;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using CP_SDK_WebSocketSharp;

namespace GuildSaberProfile.UI.Card;

public class PlayerLevelUI : CustomUIComponent
{
    protected override string m_ViewResourceName => "GuildSaberProfile.UI.Card.View.PlayerLevelUI.bsml";

    [UIComponent("ElemsLayout")] VerticalLayoutGroup m_Elems = null;

    [UIComponent("LevelNameText")] TextMeshProUGUI m_LevelNameText = null;
    [UIComponent("LevelText")] TextMeshProUGUI m_LevelText = null;

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

    protected override void OnDestroy()
    {
        GameObject.DestroyImmediate(m_Elems.gameObject, true);
    }
}

public class PlayerRankUI : CustomUIComponent
{
    protected override string m_ViewResourceName => "GuildSaberProfile.UI.Card.View.PlayerRankUI.bsml";

    #region UIComponents
    [UIComponent("ElemsLayout")] HorizontalLayoutGroup m_Elems = null;
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    [UIComponent("CategoryText")] TextMeshProUGUI m_CategoryText = null;
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    [UIComponent("PlayerRankText")] TextMeshProUGUI m_PlayerRankText = null;
    [UIComponent("Hastag")] TextMeshProUGUI m_Hastag = null;
    #endregion

    #region Properties
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    // ReSharper disable once MemberInitializerValueIgnored
    public string PointsName { get; set; }
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    // ReSharper disable once MemberInitializerValueIgnored
    public string PlayerRank { get; set; }
    #endregion

    protected override void OnDestroy()
    {
        GameObject.DestroyImmediate(m_Elems.gameObject, true);
    }

    #region Setup
    protected override void AfterViewCreation()
    {
        if (m_Hastag != null && PointsName.IsNullOrEmpty() && PlayerRank.IsNullOrEmpty())
            GameObject.DestroyImmediate(m_Hastag.gameObject);

        Color l_PlayerColor = PlayerCardUI.m_Player.ProfileColor.ToUnityColor();
        Color l_BeforePlayerColor = new Color(l_PlayerColor.r * 0.8f, l_PlayerColor.g * 0.8f, l_PlayerColor.b * 0.8f);
        Color l_NewPlayerColor = new Color(l_PlayerColor.r * 1.2f, l_PlayerColor.g * 1.2f, l_PlayerColor.b * 1.2f);

        VertexGradient l_TextGradient = new VertexGradient(l_BeforePlayerColor, l_BeforePlayerColor, l_NewPlayerColor, l_NewPlayerColor);

        m_PlayerRankText.enableVertexGradient = true;
        m_CategoryText.enableVertexGradient = true;
        m_PlayerRankText.colorGradient = l_TextGradient;
        m_CategoryText.colorGradient = l_TextGradient;
    }
    #endregion
}

public class PlayerCardUI
{
    public static PlayerCardUI m_Instance;

    public static PlayerApiReworkOutput m_Player;

    public static TimeManager m_TimeManager;

    #region Properties
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public PlayerCardViewController CardViewController;
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public FloatingScreen FloatingScreen;
    #endregion

    #region Setup
    public static PlayerCardUI CreateCard()
    {

        PlayerGuildsInfo l_Player = GuildApi.GetPlayerInfoFromAPI();

        if (l_Player.Equals(default(PlayerGuildsInfo))) { Plugin.Log.Error("Failed Getting Player Info"); return null; }

        m_Player = l_Player.m_ReturnPlayer;

        new PlayerCardUI(Plugin.AvailableGuilds);

        return m_Instance;
    }

    public static void SetCardActive(bool p_Active)
    {
        if (m_Instance == null) return;

        if (m_Instance.CardViewController != null) m_Instance.CardViewController.gameObject.SetActive(p_Active);

        m_Instance.FloatingScreen.gameObject.SetActive(p_Active);
    }

    public PlayerCardUI(List<object> p_AvailableGuilds)
    {

        Plugin.Log.Info("Loading Player Card");

        if (m_TimeManager == null)
        {
            m_TimeManager = new GameObject("CardPlayTime").AddComponent<TimeManager>();
            //If i put Object it will do an "ambïgue" reference Between System.object and UnityEngine.Object
            GameObject.DontDestroyOnLoad(m_TimeManager);
        }
        CardViewController = BeatSaberUI.CreateViewController<PlayerCardViewController>();
        FloatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(40f, 40f), true, GSConfig.Instance.CardPosition.ToUnityVector3(), GSConfig.Instance.CardRotation.ToUnityQuat());
        FloatingScreen.HighlightHandle = true;
        FloatingScreen.HandleSide = FloatingScreen.Side.Right;
        FloatingScreen.HandleReleased += OnCardHandleReleased;

        CardViewController.SetReferences(FloatingScreen);
        CardViewController.m_AvailableGuilds = p_AvailableGuilds;

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
    #endregion

    #region Updates
    public void UpdateCardHandleVisibility()
    {
        if (FloatingScreen == null) return;
        FloatingScreen.ShowHandle = GSConfig.Instance.CardHandleVisible;
        FloatingScreen.UpdateHandle();
    }
    // ReSharper disable once MemberCanBePrivate.Global
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
    #endregion

    public static void RefreshCard(bool p_RegetPlayerInfo)
    {
        if (p_RegetPlayerInfo == true)
        {
            if (m_Instance == null) { CreateCard(); return; }

            PlayerGuildsInfo l_Player = GuildApi.GetPlayerInfoFromAPI();
            if (l_Player.Equals(null)) { if (m_Instance != null) SetCardActive(false); return; }
            m_Player = l_Player.m_ReturnPlayer;

            m_Instance.CardViewController.Refresh();
        }

        m_Instance.UpdateCardPosition();
        m_Instance.UpdateCardVisibility();
        m_Instance.UpdateCardHandleVisibility();
    }

    #region Events
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
    #endregion

    #region Reset
    public static void ResetMenuCardPosition()
    {
        GSConfig.Instance.CardPosition = SerializableVector3.FromUnityVector3(GSConfig.ConfigDefaults.DefaultCardPosition);
        GSConfig.Instance.CardRotation = SerializableQuaternion.FromUnityQuat(GSConfig.ConfigDefaults.DefaultCardRotation);
        if (PlayerCardUI.m_Instance != null)
            PlayerCardUI.m_Instance.UpdateCardPosition();
    }
    public static void ResetInGameCardPosition()
    {
        GSConfig.Instance.InGameCardPosition = SerializableVector3.FromUnityVector3(GSConfig.ConfigDefaults.DefaultInGameCardPosition);
        GSConfig.Instance.InGameCardRotation = SerializableQuaternion.FromUnityQuat(GSConfig.ConfigDefaults.DefaultInGameCardRotation);
        if (BeatSaberPlus.SDK.Game.Logic.ActiveScene == BeatSaberPlus.SDK.Game.Logic.SceneType.Playing && PlayerCardUI.m_Instance != null)
            PlayerCardUI.m_Instance.UpdateCardPosition();
    }
    #endregion

    #region Destroying Card
    [Obsolete("Provoque une fuite de mémoire")]
    public void Destroy()
    {
        GameObject.DestroyImmediate(CardViewController.gameObject);
        GameObject.DestroyImmediate(FloatingScreen.gameObject);
    }

    [Obsolete("Provoque des fuites de mémoire use PlayerCardUI.m_Instance.SetCardActive() Instead")]
    public static void DestroyCard()
    {
        if (m_Instance != null && m_Instance.CardViewController != null)
        {
            m_Instance.Destroy();
        }
    }
    #endregion
}