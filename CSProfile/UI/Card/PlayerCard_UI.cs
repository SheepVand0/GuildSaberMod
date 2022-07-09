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

    public PlayerCardViewController _CardViewController;
    public FloatingScreen _FloatingScreen;

    public PlayerCard_UI(PlayerApiReworkOutput p_Player)
    {
        Plugin.Log.Info("Loading Player Card");

        _CardViewController = BeatSaberUI.CreateViewController<PlayerCardViewController>();
        _FloatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(40f, 40f), true, PluginConfig.Instance.CardPosition, PluginConfig.Instance.CardRotation);
        _FloatingScreen.HighlightHandle = true;
        _FloatingScreen.HandleSide = FloatingScreen.Side.Right;
        _FloatingScreen.HandleReleased += OnCardHandleReleased;

        _CardViewController.SetReferences(p_Player, _FloatingScreen);

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
    }

    public void UpdateAll()
    {
        _CardViewController.UpdateLevelsDetails();
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
        _FloatingScreen.gameObject.SetActive(PluginConfig.Instance.ShowCard);
    }

    public void Destroy()
    {
        Object.Destroy(_FloatingScreen.gameObject);
        Object.Destroy(_CardViewController.gameObject);
    }
}