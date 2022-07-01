using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using BSDiscordRanking.Formats.API;
using UnityEngine;
using UnityEngine.UI;
using CSProfile.Utils;
using CSProfile.Configuration;
using TMPro;
using CSProfile.Time;
using IPA.Utilities;


namespace CSProfile.UI.Card
{
    public class PlayerLevelUI
    {
        public string levelName = "Vibro/Tech/Streams/Jumps/Shitpost";
        public string level = "31";

        public PlayerLevelUI(string p_levelName, string p_level)
        {
            levelName = p_levelName;
            level = p_level;
        }
    }

    public partial class PlayerCard_UI
    {
        public FloatingScreen _floatingScreen;

        public PlayerCardViewController _cardViewController;
        public void UpdateCardHandleVisibility()
        {
            if (_floatingScreen != null)
            {
                _floatingScreen.ShowHandle = PluginConfig.Instance.m_cardHandleVisible;
                _floatingScreen.UpdateHandle();
            }
        }

        public PlayerCard_UI(PlayerApiReworkOutput p_player)
        {
            Plugin.Log.Info("Loading Player Card");

            if (_cardViewController == null)
                _cardViewController = BeatSaberUI.CreateViewController<PlayerCardViewController>();

            _floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(40, 40f), true, PluginConfig.Instance.m_cardPosition, PluginConfig.Instance.m_cardRotation);
            _floatingScreen.HighlightHandle = true;
            _floatingScreen.HandleSide = FloatingScreen.Side.Right;
            _floatingScreen.HandleReleased += OnCardHandleReleased;

            _cardViewController.SetReferences(p_player, _floatingScreen);
            _cardViewController.firstLineLevels = new List<PlayerLevelUI>() {
                new PlayerLevelUI("Streams : ", p_player.CategoryData[0].Level.ToString()),
                new PlayerLevelUI("Vibro : ", p_player.CategoryData[1].Level.ToString()),
                new PlayerLevelUI("Tech : ", p_player.CategoryData[2].Level.ToString()),
            };

            _cardViewController.secondLineLevels = new List<PlayerLevelUI>() {
                new PlayerLevelUI("Jumps : ", p_player.CategoryData[3].Level.ToString()),
                new PlayerLevelUI("Shitpost : ", p_player.CategoryData[4].Level.ToString()),
            };

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

    public partial class PlayerCard_UI
    {

    }

    
}