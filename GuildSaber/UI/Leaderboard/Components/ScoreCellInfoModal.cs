using BeatLeader.Models;
using BeatLeader.Replayer;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using CP_SDK.Misc;
using GuildSaber.API;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Resources = UnityEngine.Resources;


#nullable enable
namespace GuildSaber.UI.Leaderboard.Components
{
    internal class ScoreCellInfoModal : CustomUIComponent
    {
        protected override string ViewResourceName => "GuildSaber.UI.Leaderboard.Components.Views.ScoreCellInfoModal.bsml";

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal static ScoreCellInfoModal Instance;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent("Info")] public ModalView m_InfoModal = (ModalView)null;

        [UIComponent("ModalBadCuts")] private readonly TextMeshProUGUI m_ModalBadCutsText = (TextMeshProUGUI)null;
        [UIComponent("ModalHMD")] private readonly TextMeshProUGUI m_ModalHMD = (TextMeshProUGUI)null;
        [UIComponent("ModalMissedNotes")] private readonly TextMeshProUGUI m_ModalMissedNotesText = (TextMeshProUGUI)null;
        [UIComponent("ModalModifiedScore")] private readonly TextMeshProUGUI m_ModalModifiedScoreText = (TextMeshProUGUI)null;
        [UIComponent("ModalPassState")] private readonly TextMeshProUGUI m_ModalPassState = (TextMeshProUGUI)null;
        [UIComponent("ModalPauseCount")] private readonly TextMeshProUGUI m_ModalPauseCount = (TextMeshProUGUI)null;
        [UIComponent("ModalPlayerName")] private readonly TextMeshProUGUI m_ModalPlayerNameText = (TextMeshProUGUI)null;
        [UIComponent("ModalReplay")] internal readonly Button m_ModalReplay = (Button)null;
        [UIComponent("ModalTimeSet")] private readonly TextMeshProUGUI m_ModalTimeSet = (TextMeshProUGUI)null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private List<string> m_BannedModifiers = new List<string>();

        public int BadCuts { get; set; }

        public int MissedNotes { get; set; }

        public int Score { get; set; }

        public int ModifiedScore { get; set; }

        public int? Pauses { get; set; } = new int?(0);

        public EHMD HMD { get; set; } = EHMD.Unk;

        public PassState.EState PassState { get; set; } = API.PassState.EState.Allowed;

        public long UnixTimeSet { get; set; }

        private string? ReplayLink { get; set; }

        public Player BeatLeaderPlayer { get; set; }

        private string PlayerName { get; set; }

        private string Modifiers { get; set; }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override void AfterViewCreation() => Instance = this;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal void SetModalInfo(
          ApiMapLeaderboardContentStruct p_Score,
          ApiCustomDataStruct p_CustomData,
          List<string> p_BannedModifiers,
          Player p_BeatLeaderPlayer)
        {
            PlayerName = p_Score.Name;
            BadCuts = (int)p_Score.BadCuts;
            MissedNotes = (int)p_Score.MissedNotes;
            ModifiedScore = (int)p_Score.ModifiedScore;
            Pauses = (p_Score.ScoreStatistic != null) ? (int)p_Score.ScoreStatistic?.PauseCount! : -1;
            m_BannedModifiers = p_BannedModifiers;
            HMD = (EHMD)p_Score.HMD;
            UnixTimeSet = long.Parse(p_Score.UnixTimeSet);
            ReplayLink = p_Score.ReplayURL;
            BeatLeaderPlayer = p_BeatLeaderPlayer;
            Modifiers = p_Score.Modifiers;
            Score = (int)p_Score.BaseScore;
            PassState = (PassState.EState)p_Score.State;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal void Show()
        {
            (m_ModalPlayerNameText).text = PlayerName;
            if (BadCuts == 0 && MissedNotes == 0)
            {
                m_ModalBadCutsText.color = Color.green;
                m_ModalBadCutsText.text = "Full Combo";
                m_ModalMissedNotesText.text = string.Empty;
            }
            else
            {
                m_ModalBadCutsText.text = $"<color=#adadad>Bad cuts : <color=#b50000>{BadCuts}</color>";
                m_ModalMissedNotesText.text = $"<color=#adadad>Missed Notes : <color=#b50000>{MissedNotes}</color>";
            }
            m_ModalModifiedScoreText.gameObject.SetActive(ModifiedScore != Score);
            m_ModalModifiedScoreText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}> Modified score : </color>{ModifiedScore}";
            TextMeshProUGUI modalPauseCount = m_ModalPauseCount;

            if (Pauses == -1)
            {
                modalPauseCount.text = $"Pauses : <color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>?";
            }
            else
            {
                modalPauseCount.text = $"<color=#{ColorUtility.ToHtmlStringRGB(Color.white)}>Pauses : <color=#{((Pauses == 0) ? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red))}>{Pauses}";
            }
            foreach (string bannedModifier in m_BannedModifiers)
            {
                if (Modifiers.Contains(bannedModifier))
                    PassState = API.PassState.EState.Denied;
            }

            m_ModalPassState.text = $"Pass state : <color=#{API.PassState.GetColorFromPassState(PassState)}>{PassState}</color>";
            m_ModalHMD.text = $"Set on : {HMD}";

            DateTime l_Time = CP_SDK.Misc.Time.FromUnixTime(CP_SDK.Misc.Time.UnixTimeNow() - UnixTimeSet);

            long l_FormattedYears = l_Time.Year - 1970;
            string l_Years = (l_FormattedYears != 0) ? $"{l_FormattedYears} Years" : string.Empty;

            long l_FormattedMonth = l_Time.Month - 1;
            string l_Months = (l_FormattedMonth != 0) ? $"{l_FormattedMonth} Months" : string.Empty;

            long l_FormattedDays = l_Time.Day - 1;
            string l_Days = (l_FormattedDays != 0) ? $"{l_FormattedDays} Days" : string.Empty;

            bool l_IsMajorObjects0 = string.IsNullOrEmpty(l_Years) && string.IsNullOrEmpty(l_Months) && string.IsNullOrEmpty(l_Days);

            string l_Hours = (l_Time.Hour != 0 && l_IsMajorObjects0) ? $"{l_Time.Hour} Hours" : string.Empty;
            string l_Minutes = (l_Time.Minute != 0 && l_IsMajorObjects0) ? $"{l_Time.Minute} Minutes" : string.Empty;
            string l_Seconds = (l_Time.Second != 0 && l_IsMajorObjects0) ? $"{l_Time.Second} Seconds" : string.Empty;

            m_ModalTimeSet.text = $"{l_Years} {l_Months} {l_Days} {l_Hours} {l_Minutes} {l_Seconds}ago";

            (m_ModalReplay).interactable = ReplayLink != null && GuildSaberModule.s_BeatLeaderInstalled && !LeaderboardScoreList.StartedReplayFromMod;

            if (!GuildSaberModule.s_BeatLeaderInstalled)
                m_ModalReplay.SetButtonText("BeatLeader not installed");
            m_InfoModal.Show(true, true, null);
        }

        [UIAction("CloseModal")]
        public void Hide() => m_InfoModal.Hide(true, null);

        public void HideNoAnim() => m_InfoModal.Hide(false, null);

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIAction("StartReplay")]
        private async void StartReplay()
        {
            m_ModalReplay.interactable = false;
            Replay l_Replay = await BeatLeaderReplayDownloader.GetReplay(ReplayLink);

            if (l_Replay == null) return;

            ReplayerSettings l_Settings;
            ReplayerShortcuts l_Shortcuts;
            ReplayLaunchData l_Data;
            ReplayerLauncher l_ReplayLauncher;

            l_Shortcuts = new ReplayerShortcuts
            {
                PauseHotkey = KeyCode.Space,
                HideCursorHotkey = KeyCode.C,
                RewindBackwardHotkey = KeyCode.LeftArrow,
                RewindForwardHotkey = KeyCode.RightArrow,
            };

            l_Settings = new ReplayerSettings
            {
                ShowHead = false,
                LoadPlayerEnvironment = false,
                AutoHideUI = false,
                ShowWatermark = true,
                ShowLeftSaber = true,
                ShowRightSaber = true,
                CameraFOV = 95,
                MinCameraFOV = 50,
                MaxCameraFOV = 120,
                VRCameraView = "CenterView",
                Shortcuts = l_Shortcuts,
            };

            l_Data = new ReplayLaunchData();
            l_Data.Init(l_Replay, l_Settings, BeatLeaderPlayer);

            m_InfoModal.Hide(false, null);

            l_ReplayLauncher = Resources.FindObjectsOfTypeAll<ReplayerLauncher>().First();
            await l_ReplayLauncher.StartReplayAsync(l_Data);

            ReplayerLauncher.ReplayWasFinishedEvent += new Action<ReplayLaunchData>(OnReplayFinished);

            LeaderboardScoreList.ReplayLaunchData = l_Data;
            LeaderboardScoreList.StartedReplayFromMod = true;

        }

        private void OnReplayFinished(ReplayLaunchData p_LaunchData)
        {
            typeof(ReplayerMenuLoader).GetMethod("HandleReplayWasFinished", BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object)((IEnumerable<ReplayerMenuLoader>)Resources.FindObjectsOfTypeAll<ReplayerMenuLoader>()).First<ReplayerMenuLoader>(), new object[2]
            { null, LeaderboardScoreList.ReplayLaunchData });
            LeaderboardScoreList.StartedReplayFromMod = false;
            ReplayerLauncher.ReplayWasFinishedEvent -= new Action<ReplayLaunchData>(OnReplayFinished);
        }
    }
}
