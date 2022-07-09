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
<<<<<<< Updated upstream

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
=======

    public PlayerLevelUI(string p_LevelName, string p_Level)
    {
        LevelName = p_LevelName;
        Level = p_Level;
    }
}

public class PlayerCard_UI
{

    public PlayerCardViewController _CardViewController;
    public FloatingScreen _FloatingScreen;
>>>>>>> Stashed changes

    public PlayerCard_UI(PlayerApiReworkOutput p_Player)
    {
        Plugin.Log.Info("Loading Player Card");

<<<<<<< Updated upstream
        //if (m_CardViewController == null)
        CardViewController = BeatSaberUI.CreateViewController<PlayerCardViewController>();

        FloatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(40, 40f), true, PluginConfig.Instance.CardPosition, PluginConfig.Instance.CardRotation);
        FloatingScreen.HighlightHandle = true;
        FloatingScreen.HandleSide = FloatingScreen.Side.Right;
        FloatingScreen.HandleReleased += OnCardHandleReleased;
=======
        _CardViewController = BeatSaberUI.CreateViewController<PlayerCardViewController>();
        _FloatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(40f, 40f), true, PluginConfig.Instance.CardPosition, PluginConfig.Instance.CardRotation);
        _FloatingScreen.HighlightHandle = true;
        _FloatingScreen.HandleSide = FloatingScreen.Side.Right;
        _FloatingScreen.HandleReleased += OnCardHandleReleased;

        _CardViewController.SetReferences(p_Player, _FloatingScreen);
>>>>>>> Stashed changes

        CardViewController.SetReferences(p_Player, FloatingScreen);

<<<<<<< Updated upstream
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
=======
        bool l_UseALot = false;

        if (l_UseALot == false)
        {
            foreach (CustomApiPlayerCategory l_Category in p_Player.CategoryData)
            {
                _CardViewController.Levels.Add(new PlayerLevelUI(l_Category.Category, l_Category.Level.ToString()));
            }
        }
        else
        {
            for (int l_I = 0; l_I < 50; l_I++)
            {
                _CardViewController.Levels.Add(new PlayerLevelUI("Vibro", "31"));
            }
        }

        _FloatingScreen.SetRootViewController(_CardViewController, ViewController.AnimationType.None);

        Object.DontDestroyOnLoad(_FloatingScreen);
        Object.DontDestroyOnLoad(_CardViewController);

        UpdateAll();
    }
    public void UpdateCardHandleVisibility()
    {
        if (_FloatingScreen == null) return;
        _FloatingScreen.ShowHandle = PluginConfig.Instance.CardHandleVisible;
        _FloatingScreen.UpdateHandle();
>>>>>>> Stashed changes
    }

    public void UpdateAll()
    {
<<<<<<< Updated upstream
        CardViewController.UpdateLevelsDetails();
=======
        _CardViewController.UpdateLevelsDetails();
>>>>>>> Stashed changes
        UpdateCardHandleVisibility();
        UpdateCardVisibility();
    }

<<<<<<< Updated upstream
    public void OnCardHandleReleased(object p_Sender, FloatingScreenHandleEventArgs p_EventArgs)
=======
    private static void OnCardHandleReleased(object p_Sender, FloatingScreenHandleEventArgs p_EventArgs)
>>>>>>> Stashed changes
    {
        PluginConfig.Instance.CardPosition = p_EventArgs.Position;
        PluginConfig.Instance.CardRotation = p_EventArgs.Rotation;
    }

    public void UpdateCardVisibility()
    {
<<<<<<< Updated upstream
        FloatingScreen.gameObject.SetActive(PluginConfig.Instance.ShowCard);
=======
        _FloatingScreen.gameObject.SetActive(PluginConfig.Instance.ShowCard);
>>>>>>> Stashed changes
    }

    public void Destroy()
    {
<<<<<<< Updated upstream
        Object.Destroy(FloatingScreen.gameObject);
        Object.Destroy(CardViewController.gameObject);
=======
        Object.Destroy(_FloatingScreen.gameObject);
        Object.Destroy(_CardViewController.gameObject);
>>>>>>> Stashed changes
    }
}
