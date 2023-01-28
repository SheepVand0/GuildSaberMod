﻿using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using GuildSaber.Logger;
using GuildSaber.UI.Leaderboard.Components;
using GuildSaber.UI.Leaderboard.Managers;
using GuildSaber.Utils;
using LeaderboardCore.Interfaces;
using Polyglot;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace GuildSaber.UI.Leaderboard;

[HotReload(RelativePathToLayout = @"LeaderboardView.bsml")]
[ViewDefinition("GuildSaber.UI.GuildSaber.View.LeaderboardView.bsml")]
internal class GuildSaberLeaderboardView : BSMLAutomaticViewController
{

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public static GuildSaberLeaderboardView m_Instance;

    public static GameObject s_GameObjectReference;
    public ScopeSelector m_ScopeSelector;
    [UIComponent("PageDownImage")] public readonly Button m_PageDownImage = null;
    [UIComponent("PageUpImage")] public readonly Button m_PageUpImage = null;
    [UIComponent("WorldSelection")] private readonly VerticalLayoutGroup m_ScopeSelectionLayout = null;

    [UIComponent("ScoreParamsLayout")] internal readonly VerticalLayoutGroup m_ScoreParamsLayout = null;

    private LeaderboardScoreList m_ScoresList;

    public static bool Initialized { get; private set; }

    public bool ChangingLeaderboard { get; } = false;

    public void OnLeaderboardSet(IDifficultyBeatmap difficultyBeatmap)
    {
        GSLogger.Instance.Log("Setting", IPA.Logging.Logger.LogLevel.InfoUp);
        if (m_ScoresList == null) return;
        m_ScoresList.OnLeaderboardSet(difficultyBeatmap);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Post Parse
    /// </summary>
    [UIAction("#post-parse")]
    private async void PostParse()
    {
        m_Instance = this;
        s_GameObjectReference = gameObject;

        //Plugin.Log.Info("Creating GuildSaber leaderboard view");
        m_ScoresList = CustomUIComponent.CreateItem<LeaderboardScoreList>(m_ScoreParamsLayout.transform, true, true);
        m_ScopeSelector = CustomUIComponent.CreateItem<ScopeSelector>(m_ScopeSelectionLayout.transform, true, true);

        await WaitUtils.Wait(() => GuildSaberLeaderboardPanel.PanelInstance != null, 10);

        Initialized = true;
    }

    /// <summary>
    ///     When page up button pressed
    /// </summary>
    [UIAction("PageUp")]
    private void PageUp() { m_ScoresList.PageUp(); }

    /// <summary>
    ///     When page down button pressed
    /// </summary>
    [UIAction("PageDown")]
    private void PageDown() { m_ScoresList.PageDown(); }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Set leaderboard mode
    /// </summary>
    /// <param name="p_Mode"></param>
    public void SetLeaderboardViewMode(ELeaderboardViewMode p_Mode)
    {
        try {
            m_ScoresList.m_NotRankedText.richText = true;
            m_ScoresList.m_ScoreList.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores);
            m_ScopeSelectionLayout.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores || p_Mode == ELeaderboardViewMode.UnPassed && m_ScoresList.m_SelectedScope != ELeaderboardScope.Global);
            m_ScoresList.m_NotRankedText.gameObject.SetActive(p_Mode == ELeaderboardViewMode.UnPassed || p_Mode == ELeaderboardViewMode.NotRanked);
            m_ScoresList.m_ErrorText.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Error);
            m_ScoresList.m_LoadingLayout.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Loading);
            m_PageDownImage.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores);
            m_PageUpImage.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores);
            switch (p_Mode) {
                case ELeaderboardViewMode.NotRanked:
                    m_ScoresList.m_NotRankedText.SetText("<i>Map not ranked");
                    m_ScoresList.m_NotRankedText.color = Color.red;
                    LeaderboardHeaderManager.ChangeText(Localization.Get("TITLE_HIGHSCORES"));
                    break;
                case ELeaderboardViewMode.UnPassed:
                    LeaderboardHeaderManager.ChangeText(Localization.Get("TITLE_HIGHSCORES"));
                    m_ScoresList.m_NotRankedText.SetText("<i>Map un passed");
                    m_ScoresList.m_NotRankedText.color = Color.yellow;
                    break;
                case ELeaderboardViewMode.Scores:
                    break;
                case ELeaderboardViewMode.Loading:
                    break;
                case ELeaderboardViewMode.Error:
                    break;
                default:
                    GSLogger.Instance.Error(new Exception("Not Valid"), nameof(GuildSaberLeaderboardView), nameof(SetLeaderboardViewMode));
                    break;
            }
        }
        catch (Exception l_E) { GSLogger.Instance.Error(l_E, nameof(GuildSaberLeaderboardView), nameof(SetLeaderboardViewMode)); }
    }
}

public enum ELeaderboardViewMode
{
    Scores,
    NotRanked,
    UnPassed,
    Loading,
    Error
}