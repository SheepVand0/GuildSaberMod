using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using CSProfile.API;
using CSProfile.Configuration;
using HMUI;
using UnityEngine;

namespace CSProfile.UI.Card;

public class PlayerLevelUI
{
    public string Level = "31";
    public string LevelName = "Vibro/Tech/Streams/Jumps/Shitpost";

    public PlayerLevelUI(string p_LevelName, string p_Level)
    {
        LevelName = p_LevelName;
        Level = p_Level;
    }
}

public class PlayerCard_UI
{

    public PlayerCardViewController CardViewController;

    public FloatingScreen FloatingScreen;

    public PlayerCard_UI(PlayerApiReworkOutput p_Player)
    {
        Plugin.Log.Info("Loading Player Card");

        //if (m_CardViewController == null)
        CardViewController = BeatSaberUI.CreateViewController<PlayerCardViewController>();

        FloatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(40, 40f), true, PluginConfig.Instance.CardPosition, PluginConfig.Instance.CardRotation);
        FloatingScreen.HighlightHandle = true;
        FloatingScreen.HandleSide = FloatingScreen.Side.Right;
        FloatingScreen.HandleReleased += OnCardHandleReleased;

        CardViewController.SetReferences(p_Player, FloatingScreen);

        CardViewController.FirstLineLevels = new List<PlayerLevelUI>
        {
            new PlayerLevelUI("Streams : ", p_Player.CategoryData[0].Level.ToString()),
            new PlayerLevelUI("Vibro : ", p_Player.CategoryData[1].Level.ToString()),
            new PlayerLevelUI("Tech : ", p_Player.CategoryData[2].Level.ToString())
        };

        CardViewController.SecondLineLevels = new List<PlayerLevelUI>
        {
            new PlayerLevelUI("Jumps : ", p_Player.CategoryData[3].Level.ToString()),
            new PlayerLevelUI("Shitpost : ", p_Player.CategoryData[4].Level.ToString())
        };

        FloatingScreen.SetRootViewController(CardViewController, ViewController.AnimationType.None);

        Object.DontDestroyOnLoad(FloatingScreen);
        Object.DontDestroyOnLoad(CardViewController);

        Plugin.Log.Info("Done Loading Player Card");
        UpdateAll();
    }
    public void UpdateCardHandleVisibility()
    {
        if (FloatingScreen == null) return;

        FloatingScreen.ShowHandle = PluginConfig.Instance.CardHandleVisible;
        FloatingScreen.UpdateHandle();
    }

    public void UpdateAll()
    {
        CardViewController.UpdateLevelsDetails();
        UpdateCardHandleVisibility();
        UpdateCardVisibility();
    }

    public void OnCardHandleReleased(object p_Sender, FloatingScreenHandleEventArgs p_EventArgs)
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
