﻿using BeatSaberMarkupLanguage.Attributes;
using GuildSaber.API;
using GuildSaber.UI.Leaderboard.Components.SubComponents;
using GuildSaber.Utils.DiscordColor;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildSaber.UI.Leaderboard.Components;

internal class CustomLevelStatsView : CustomUIComponent
{
    protected override string ViewResourceName
    {
        get => "GuildSaber.UI.Leaderboard.Components.Views.CustomLevelStatsView.bsml";
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [UIComponent("MainButton")] private readonly Button m_MainButton = null;
    [UIComponent("MainHorizontal")] private Button m_MainHorizontal = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [UIComponent("Rank")]
    private readonly TextMeshProUGUI m_CRank = null;
    [UIComponent("Name")]
    private readonly TextMeshProUGUI m_CName = null;
    [UIComponent("PassState")]
    private readonly TextMeshProUGUI m_CPassState = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [UIComponent("InfoModal")] private readonly ModalView m_InfoModal = null;

    [UIComponent("BadCuts")] private readonly TextMeshProUGUI m_CBadCuts = null;
    [UIComponent("MissedNotes")] private readonly TextMeshProUGUI m_CMissedNotes = null;
    [UIComponent("Pauses")] private readonly TextMeshProUGUI m_CPauses = null;
    [UIComponent("HMD")] private readonly TextMeshProUGUI m_CHMD = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     After view creation
    /// </summary>
    protected override void AfterViewCreation()
    {
        m_CRank.fontSize = LeaderboardScoreCell.ScoreFontSize;
        m_CName.fontStyle = FontStyles.Italic;
        m_CName.fontSize = LeaderboardScoreCell.ScoreFontSize;
        m_CPassState.richText = true;
        m_CPassState.fontSize = LeaderboardScoreCell.ScoreFontSize;
        m_MainButton.gameObject.GetComponentsInChildren<ImageView>()[1].gameObject.SetActive(false);
        new ButtonBinder().AddBinding(m_MainButton, ShowInfoModal);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Set Stats View Informations
    /// </summary>
    /// <param name="p_Rank"></param>
    /// <param name="p_Name"></param>
    /// <param name="p_State"></param>
    public void Init(int p_Rank, string p_Name, PassState.EState p_State)
    {
        m_MainButton.gameObject.SetActive(true);
        m_CRank.SetText($"#{p_Rank}");
        m_CName.SetText(p_Name);
        m_CPassState.SetText($"State : <color=#{ColorUtility.ToHtmlStringRGBA(PassState.GetColorFromPassState(p_State))}>{p_State}</color>");
    }

    /// <summary>
    ///     Set View Modal Info
    /// </summary>
    /// <param name="p_BadCuts"></param>
    /// <param name="p_MissedNotes"></param>
    /// <param name="p_Pauses"></param>
    /// <param name="p_Hmd"></param>
#nullable enable
    public void SetModalInfo(int p_BadCuts, int p_MissedNotes, int? p_Pauses, EHMD p_Hmd)
    {
        if (p_BadCuts == 0 && p_MissedNotes == 0) {
            m_CBadCuts.color = UnityEngine.Color.green;
            m_CBadCuts.text = "Full Combo";
            m_CMissedNotes.text = string.Empty;
        }
        else {
            m_CBadCuts.text = $"<color=#adadad>Bad cuts : <color=#b50000>{p_BadCuts}</color>";
            m_CMissedNotes.text = $"<color=#adadad>Missed Notes : <color=#b50000>{p_MissedNotes}</color>";
        }
        if (p_Pauses != null)
            m_CPauses.text = $"<color=#{ColorUtility.ToHtmlStringRGB(UnityEngine.Color.white)}>Pauses : <color=#{(p_Pauses == 0 ? ColorUtility.ToHtmlStringRGB(UnityEngine.Color.green) : ColorUtility.ToHtmlStringRGB(UnityEngine.Color.red))}>{p_Pauses}";
        else
            m_CPauses.text = "Pauses : ?";
        m_CHMD.text = $"Set on : {p_Hmd}";
    }
#nullable disable

    ////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Reset informations and hide
    /// </summary>
    public void Clear()
    {
        m_CRank.SetText(string.Empty);
        m_CName.SetText(string.Empty);
        m_CPassState.SetText(string.Empty);
        m_MainButton.gameObject.SetActive(false);
    }

    ////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Show Modal
    /// </summary>
    [UIAction("ShowInfoModal")]
    public void ShowInfoModal() { m_InfoModal.Show(true, true); }

    /// <summary>
    ///     Close Info Modal
    /// </summary>
    [UIAction("CloseModal")]
    private void CloseModal() { m_InfoModal.Hide(true); }
}