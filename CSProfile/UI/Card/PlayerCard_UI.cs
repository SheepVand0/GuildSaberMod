using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using CSProfile.API;
using UnityEngine;
using CSProfile.Configuration;


namespace CSProfile.UI.Card
{
    public class PlayerLevelUI
    {
        public string LevelName = "Vibro/Tech/Streams/Jumps/Shitpost";
        public string Level = "31";

        public PlayerLevelUI(string p_levelName, string p_level)
        {
            LevelName = p_levelName;
            Level = p_level;
        }
    }

    public class PlayerCard_UI
    {
        public FloatingScreen _FloatingScreen;

        public PlayerCardViewController _CardViewController;
        public void UpdateCardHandleVisibility()
        {
            if (_FloatingScreen == null) return;
            _FloatingScreen.ShowHandle = PluginConfig.Instance.CardHandleVisible;
            _FloatingScreen.UpdateHandle();
        }

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
                foreach (var l_Category in p_Player.CategoryData)
                {
                    _CardViewController.Levels.Add(new PlayerLevelUI(l_Category.Category, l_Category.Level.ToString()));
                }
            } else
            {
               for (int l_i = 0;l_i < 50;l_i++)
               {
                   _CardViewController.Levels.Add(new PlayerLevelUI("Vibro", "31"));
               }
            }

            _FloatingScreen.SetRootViewController(_CardViewController, HMUI.ViewController.AnimationType.None);

            GameObject.DontDestroyOnLoad(_FloatingScreen);
            GameObject.DontDestroyOnLoad(_CardViewController);

            UpdateAll();
        }

        public void UpdateAll()
        {
            _CardViewController.UpdateLevelsDetails();
            UpdateCardHandleVisibility();
            UpdateCardVisibility();
        }

        private void OnCardHandleReleased(object p_Sender, FloatingScreenHandleEventArgs p_Event)
        {
            PluginConfig.Instance.CardPosition = p_Event.Position;
            PluginConfig.Instance.CardRotation = p_Event.Rotation;
        }

        public void UpdateCardVisibility()
        {
            _FloatingScreen.gameObject.SetActive(PluginConfig.Instance.ShowCard);
        }

        public void Destroy()
        {
            GameObject.Destroy(_FloatingScreen.gameObject);
            GameObject.Destroy(_CardViewController.gameObject);
        }
    }
}
