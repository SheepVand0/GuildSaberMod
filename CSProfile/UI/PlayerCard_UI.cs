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
using CSProfile.API;
using BSDiscordRanking.Formats.API;
using UnityEngine;
using UnityEngine.UI;
using CSProfile.Utils;
using TMPro;

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

        public GameObject m_cardGo;

        public GameObject m_uiGo;

        public Canvas m_cardCanvas;

        public string playerName { get { return m_playerInfo.Name; } set { } }
        public string playerGlobalLevel { get { return m_playerInfo.Level.ToString(); } set { } }
        public string playerNumberOfPasses { get {
                return m_numberOfPasses.ToString();

            } set { } }

        public string playerRank { get { return m_playerInfo.RankData[0].Rank.ToString(); } set { } }

        public string playerImageSrc { get { return m_playerInfo.ProfilePicture; } set { } }

        public List<PlayerLevelUI> firstLineLevels = new List<PlayerLevelUI>();

        public List<PlayerLevelUI> secondLineLevels = new List<PlayerLevelUI>();

        string playerColor { get { return "#ffffff"; } set { } }

        int m_numberOfPasses = 0;

        PlayerApiReworkOutput m_playerInfo = new PlayerApiReworkOutput();

        [UIComponent("playerNameText")] TextMeshProUGUI m_playerNameText = null;
        [UIComponent("playerRankText")] TextMeshProUGUI m_playerRankText = null;
        //[UIComponent("background")] Backgroundable m_background = null;

        public PlayerCard_UI(PlayerApiReworkOutput p_player)
        {
            m_playerInfo = p_player;

            m_cardGo = new GameObject("CSCardGameObject");
            m_cardGo.transform.localPosition = new Vector3(0,3,0);
            m_cardCanvas = m_cardGo.AddComponent<Canvas>();
            m_cardCanvas.renderMode = RenderMode.WorldSpace;
            m_cardCanvas.scaleFactor = 100.0f;
            m_cardCanvas.transform.localScale = 0.1f * Vector3.one;

            m_cardGo.AddComponent<CanvasScaler>().scaleFactor = 1.0f;
            m_cardGo.AddComponent<GraphicRaycaster>();

            RectTransform l_clockRectTransform = m_cardGo.GetComponent<RectTransform>();
            l_clockRectTransform.localPosition = Vector3.zero;
            l_clockRectTransform.sizeDelta = new Vector2(20, 10);
            l_clockRectTransform.rect.Set(0, 0, 4, 2);

            m_uiGo = new GameObject("playerCardUIGo");
            m_uiGo.transform.parent = m_cardGo.transform;
            m_uiGo.transform.localPosition = new Vector3(0,3,0);
            m_uiGo.transform.localScale = 0.1f * Vector3.one;

            GameObject.DontDestroyOnLoad(m_cardGo);
            GameObject.DontDestroyOnLoad(m_uiGo);

            Plugin.Log.Info("Loading player card Elements");

            m_numberOfPasses = 0;

            for (int l_i = 0; l_i < m_playerInfo.CategoryData.Count; l_i++)
            {
                m_numberOfPasses = m_numberOfPasses + m_playerInfo.CategoryData[l_i].NumberOfPass;
            }

            playerNumberOfPasses = m_numberOfPasses.ToString();

            playerRank = "#" + m_playerInfo.RankData[0].Rank.ToString();

            firstLineLevels = new List<PlayerLevelUI>() {
                new PlayerLevelUI("Streams : ", m_playerInfo.CategoryData[0].Level.ToString()),
                new PlayerLevelUI("Vibro : ", m_playerInfo.CategoryData[1].Level.ToString()),
                new PlayerLevelUI("Tech : ", m_playerInfo.CategoryData[2].Level.ToString()),
            };

            secondLineLevels = new List<PlayerLevelUI>() {
                new PlayerLevelUI("Jumps : ", m_playerInfo.CategoryData[3].Level.ToString()),
                new PlayerLevelUI("Shitpost : ", m_playerInfo.CategoryData[4].Level.ToString()),
            };

            UnityEngine.Color l_playerColor = m_playerInfo.ProfileColor.ToUnityColor();
            UnityEngine.Color l_beforePlayerColor = new UnityEngine.Color(l_playerColor.r * 0.8f, l_playerColor.g * 0.8f, l_playerColor.b * 0.8f);
            UnityEngine.Color l_newPlayerColor = new UnityEngine.Color(l_playerColor.r * 1.2f, l_playerColor.g * 1.2f, l_playerColor.b * 1.2f);

            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), $"CSProfile.UI.PlayerCard_UI.bsml"), m_uiGo, this);

            m_playerNameText.enableVertexGradient = true;
            m_playerRankText.enableVertexGradient = true;
            m_playerNameText.colorGradient = new VertexGradient(l_beforePlayerColor, l_beforePlayerColor, l_newPlayerColor, l_newPlayerColor);
            m_playerRankText.colorGradient = new VertexGradient(l_beforePlayerColor, l_beforePlayerColor, l_newPlayerColor, l_newPlayerColor);
        }

        [UIAction("#post-parse")]
        public void PostParse()
        {
            Plugin.Log.Info("Card loaded");
        }
    }
}
