using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using BSDiscordRanking.Formats.API;
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

    public partial class PlayerCard_UI
    {
        public FloatingScreen _floatingScreen;

        public PlayerCardViewController _cardViewController;
        public void UpdateCardHandleVisibility()
        {
            if (_floatingScreen == null) return;
            _floatingScreen.ShowHandle = PluginConfig.Instance.m_cardHandleVisible;
            _floatingScreen.UpdateHandle();
        }

        public PlayerCard_UI(PlayerApiReworkOutput p_player)
        {
            Plugin.Log.Info("Loading Player Card");

            if (_cardViewController == null)
                _cardViewController = BeatSaberUI.CreateViewController<PlayerCardViewController>();

            _floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(40f, 40f), true, PluginConfig.Instance.m_cardPosition, PluginConfig.Instance.m_cardRotation);
            _floatingScreen.HighlightHandle = true;
            _floatingScreen.HandleSide = FloatingScreen.Side.Right;
            _floatingScreen.HandleReleased += OnCardHandleReleased;

            _cardViewController.SetReferences(p_player, _floatingScreen);

            foreach (var l_category in p_player.CategoryData)
            {
                _cardViewController.Levels.Add(new PlayerLevelUI(l_category.Category, l_category.Level.ToString()));
            }

            _floatingScreen.SetRootViewController(_cardViewController, HMUI.ViewController.AnimationType.None);

            GameObject.DontDestroyOnLoad(_floatingScreen);
            GameObject.DontDestroyOnLoad(_cardViewController);

            UpdateAll();
        }

        public void UpdateAll()
        {
            _cardViewController.UpdateLevelsDetails();
            UpdateCardHandleVisibility();
            UpdateCardVisibility();
        }

        private void OnCardHandleReleased(object sender, FloatingScreenHandleEventArgs e)
        {
            PluginConfig.Instance.m_cardPosition = e.Position;
            PluginConfig.Instance.m_cardRotation = e.Rotation;
        }

        public void UpdateCardVisibility()
        {
            _floatingScreen.gameObject.SetActive(PluginConfig.Instance.m_showCard);
        }

        public void Destroy()
        {
            GameObject.Destroy(_floatingScreen.gameObject);
            GameObject.Destroy(_cardViewController.gameObject);
        }
    }
}
