using System.Collections.Generic;
using UnityEngine.UI;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaber.API;
using GuildSaber.BSPModule;
using GuildSaber.Utils;
using HMUI;
using TMPro;
using UnityEngine;

namespace GuildSaber.UI.Leaderboard.Components.SubComponents
{
    internal class LeaderboardScoreCell
    {
        [UIComponent("Elems")] public HorizontalLayoutGroup m_ElemsLayout = null;
        [UIComponent("Interactable")] public Button m_Interactable = null;
        [UIComponent("Info")] public ModalView m_InfoModal = null;

        [UIValue("Rank")] private CustomText m_CRank = null;
        [UIValue("Name")] private CustomText m_CPlayerName = null;
        [UIValue("Points")] private CustomText m_CPoints = null;
        [UIValue("ScoreText")] private CustomText m_CScore = null;
        [UIValue("AccText")] private CustomText m_CAcc = null;
        [UIValue("Modifiers")] private CustomText m_CModifiers = null;

        [UIComponent("ModalPlayerName")] private readonly TextMeshProUGUI m_ModalPlayerNameText = null;
        [UIComponent("ModalBadCuts")] private readonly TextMeshProUGUI m_ModalBadCutsText = null;
        [UIComponent("ModalMissedNotes")] private readonly TextMeshProUGUI m_ModalMissedNotesText = null;
        [UIComponent("ModalModifiedScore")] private readonly TextMeshProUGUI m_ModalModifiedScoreText = null;
        [UIComponent("ModalPassState")] private readonly TextMeshProUGUI m_ModalPassState = null;
        [UIComponent("ModalPauseCount")] private readonly TextMeshProUGUI m_ModalPauseCount = null;
        [UIComponent("ModalHMD")] private readonly TextMeshProUGUI m_ModalHMD = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private static readonly Color s_Blue = new(0f, 0.8f, 1f, 0.8f);

        public static float ScaleFactor
        {
            get => 1.2f;
        }
        private float InteractableScaleY
        {
            get => (6.5f * ScaleFactor) * 0.9f;
        }
        public static float ScoreFontSize
        {
            get => 2.5f * ScaleFactor;
        }
        private float LeaderWidth
        {
            get => 90 * ScaleFactor + (7 * 1.4f);
        }

        public bool HasBeenParsed { get; private set; } = false;

        float BasePoints { get; set; } = 0f;
        public string Rank { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Points { get; set; } = string.Empty;
        public string Score { get; set; } = string.Empty;
        public string Acc { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Modifiers { get; set; } = string.Empty;

        public int BadCuts { get; set; } = 0;
        public int MissedNotes { get; set; } = 0;
        public int ModifiedScore { get; set; } = 0;
        public int? Pauses { get; set; } = 0;
        public EHMD HMD { get; set; } = EHMD.Unk;
        public PassState.EState PassState { get; set; } = API.PassState.EState.Allowed;

        private List<string> m_BannedModifiers = new List<string>();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Update cell on leaderboard refresh
        /// </summary>
        /// <param name="p_Rank">Player rank in this map</param>
        /// <param name="p_Name">player name</param>
        /// <param name="p_Points">Points</param>
        /// <param name="p_PointsName">Selected points type name</param>
        /// <param name="p_Score">Score</param>
        /// <param name="p_Acc">Acc</param>
        /// <param name="p_Id">Player GuildSaber Id</param>
        /// <param name="p_Modfiers">Modifiers</param>
        internal void Init(int p_Rank, string p_Name, float p_Points, string p_PointsName, int p_Score, float p_Acc, string p_Id, string p_Modfiers)
        {
            Rank = $"{p_Rank}";
            PlayerName = p_Name;
            BasePoints = p_Points;
            Points = $"{p_Points:0.00} {p_PointsName}";
            Score = p_Score.ToString();
            Acc = $"{p_Acc:00.00}%";
            Id = p_Id;
            Modifiers = GuildSaberUtils.GetPlayerNameToFit(p_Modfiers, 4);
            Show();
        }

        /// <summary>
        /// Set Score Modal Information
        /// </summary>
        /// <param name="p_BadCuts"></param>
        /// <param name="p_MissedNotes"></param>
        /// <param name="p_Pauses"></param>
        /// <param name="p_ModifiedScore"></param>
        /// <param name="p_BannedModifiers"></param>
        /// <param name="p_PassState"></param>
        /// <param name="p_Hmd"></param>
        internal void SetModalInfo(int p_BadCuts, int p_MissedNotes, int? p_Pauses, int p_ModifiedScore, List<string> p_BannedModifiers, PassState.EState p_PassState, EHMD p_Hmd)
        {
            BadCuts = p_BadCuts;
            MissedNotes = p_MissedNotes;
            ModifiedScore = p_ModifiedScore;
            Pauses = p_Pauses;
            PassState = p_PassState;
            m_BannedModifiers = p_BannedModifiers;
            HMD = p_Hmd;
        }

        /// <summary>
        /// PostParse
        /// </summary>
        [UIAction("#post-parse")]
        private void _PostParse()
        {
            List<ItemParam> l_CurrentParams = new List<ItemParam>()
            {
                new ItemParam("FontSize", ScoreFontSize),
                new ItemParam("AnchorPosX", 2f),
                new ItemParam("Alignment", TextAlignmentOptions.Left),
                new ItemParam("LayoutAlignment", TextAnchor.MiddleLeft),
                new ItemParam("Color", Color.white),
                new ItemParam("Italic", true),
                new ItemParam("RichText", string.Empty),
                new ItemParam("EnableRichText", true)
            };

            float l_OffsetFromRank = 7f;
            float l_NameOffset = -1f;

            Color l_StartColor = GuildSaberModule.LeaderboardSelectedGuild.Color.ToUnityColor();

            m_CRank = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", (3f + l_OffsetFromRank) + (ScaleFactor) * 0.7f);
            l_CurrentParams[7] = new ItemParam("EnableRichText", false);

            m_CPlayerName = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", (34f + l_OffsetFromRank + l_NameOffset) + (ScaleFactor * 1.02f));
            l_CurrentParams[4] = new ItemParam("Color", l_StartColor);
            l_CurrentParams[7] = new ItemParam("EnableRichText", true);

            m_CPoints = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", (-40f + l_OffsetFromRank + l_NameOffset) + (ScaleFactor * 1.04f));
            l_CurrentParams[2] = new ItemParam("Alignment", TextAlignmentOptions.Right);
            l_CurrentParams[4] = new ItemParam("Color", Color.white);
            // ReSharper disable once StringLiteralTypo
            l_CurrentParams[6] = new ItemParam("RichText", "<mspace=0.5em>");

            m_CScore = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[0] = new ItemParam("FontSize", ScoreFontSize / ScaleFactor);
            l_CurrentParams[1] = new ItemParam("AnchorPosX", (-32.5f + l_OffsetFromRank + l_NameOffset) + (ScaleFactor * 1.06f));
            l_CurrentParams[4] = new ItemParam("Color", Color.grey);
            l_CurrentParams[5] = new ItemParam("Italic", false);
            l_CurrentParams[6] = new ItemParam("RichText", string.Empty);

            m_CModifiers = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[0] = new ItemParam("FontSize", ScoreFontSize);
            l_CurrentParams[1] = new ItemParam("AnchorPosX", (-24f + l_OffsetFromRank + l_NameOffset) + (ScaleFactor * 1.06f));
            l_CurrentParams[4] = new ItemParam("Color", l_StartColor);

            m_CAcc = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            HasBeenParsed = true;

            Events.Instance.e_OnGuildSelected += OnGuildSelected;

            Reset();
        }

        /// <summary>
        /// Apply all values on texts
        /// </summary>
        internal void SetTexts()
        {
            m_CRank.SetText(Rank);
            m_CPlayerName.SetText(GuildSaberUtils.GetPlayerNameToFit(PlayerName, 22));
            m_CPoints.SetText(Points);

            m_CScore.SetText($"{m_CScore.RichText}{Score}");
            m_CAcc.SetText(Acc);
            m_CModifiers.SetText(Modifiers);
        }

        /// <summary>
        /// Reset cell colors, texts, and hide button
        /// </summary>
        internal async void Reset()
        {
            await WaitUtils.Wait(() => m_ElemsLayout.gameObject.activeInHierarchy, 10, p_CodeLine: 207);

            SetEmpty();
            Color l_White = Color.white;
            m_CRank.SetColor(l_White);
            m_CPlayerName.SetColor(l_White);
            m_CScore.SetColor(l_White);

            m_Interactable.gameObject.SetActive(false);
        }

        /// <summary>
        /// Set current cell to player to highlight it in blue
        /// </summary>
        internal void SetCellToCurrentPlayer()
        {
            m_CRank.SetColor(s_Blue);
            m_CPlayerName.SetColor(s_Blue);
            m_CScore.SetColor(s_Blue);
        }

        /// <summary>
        /// Show it when have a score
        /// </summary>
        internal void Show()
        {
            SetTexts();

            m_Interactable.gameObject.SetActive(true);
        }
        /// <summary>
        /// Set texts to empty
        /// </summary>
        public void SetEmpty()
        {
            Rank = string.Empty;
            PlayerName = string.Empty;
            Points = string.Empty;
            Score = string.Empty;
            Acc = string.Empty;
            Modifiers = string.Empty;

            m_CRank.SetText(string.Empty);
            m_CPlayerName.SetText(string.Empty);
            m_CPoints.SetText(string.Empty);
            m_CScore.SetText(string.Empty);
            m_CAcc.SetText(string.Empty);
            m_CModifiers.SetText(string.Empty);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private void OnGuildSelected(int p_Guild)
        {
            //m_CAcc, m_CPoints
            Color l_NewColor = GuildSaberModule.LeaderboardSelectedGuild.Color.ToUnityColor();
            m_CAcc.SetColor(l_NewColor);
            m_CPoints.SetColor(l_NewColor);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIAction("ShowInfo")]
        private void ShowInfo()
        {
            m_InfoModal.Show(true, true);
            m_ModalPlayerNameText.text = PlayerName;
            if (BadCuts == 0 && MissedNotes == 0)
            {
                m_ModalBadCutsText.color = Color.green;
                m_ModalBadCutsText.text = "Full Combo";
                m_ModalMissedNotesText.text = string.Empty;
            }
            else
            {
                // ReSharper disable once StringLiteralTypo
                m_ModalBadCutsText.text = $"<color=#adadad>Bad cuts : <color=#b50000>{BadCuts}</color>";
                // ReSharper disable once StringLiteralTypo
                m_ModalMissedNotesText.text = $"<color=#adadad>Missed Notes : <color=#b50000>{MissedNotes}</color>";
            }

            m_ModalModifiedScoreText.gameObject.SetActive(ModifiedScore != int.Parse(Score));
            m_ModalModifiedScoreText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}> Modified score : </color>{ModifiedScore}";
            if (Pauses != null)
                m_ModalPauseCount.text = $"<color=#{ColorUtility.ToHtmlStringRGB(Color.white)}>Pauses : <color=#{(Pauses == 0 ? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red))}>{Pauses}";
            else
                m_ModalPauseCount.text = "Pauses : ?";

            foreach (var l_BannedModifier in m_BannedModifiers)
            {
                if (!Modifiers.Contains(l_BannedModifier)) continue;

                PassState = API.PassState.EState.Denied;
            }

            m_ModalPassState.text = "Pass state : " + $"<color=#{API.PassState.GetColorFromPassState(PassState)}>{PassState}</color>";
            m_ModalHMD.text = $"Set on : {HMD}";
        }

        /// <summary>
        /// On Close Modal button pressed
        /// </summary>
        [UIAction("CloseModal")]
        private void CloseModal()
        {
            m_InfoModal.Hide(true);
        }


    }
}
