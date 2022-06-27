using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.FloatingScreen;
using CSProfile.API;
using BSDiscordRanking.Formats.API;
using UnityEngine;
using UnityEngine.UI;
using CSProfile.Utils;
using CSProfile.Configuration;
using TMPro;
using CSProfile.Time;

namespace CSProfile.UI
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

    public class PlayerCard_UI
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
            { /*m_playerInfo = p_player;

            m_cardGo = new GameObject("CSCardGameObject");
            m_cardGo.transform.localPosition = Vector3.zero;
            m_cardCanvas = m_cardGo.AddComponent<Canvas>();
            m_cardCanvas.renderMode = RenderMode.WorldSpace;
            m_cardCanvas.scaleFactor = 100.0f;
            m_cardCanvas.transform.localScale = 0.1f * Vector3.one;
            
            m_cardGo.AddComponent<CanvasScaler>().scaleFactor = 1.0f;
            //m_cardGo.AddComponent<GraphicRaycaster>();

            RectTransform l_cardRectTransform = m_cardGo.GetComponent<RectTransform>();
            l_cardRectTransform.localPosition = Vector3.zero;
            l_cardRectTransform.sizeDelta = new Vector2(30, 40);
            l_cardRectTransform.rect.Set(0, 0, 8, 4);

            m_uiGo = new GameObject("PlayerCardUIGo");
            m_uiGo.transform.parent = m_cardGo.transform;
            m_uiGo.transform.localPosition = new Vector3(0, 5, 0);
            m_uiGo.transform.localScale = Vector3.one * 0.1f;

            GameObject.DontDestroyOnLoad(m_uiGo);
            GameObject.DontDestroyOnLoad(m_cardGo);*/
            }
            if (_cardViewController == null)
                _cardViewController = BeatSaberUI.CreateViewController<PlayerCardViewController>();

            _floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(40, 40f), true, PluginConfig.Instance.m_cardPosition, PluginConfig.Instance.m_cardRotation);
            _floatingScreen.HighlightHandle = true;
            _floatingScreen.HandleSide = FloatingScreen.Side.Right;
            _floatingScreen.HandleReleased += OnCardHandleReleased;

            Plugin.Log.Info("Loading player card Elements");

            _cardViewController.SetReferences(p_player, _floatingScreen);
            _cardViewController.m_playerColor = p_player.ProfileColor.ToUnityColor();
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

            UpdateCardHandleVisibility();
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

    [HotReload(RelativePathToLayout = @"PlayerCard_UI.bsml")]
    [ViewDefinition("CSProfile.UI.PlayerCard_UI.bsml")]
    public partial class PlayerCardViewController : BSMLAutomaticViewController
    {
        public UnityEngine.Color m_playerColor = UnityEngine.Color.green;
        public string playerColor { get { return "#" + UnityEngine.ColorUtility.ToHtmlStringRGB(m_playerColor); } set { } }
        public string playerName { get { return m_playerInfo.Name; } set { } }
        public string playerGlobalLevel { get { return m_playerInfo.Level.ToString(); } set { } }
        public string playerNumberOfPasses
        {
            get
            {
                m_numberOfPasses = 0;

                for (int l_i = 0; l_i < m_playerInfo.CategoryData.Count; l_i++)
                {
                    m_numberOfPasses = m_numberOfPasses + m_playerInfo.CategoryData[l_i].NumberOfPass;
                }

                playerNumberOfPasses = m_numberOfPasses.ToString();
                return m_numberOfPasses.ToString();
            }
            set { }
        }

        public string playerImageSrc { get { return m_playerInfo.ProfilePicture; } set { } }

        public List<PlayerLevelUI> firstLineLevels = new List<PlayerLevelUI>();

        public List<PlayerLevelUI> secondLineLevels = new List<PlayerLevelUI>();

        int m_numberOfPasses = 0;

        PlayerApiReworkOutput m_playerInfo = new PlayerApiReworkOutput();

        [UIComponent("playerNameText")] TextMeshProUGUI m_playerNameText = null;
        [UIComponent("playerRankText")] TextMeshProUGUI m_playerRankText = null;

        [UIAction("#post-parse")]
        public void PostParse()
        {
            m_playerRankText.SetText(m_playerInfo.RankData[0].Rank.ToString());
            
            UnityEngine.Color l_playerColor = m_playerInfo.ProfileColor.ToUnityColor();

            m_playerColor = l_playerColor;

            //playerColor = l_playerColor;
            Plugin.Log.Info(playerColor.ToString());

            UnityEngine.Color l_beforePlayerColor = new UnityEngine.Color(l_playerColor.r * 0.8f, l_playerColor.g * 0.8f, l_playerColor.b * 0.8f);
            UnityEngine.Color l_newPlayerColor = new UnityEngine.Color(l_playerColor.r * 1.2f, l_playerColor.g * 1.2f, l_playerColor.b * 1.2f);

            m_playerNameText.enableVertexGradient = true;
            m_playerRankText.enableVertexGradient = true;
            m_playerNameText.colorGradient = new VertexGradient(l_beforePlayerColor, l_beforePlayerColor, l_newPlayerColor, l_newPlayerColor);
            m_playerRankText.colorGradient = new VertexGradient(l_beforePlayerColor, l_beforePlayerColor, l_newPlayerColor, l_newPlayerColor);

            m_timeManager = gameObject.AddComponent<TimeManager>();
            m_timeManager.SetPlayerCardViewControllerRef(this);

            Plugin.Log.Info("Card loaded");
        }

    }

    public partial class PlayerCardViewController : BSMLAutomaticViewController
    {
        public string playTime { get { return "00:00:00"; } set { } }

        public TimeManager m_timeManager;

        public FloatingScreen m_cardScreen;

        [UIComponent("playTimeText")] TextMeshProUGUI m_playTimeText = null;
        [UIComponent("detailsLevelsLayout")] VerticalLayoutGroup m_detailsLevelsLayout = null;

        public void UpdateLevelsDetails()
        {
            m_detailsLevelsLayout.gameObject.SetActive(PluginConfig.Instance.m_showDetaislLevels);

            if (m_cardScreen == null)
                return;

            if (PluginConfig.Instance.m_showDetaislLevels == true)
                m_cardScreen.ScreenSize = new Vector2(40, 40);
            else
                m_cardScreen.ScreenSize = new Vector2(40, 30);
        }

        public void UpdateTime(OptimizedDateTime p_time)
        {
            m_playTimeText.text = PluginConfig.Instance.m_showPlayTime ? String.Join(":", p_time.m_Hours.ToString("00"), p_time.m_Minutes.ToString("00"), p_time.m_Seconds.ToString("00")) : " ";
        }

        public void SetReferences(PlayerApiReworkOutput p_player, FloatingScreen p_cardScreen)
        {
            m_playerInfo = p_player;
            m_cardScreen = p_cardScreen;
        }
    }
}
