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
using HMUI;
using CSProfile.API;
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

    #region ViewController
    //PlayerCard variables
    [HotReload(RelativePathToLayout = @"PlayerCard_UI.bsml")]
    [ViewDefinition("CSProfile.UI.Card.View.PlayerCard_UI.bsml")]
    public partial class PlayerCardViewController : BSMLAutomaticViewController
    {
        public TimeManager m_TimeManager;

        public FloatingScreen m_CardScreen;
        public string PlayerName { get { return m_PlayerInfo.Name; } set { } }
        public string PlayerGlobalLevel { get { return m_PlayerInfo.Level.ToString(); } set { } }
        public string PlayerImageSrc { get { return m_PlayerInfo.ProfilePicture; } set { } }
        public string PlayerNumberOfPasses
        {
            get
            {
                m_NumberOfPasses = 0;
                for (int l_i = 0; l_i < m_PlayerInfo.CategoryData.Count; l_i++)
                    m_NumberOfPasses = m_NumberOfPasses + m_PlayerInfo.CategoryData[l_i].NumberOfPass;
                PlayerNumberOfPasses = m_NumberOfPasses.ToString();
                return m_NumberOfPasses.ToString();
            }
            set { }
        }

        public List<PlayerLevelUI> Levels = new List<PlayerLevelUI>();

        PlayerApiReworkOutput m_PlayerInfo = new PlayerApiReworkOutput();

        int m_NumberOfPasses = 0;

        [UIComponent("playerNameText")] public TextMeshProUGUI m_PlayerNameText = null;
        [UIComponent("playerRankText")] TextMeshProUGUI m_PlayerRankText = null;
        [UIComponent("playTimeText")] TextMeshProUGUI m_PlayTimeText = null;
        [UIComponent("detailsLevelsLayout")] GridLayoutGroup m_DetailsLevelsLayout = null;
        [UIComponent("neonBackground")] Transform m_NeonBackground = null;
        [UIComponent("elemGrid")] GridLayoutGroup m_ElementsGrid = null;

        #region Main Card Info and style Loading
        [UIAction("#post-parse")]
        public void PostParse()
        {
            m_PlayerRankText.SetText(m_PlayerInfo.RankData[0].Rank.ToString());

            UnityEngine.Color l_playerColor = m_PlayerInfo.ProfileColor.ToUnityColor();
            UnityEngine.Color l_beforePlayerColor = new UnityEngine.Color(l_playerColor.r * 0.8f, l_playerColor.g * 0.8f, l_playerColor.b * 0.8f);
            UnityEngine.Color l_newPlayerColor = new UnityEngine.Color(l_playerColor.r * 1.2f, l_playerColor.g * 1.2f, l_playerColor.b * 1.2f);

            m_PlayerNameText.enableVertexGradient = true;
            m_PlayerRankText.enableVertexGradient = true;
            m_PlayerNameText.colorGradient = new VertexGradient(l_beforePlayerColor, l_beforePlayerColor, l_newPlayerColor, l_newPlayerColor);
            m_PlayerRankText.colorGradient = new VertexGradient(l_beforePlayerColor, l_beforePlayerColor, l_newPlayerColor, l_newPlayerColor);

            m_TimeManager = gameObject.AddComponent<TimeManager>();
            m_TimeManager.SetPlayerCardViewControllerRef(this);

            Plugin.Log.Info("________________________________");
            ImageView l_CurrentImageView = m_NeonBackground.GetComponentInChildren<ImageView>();

            l_CurrentImageView.SetField("_skew", 0.0f);
            l_CurrentImageView.overrideSprite = null;
            l_CurrentImageView.SetImage("#RoundRect10BorderFade");

            UnityEngine.Color l_divideColor = l_CurrentImageView.color;
            l_CurrentImageView.color0 = l_beforePlayerColor.ColorWithAlpha(1f);
            l_CurrentImageView.color1 = l_newPlayerColor.ColorWithAlpha(1f);
            l_CurrentImageView.color = l_playerColor.ColorWithAlpha(1f);

            l_CurrentImageView.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;

            Plugin.Log.Info("Card loaded");
        }
        #endregion

        #region Card Updates
        public void UpdateLevelsDetails()
        {
            m_DetailsLevelsLayout.gameObject.SetActive(PluginConfig.Instance.ShowDetaislLevels);

            if (m_CardScreen == null)
                return;

            float l_LevelsSize = Levels.Count;
            if (PluginConfig.Instance.ShowDetaislLevels == true)
            {

                //When the details levels is visible

                m_CardScreen.ScreenSize = new Vector2((62+m_PlayerInfo.Name.Length+l_LevelsSize)*1.02f, 28+(l_LevelsSize/3));
                m_ElementsGrid.cellSize = new Vector2((46+m_PlayerInfo.Name.Length+l_LevelsSize)*1.17f, 40);
                m_ElementsGrid.spacing = new Vector2(7, 7);
            }
            else
            {
                //When the detaisl levels is hidden
                m_CardScreen.ScreenSize = new Vector2(33+m_PlayerInfo.Name.Length, 28);
                m_ElementsGrid.cellSize = new Vector2(25+m_PlayerInfo.Name.Length, 40);
                m_ElementsGrid.spacing = new Vector2(1, 7);
            }
        }

        public void UpdateTime(OptimizedDateTime p_time)
        {
            m_PlayTimeText.text = PluginConfig.Instance.ShowPlayTime ? String.Join(":", p_time.m_Hours.ToString("00"), p_time.m_Minutes.ToString("00"), p_time.m_Seconds.ToString("00")) : " ";
        }
        #endregion

        #region References
        public void SetReferences(PlayerApiReworkOutput p_player, FloatingScreen p_cardScreen)
        {
            m_PlayerInfo = p_player;
            m_CardScreen = p_cardScreen;
        }
        #endregion
    }
    #endregion
}
