using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using CSProfile.API;
using CSProfile.Configuration;
using HMUI;
using UnityEngine;

namespace CSProfile.UI.Card;

public class PlayerLevelUI
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once MemberInitializerValueIgnored
    public string Level = "31";
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once MemberInitializerValueIgnored
    public string LevelName = "Vibro/Tech/Streams/Jumps/Shitpost";

    public PlayerLevelUI(string p_LevelName, string p_Level)
    {
        LevelName = p_LevelName;
        Level = p_Level;
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

        foreach (CustomApiPlayerCategory l_Category in p_Player.CategoryData)
        {
            CardViewController.Levels.Add(new PlayerLevelUI(l_Category.Category, l_Category.Level.ToString()));
        }

        /// For debug purpose with lots of levels
        /*for (int l_I = 0; l_I < 50; l_I++)
        {
            _CardViewController.Levels.Add(new PlayerLevelUI("Vibro", "31"));
        }*/

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
        FloatingScreen.gameObject.SetActive(PluginConfig.Instance.ShowCard);
    }

    public void Destroy()
    {
        Object.Destroy(FloatingScreen.gameObject);
        Object.Destroy(CardViewController.gameObject);
    }
}
