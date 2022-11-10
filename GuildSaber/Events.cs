using BS_Utils.Utilities;
using UnityEngine;
using GuildSaber.Time;
using UnityEngine.SceneManagement;
using GuildSaber.Utils;
using GuildSaber.UI.Card;
using Zenject;
using System;
using IPA.Loader;

namespace GuildSaber;

public class Events : IInitializable
{
    public static Events m_Instance { get; private set; } = null;

    public bool m_LeaderboardViewHasBeenLoaded = false;

    internal static bool m_EventsEnabled = true;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public delegate void OnLeaderboardViewPostLoad();
    public static event OnLeaderboardViewPostLoad e_OnLeaderboardPostLoad = null;

    public delegate void OnGuildSelected(int p_GuildId);
    public event OnGuildSelected e_OnGuildSelected = null;

    public delegate void OnPointsTypeChange(string p_PointsName);
    public event OnPointsTypeChange e_OnPointsTypeChange = null;

    public delegate void OnLeaderboardShown(bool p_FirstActivation);
    public static event OnLeaderboardShown e_OnLeaderboardShown = null;

    public delegate void OnLeaderboardHide();
    public static event OnLeaderboardHide e_OnLeaderboardHide = null;

    public delegate void OnScopeSelected(ELeaderboardScope p_Scope);
    public event OnScopeSelected e_OnScopeSelected = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public void Initialize()
    {
        m_Instance = this;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public void SelectGuild(int p_GuildId)
    {
        if (!m_EventsEnabled)
            return;

        e_OnGuildSelected?.Invoke(p_GuildId);
    }

    public void SelectPointsTypes(string p_PointsNames)
    {
        if (!m_EventsEnabled)
            return;

        e_OnPointsTypeChange?.Invoke(p_PointsNames);
    }

    public void EventOnPostLoadLeaderboard()
    {
        if (!m_EventsEnabled)
            return;

        if (m_LeaderboardViewHasBeenLoaded) return;
        e_OnLeaderboardPostLoad?.Invoke();
        m_LeaderboardViewHasBeenLoaded = true;
    }

    public static void OnLeaderboardShow(bool p_FirstActivation)
    {
        if (!m_EventsEnabled)
            return;

        e_OnLeaderboardShown?.Invoke(p_FirstActivation);
    }

    public static void OnLeaderboardIsHide()
    {
        if (!m_EventsEnabled)
            return;

        e_OnLeaderboardHide?.Invoke();
    }

    public void SelectScope(ELeaderboardScope p_Scope)
    {
        if (!m_EventsEnabled)
            return;
        e_OnScopeSelected?.Invoke(p_Scope);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
}

