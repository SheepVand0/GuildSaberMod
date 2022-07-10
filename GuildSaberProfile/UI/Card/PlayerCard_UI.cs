using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaberProfile.API;
using GuildSaberProfile.Configuration;
using HMUI;
using UnityEngine;
using TMPro;

namespace GuildSaberProfile.UI.Card;

public class PlayerLevelUI
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once MemberInitializerValueIgnored
    public string Level = "31";
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once MemberInitializerValueIgnored
    public string LevelName = "Vibro/Tech/Streams/Jumps/Shitpost";
    // ReSharper disable once MemberInitializerValueIgnored
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public int FontSize = 3;

    public PlayerLevelUI(string p_LevelName, string p_Level, int p_LevelsCount)
    {
        LevelName = p_LevelName;
        Level = p_Level;
        FontSize = (int)(2 / (p_LevelsCount*0.1f));
        if (FontSize < 1) FontSize = 2;
    }
}

public class PlayerRankUI
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    [UIComponent("PlayerRankText")] private TextMeshProUGUI m_PlayerRankText = null;
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    [UIComponent("CategoryText")] private TextMeshProUGUI m_CategoryText = null;

    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    // ReSharper disable once MemberInitializerValueIgnored
    private string m_Category = string.Empty;
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    // ReSharper disable once MemberInitializerValueIgnored
    private string m_PlayerRank = string.Empty;

    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    PlayerApiReworkOutput m_Player;

    public PlayerRankUI(PlayerApiReworkOutput p_Player, string p_Category, string p_Rank)
    {
        m_Player = p_Player;
        m_Category = p_Category;
        m_PlayerRank = p_Rank;
    }

    [UIAction("#post-parse")]
    public void PostParse()
    {
        m_CategoryText.SetText(m_Category);
        m_PlayerRankText.SetText(m_PlayerRank);
        //m_PointsText.SetText(m_Points);

        Color l_PlayerColor = m_Player.ProfileColor.ToUnityColor();
        Color l_BeforePlayerColor = new Color(l_PlayerColor.r * 0.8f, l_PlayerColor.g * 0.8f, l_PlayerColor.b * 0.8f);
        Color l_NewPlayerColor = new Color(l_PlayerColor.r * 1.2f, l_PlayerColor.g * 1.2f, l_PlayerColor.b * 1.2f);

        VertexGradient l_TextGradient = new VertexGradient(l_BeforePlayerColor, l_BeforePlayerColor, l_NewPlayerColor, l_NewPlayerColor);

        m_PlayerRankText.enableVertexGradient = true;
        m_CategoryText.enableVertexGradient = true;
        m_PlayerRankText.colorGradient = l_TextGradient;
        m_CategoryText.colorGradient = l_TextGradient;
    }
}

public class PlayerCard_UI
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public PlayerCardViewController CardViewController;
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public FloatingScreen FloatingScreen;

    public PlayerCard_UI(PlayerApiReworkOutput p_Player)
    {
        Plugin.Log.Info("Loading Player Card");

        CardViewController = BeatSaberUI.CreateViewController<PlayerCardViewController>();
        FloatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(40f, 40f), true, PluginConfig.Instance.CardPosition, PluginConfig.Instance.CardRotation);
        FloatingScreen.HighlightHandle = true;
        FloatingScreen.HandleSide = FloatingScreen.Side.Right;
        FloatingScreen.HandleReleased += OnCardHandleReleased;

        CardViewController.SetReferences(p_Player, FloatingScreen);

        /// For debug purpose with lots of levels
        /*bool l_UseALot = true;

        if (!l_UseALot)
        {*/
            foreach (CustomApiPlayerCategory l_Category in p_Player.CategoryData)
            {
                CardViewController.Levels.Add(new PlayerLevelUI(l_Category.Category, l_Category.Level.ToString(), p_Player.CategoryData.Count));
            }
        /*}
        else
        {
            for (int l_i = 0; l_i < 50; l_i++)
            {
                CardViewController.Levels.Add(new PlayerLevelUI("Vibro", "31", 50));
            }
        }*/

        foreach (RankData l_RankData in p_Player.RankData)
        {
            CardViewController.Ranks.Add(new PlayerRankUI(p_Player, l_RankData.PointsName, l_RankData.Rank.ToString()));
        }

        FloatingScreen.SetRootViewController(CardViewController, ViewController.AnimationType.None);

        Object.DontDestroyOnLoad(FloatingScreen);
        Object.DontDestroyOnLoad(CardViewController);

        UpdateAll();
    }
    public void UpdateCardHandleVisibility()
    {
        if (FloatingScreen == null) return;
        FloatingScreen.ShowHandle = PluginConfig.Instance.CardHandleVisible;
        FloatingScreen.UpdateHandle();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public void UpdateAll()
    {
        CardViewController.UpdateLevelsDetails();
        UpdateCardHandleVisibility();
        UpdateCardVisibility();
    }

    private static void OnCardHandleReleased(object p_Sender, FloatingScreenHandleEventArgs p_EventArgs)
    {
        PluginConfig.Instance.CardPosition = p_EventArgs.Position;
        PluginConfig.Instance.CardRotation = p_EventArgs.Rotation;
    }

    public void UpdateCardVisibility()
    {
        switch (Plugin.CurrentSceneName) {
            case "MainMenu":
                FloatingScreen.gameObject.SetActive(PluginConfig.Instance.ShowCardInMenu);
                break;
            case "GameCore":
                FloatingScreen.gameObject.SetActive(PluginConfig.Instance.ShowCardInGame);
                break;
            default:
                Plugin.Log.Info(Plugin.CurrentSceneName);
                break;
        }
    }

    public void Destroy()
    {
        Object.Destroy(FloatingScreen.gameObject);
        Object.Destroy(CardViewController.gameObject);
    }
}
