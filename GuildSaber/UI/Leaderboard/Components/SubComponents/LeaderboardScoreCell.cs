using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatLeader.Models;
using BeatLeader.Replayer;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaber.API;
using GuildSaber.Utils;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace GuildSaber.UI.Leaderboard.Components.SubComponents;

internal class LeaderboardScoreCell
{

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private static readonly Color s_Blue = new Color(0f, 0.8f, 1f, 0.8f);
    [UIComponent("ModalBadCuts")] private readonly TextMeshProUGUI m_ModalBadCutsText = null;
    [UIComponent("ModalHMD")] private readonly TextMeshProUGUI m_ModalHMD = null;
    [UIComponent("ModalMissedNotes")] private readonly TextMeshProUGUI m_ModalMissedNotesText = null;
    [UIComponent("ModalModifiedScore")] private readonly TextMeshProUGUI m_ModalModifiedScoreText = null;
    [UIComponent("ModalPassState")] private readonly TextMeshProUGUI m_ModalPassState = null;
    [UIComponent("ModalPauseCount")] private readonly TextMeshProUGUI m_ModalPauseCount = null;

    [UIComponent("ModalPlayerName")] private readonly TextMeshProUGUI m_ModalPlayerNameText = null;
    [UIComponent("ModalReplay")] internal readonly Button m_ModalReplay = null;
    [UIComponent("ModalTimeSet")] private readonly TextMeshProUGUI m_ModalTimeSet = null;

    private List<string> m_BannedModifiers = new List<string>();
    [UIValue("AccText")] private CustomText m_CAcc;
    [UIValue("Modifiers")] private CustomText m_CModifiers;
    [UIValue("Name")] private CustomText m_CPlayerName;
    [UIValue("Points")] private CustomText m_CPoints;

    [UIValue("Rank")] private CustomText m_CRank;
    [UIValue("ScoreText")] private CustomText m_CScore;
    [UIComponent("Elems")] public HorizontalLayoutGroup m_ElemsLayout = null;
    [UIComponent("Info")] public ModalView m_InfoModal = null;
    [UIComponent("Interactable")] public Button m_Interactable = null;

    public static float ScaleFactor
    {
        get => 1.2f;
    }
    private float InteractableScaleY
    {
        get => 6.5f * ScaleFactor * 0.9f;
    }
    public static float ScoreFontSize
    {
        get => 2.5f * ScaleFactor;
    }
    private float LeaderWidth
    {
        get => 90 * ScaleFactor + 7 * 1.4f;
    }

    public bool HasBeenParsed { get; private set; }

    private float BasePoints { get; set; }
    public string Rank { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public string Points { get; set; } = string.Empty;
    public string Score { get; set; } = string.Empty;
    public string Acc { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public Player BeatLeaderPlayer { get; set; }
    public string Modifiers { get; set; } = string.Empty;

    public int BadCuts { get; set; }
    public int MissedNotes { get; set; }
    public int ModifiedScore { get; set; }
    public int? Pauses { get; set; } = 0;
    public EHMD HMD { get; set; } = EHMD.Unk;
    public PassState.EState PassState { get; set; } = API.PassState.EState.Allowed;
    public long UnixTimeSet { get; set; }
    private string? ReplayLink { get; set; }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Update cell on leaderboard refresh
    /// </summary>
    /// <param name="p_Rank">Player rank in this map</param>
    /// <param name="p_Name">player name</param>
    /// <param name="p_Points">Points</param>
    /// <param name="p_PointsName">Selected points type name</param>
    /// <param name="p_Score">Score</param>
    /// <param name="p_Acc">Acc</param>
    /// <param name="p_Id">Player GuildSaber Id</param>
    /// <param name="p_Modifiers">Modifiers</param>
    internal void Init(int p_Rank, string p_Name, float p_Points, string p_PointsName, int p_Score, float p_Acc, string p_Id, string p_Modifiers)
    {
        Rank = $"{p_Rank}";
        PlayerName = p_Name;
        BasePoints = p_Points;
        Points = !float.IsNaN(p_Points) ? $"{p_Points:0.00} {p_PointsName}" : string.Empty;
        Score = p_Score.ToString();
        Acc = $"{p_Acc:00.00}%";
        Id = p_Id;
        Modifiers = GuildSaberUtils.GetPlayerNameToFit(p_Modifiers, 4);
        Show();
    }

    /// <summary>
    ///     Set Score Modal Information
    /// </summary>
    /// <param name="p_BadCuts"></param>
    /// <param name="p_MissedNotes"></param>
    /// <param name="p_Pauses"></param>
    /// <param name="p_ModifiedScore"></param>
    /// <param name="p_BannedModifiers"></param>
    /// <param name="p_PassState"></param>
    /// <param name="p_Hmd"></param>
    internal void SetModalInfo(int p_BadCuts, int p_MissedNotes, int? p_Pauses, int p_ModifiedScore, List<string> p_BannedModifiers, PassState.EState p_PassState, EHMD p_Hmd, long p_UnixTimeSet, Player p_BeatLeaderPlayer, string? p_ReplayLink)
    {
        BadCuts = p_BadCuts;
        MissedNotes = p_MissedNotes;
        ModifiedScore = p_ModifiedScore;
        Pauses = p_Pauses;
        PassState = p_PassState;
        m_BannedModifiers = p_BannedModifiers;
        HMD = p_Hmd;
        UnixTimeSet = p_UnixTimeSet;
        ReplayLink = p_ReplayLink;
        BeatLeaderPlayer = p_BeatLeaderPlayer;

        // ReSharper disable once InvertIf
        if (Points == string.Empty && p_PassState is not API.PassState.EState.Allowed) {
            //m_CPoints.SetEnableRichText(true);
            Points = $"<color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>⬛";
            m_CPoints.SetText(Points);
        }
    }

    /// <summary>
    ///     PostParse
    /// </summary>
    [UIAction("#post-parse")]
    private void _PostParse()
    {
        List<ItemParam> l_CurrentParams = new List<ItemParam>
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

        l_CurrentParams[1] = new ItemParam("AnchorPosX", 3f + l_OffsetFromRank + ScaleFactor * 0.7f);
        l_CurrentParams[7] = new ItemParam("EnableRichText", false);

        m_CPlayerName = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

        l_CurrentParams[1] = new ItemParam("AnchorPosX", 34f + l_OffsetFromRank + l_NameOffset + ScaleFactor * 1.02f);
        l_CurrentParams[4] = new ItemParam("Color", l_StartColor);
        l_CurrentParams[7] = new ItemParam("EnableRichText", true);

        m_CPoints = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

        l_CurrentParams[1] = new ItemParam("AnchorPosX", -40f + l_OffsetFromRank + l_NameOffset + ScaleFactor * 1.04f);
        l_CurrentParams[2] = new ItemParam("Alignment", TextAlignmentOptions.Right);
        l_CurrentParams[4] = new ItemParam("Color", Color.white);
        // ReSharper disable once StringLiteralTypo
        l_CurrentParams[6] = new ItemParam("RichText", "<mspace=0.5em>");

        m_CScore = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

        l_CurrentParams[0] = new ItemParam("FontSize", ScoreFontSize / ScaleFactor);
        l_CurrentParams[1] = new ItemParam("AnchorPosX", -32.5f + l_OffsetFromRank + l_NameOffset + ScaleFactor * 1.06f);
        l_CurrentParams[4] = new ItemParam("Color", Color.grey);
        l_CurrentParams[5] = new ItemParam("Italic", false);
        l_CurrentParams[6] = new ItemParam("RichText", string.Empty);

        m_CModifiers = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

        l_CurrentParams[0] = new ItemParam("FontSize", ScoreFontSize);
        l_CurrentParams[1] = new ItemParam("AnchorPosX", -24f + l_OffsetFromRank + l_NameOffset + ScaleFactor * 1.06f);
        l_CurrentParams[4] = new ItemParam("Color", l_StartColor);

        m_CAcc = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

        HasBeenParsed = true;

        Events.Instance.e_OnGuildSelected += OnGuildSelected;

        Reset();
    }

    /// <summary>
    ///     Apply all values on texts
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
    ///     Reset cell colors, texts, and hide button
    /// </summary>
    internal async void Reset()
    {
        await WaitUtils.Wait(() => LeaderboardScoreList.Instance.Initialized, 10);
        await WaitUtils.Wait(() => HasBeenParsed, 10);
        //await WaitUtils.Wait(() => m_ElemsLayout.gameObject.activeInHierarchy, 10, p_CodeLine: 207);

        SetEmpty();
        Color l_White = Color.white;
        m_CRank.SetColor(l_White);
        m_CPlayerName.SetColor(l_White);
        m_CScore.SetColor(l_White);

        m_Interactable.gameObject.SetActive(false);
    }

    /// <summary>
    ///     Set current cell to player to highlight it in blue
    /// </summary>
    internal void SetCellToCurrentPlayer()
    {
        m_CRank.SetColor(s_Blue);
        m_CPlayerName.SetColor(s_Blue);
        m_CScore.SetColor(s_Blue);
    }

    /// <summary>
    ///     Show it when have a score
    /// </summary>
    internal async void Show()
    {
        await WaitUtils.Wait(() => LeaderboardScoreList.Instance.Initialized, 10);

        SetTexts();

        m_Interactable.gameObject.SetActive(true);
    }
    /// <summary>
    ///     Set texts to empty
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
        if (BadCuts == 0 && MissedNotes == 0) {
            m_ModalBadCutsText.color = Color.green;
            m_ModalBadCutsText.text = "Full Combo";
            m_ModalMissedNotesText.text = string.Empty;
        }
        else {
            // ReSharper disable once StringLiteralTypo
            m_ModalBadCutsText.text = $"<color=#adadad>Bad cuts : <color=#b50000>{BadCuts}</color>";
            // ReSharper disable once StringLiteralTypo
            m_ModalMissedNotesText.text = $"<color=#adadad>Missed Notes : <color=#b50000>{MissedNotes}</color>";
        }

        m_ModalModifiedScoreText.gameObject.SetActive(ModifiedScore != int.Parse(Score));
        m_ModalModifiedScoreText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}> Modified score : </color>{ModifiedScore}";
        m_ModalPauseCount.text = Pauses != null ? $"<color=#{ColorUtility.ToHtmlStringRGB(Color.white)}>Pauses : <color=#{(Pauses == 0 ? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red))}>{Pauses}" : "Pauses : ?";

        foreach (string? l_BannedModifier in m_BannedModifiers) {
            if (!Modifiers.Contains(l_BannedModifier)) continue;

            PassState = API.PassState.EState.Denied;
        }

        m_ModalPassState.text = "Pass state : " + $"<color=#{API.PassState.GetColorFromPassState(PassState)}>{PassState}</color>";
        m_ModalHMD.text = $"Set on : {HMD}";
        DateTime l_Time = CP_SDK.Misc.Time.FromUnixTime(CP_SDK.Misc.Time.UnixTimeNow() - UnixTimeSet);

        long l_FormattedYears = l_Time.Year - 1970;
        string l_Years = l_FormattedYears != 0 ? $"{l_FormattedYears} Years" : string.Empty;

        long l_FormattedMonth = l_Time.Month - 1;
        string l_Months = l_FormattedMonth != 0 ? $"{l_FormattedMonth} Months" : string.Empty;

        long l_FormattedDays = l_Time.Day - 1;
        string l_Days = l_FormattedDays != 0 ? $"{l_FormattedDays} Days" : string.Empty;

        bool l_IsMajorObjects0 = string.IsNullOrEmpty(l_Years) && string.IsNullOrEmpty(l_Months) && string.IsNullOrEmpty(l_Days);

        string l_Hours = l_Time.Hour != 0 && l_IsMajorObjects0 ? $"{l_Time.Hour} Hours" : string.Empty;
        string l_Minutes = l_Time.Minute != 0 && l_IsMajorObjects0 ? $"{l_Time.Minute} Minutes" : string.Empty;
        string l_Seconds = l_Time.Second != 0 && l_IsMajorObjects0 ? $"{l_Time.Second} Seconds" : string.Empty;
        m_ModalTimeSet.text = $"{l_Years} {l_Months} {l_Days} {l_Hours} {l_Minutes} {l_Seconds}ago";

        m_ModalReplay.interactable = ReplayLink != null && GuildSaberModule.s_BeatLeaderInstalled;

        if (!GuildSaberModule.s_BeatLeaderInstalled) m_ModalReplay.SetButtonText("BeatLeader not installed");

    }

    /// <summary>
    ///     On Close Modal button pressed
    /// </summary>
    [UIAction("CloseModal")]
    private void CloseModal() { m_InfoModal.Hide(true); }

    [UIAction("StartReplay")]
    private async void StartReplay()
    {
        m_ModalReplay.interactable = false;
        Replay? l_Replay = await BeatLeaderReplayDownloader.GetReplay(ReplayLink);
        if (l_Replay is null) {
            return;
        }

        var l_Settings = new ReplayerSettings();
        l_Settings.ShowHead = false;
        l_Settings.LoadPlayerEnvironment = false;
        l_Settings.ShowUI = true;
        l_Settings.ShowWatermark = true;
        l_Settings.ForceUseReplayerCamera = false;
        l_Settings.ShowLeftSaber = true;
        l_Settings.ShowRightSaber = true;
        l_Settings.CameraFOV = 95;
        l_Settings.MinFOV = 50;
        l_Settings.MaxFOV = 120;
        l_Settings.VRCameraPose = "CenterView";
        l_Settings.FPFCCameraPose = "CenterView";

        var l_Shortcuts = new ReplayerShortcuts();
        l_Shortcuts.PauseHotkey = KeyCode.Space;
        l_Shortcuts.HideCursorHotkey = KeyCode.A;
        l_Shortcuts.RewindBackwardHotkey = KeyCode.LeftArrow;
        l_Shortcuts.RewindForwardHotkey = KeyCode.RightArrow;
        l_Shortcuts.HideUIHotkey = KeyCode.H;

        l_Settings.Shortcuts = l_Shortcuts;

        var l_Data = new ReplayLaunchData(l_Replay, BeatLeaderPlayer, null, null, l_Settings);

        m_InfoModal.Hide(false);

        ReplayerLauncher? l_ReplayLauncher = Resources.FindObjectsOfTypeAll<ReplayerLauncher>().First();

        await l_ReplayLauncher.StartReplayAsync(l_Data);
        ReplayerLauncher.ReplayWasFinishedEvent +=
            OnReplayFinished;

        LeaderboardScoreList.s_ReplayLaunchData = l_Data;

        LeaderboardScoreList.s_StartedReplayFromMod = true;
    }

    private void OnReplayFinished(ReplayLaunchData p_LaunchData)
    {
        MethodInfo? l_Method = typeof(ReplayerMenuLoader).GetMethod("HandleReplayWasFinished", BindingFlags.Instance | BindingFlags.NonPublic);
        l_Method.Invoke(Resources.FindObjectsOfTypeAll<ReplayerMenuLoader>().First(), new object[]
        {
            null, LeaderboardScoreList.s_ReplayLaunchData
        });



        ReplayerLauncher.ReplayWasFinishedEvent -=
            OnReplayFinished;
    }

    /*private static void Init() {
        var types = _assembly.GetTypes();
        var _installersMethods = new List<MethodInfo>();

        foreach (var item in types) {
            if (item.IsSubclassOf(typeof(Installer))) {
                var method = item.GetMethod(nameof(
                    Installer.InstallBindings), ReflectionUtils.DefaultFlags);
                _installersMethods.Add(method);
            }
        }

        var _installatorsSilencer = new(_installersMethods, false);
        GSLogger.Instance.Log($"Successfully patched {_installersMethods.Count} ScoreSaber installators!", IPA.Logging.Logger.LogLevel.InfoUp);
    }*/
}
