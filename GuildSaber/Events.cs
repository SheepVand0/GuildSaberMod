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
    public static Events m_Instance { get; private set; }

    public static bool m_IsGuildSaberLeaderboardShown = false;

    public bool m_LeaderboardViewHasBeenLoaded = false;

    #region Events
    public delegate void OnLeaderboardViewPostLoad();
    public event OnLeaderboardViewPostLoad e_OnLeaderboardPostLoad;

    public delegate void OnGuildSelected(int p_GuildId);
    public event OnGuildSelected e_OnGuildSelected;

    public delegate void OnPointsTypeChange(string p_PointsName);
    public event OnPointsTypeChange e_OnPointsTypeChange;

    public delegate void OnLeaderboardShown(bool p_FirstActivation);
    public static event OnLeaderboardShown e_OnLeaderboardShown;

    public delegate void OnLeaderboardHide();
    public static event OnLeaderboardHide e_OnLeaderboardHide;

    public delegate void OnScopeSelected(ELeaderboardScope p_Scope);
    public event OnScopeSelected e_OnScopeSelected;
    #endregion

    #region Setup
    public void Initialize()
    {
        BSEvents.lateMenuSceneLoadedFresh += OnMenuSceneLoadedFresh;

        Plugin.Log.Info("Defining Events Manager");
        m_Instance = this;
    }
    #endregion

    #region Events Invoker
    public void SelectGuild(int p_GuildId)
    {
        e_OnGuildSelected?.Invoke(p_GuildId);
    }

    public void SelectPointsTypes(string p_PointsNames)
    {
        e_OnPointsTypeChange?.Invoke(p_PointsNames);
    }

    public void EventOnPostLoadLeaderboard()
    {
        if (m_LeaderboardViewHasBeenLoaded) return;
        e_OnLeaderboardPostLoad?.Invoke();
        m_LeaderboardViewHasBeenLoaded = true;
    }

    public static void OnLeaderboardShow(bool p_FirstActivation)
    {
        m_IsGuildSaberLeaderboardShown = true;
        e_OnLeaderboardShown?.Invoke(p_FirstActivation);
    }

    public static void OnLeaderboardIsHide()
    {
        m_IsGuildSaberLeaderboardShown = false;
        e_OnLeaderboardHide?.Invoke();
    }

    public void SelectScope(ELeaderboardScope p_Scope)
    {
        e_OnScopeSelected?.Invoke(p_Scope);
    }
    #endregion

    #region Defaults Events Handlers
    private void OnMenuSceneLoadedFresh(ScenesTransitionSetupDataSO p_Obj)
    {
        try
        {
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        catch (Exception l_E)
        {
            Plugin.Log.Error($"Exception caught when creating card : {l_E.Message}");
            Plugin.Log.Error("------------------------------------------------------------");
            Plugin.Log.Error($"Stack trace : {l_E.StackTrace}");
            Plugin.Log.Error("------------------------------------------------------------");
            Plugin.Log.Error($"Inner : {l_E.InnerException}");
            Plugin.Log.Error("------------------------------------------------------------");
            Plugin.Log.Error($"TargetSite : {l_E.TargetSite}");
        }
    }
    private static void OnSceneChanged(Scene p_CurrentScene, Scene p_NextScene)
    {
        if (p_NextScene == null) return;

        Plugin.CurrentSceneName = p_NextScene.name;
        PlayerCardUI.m_Instance.UpdateCardVisibility();
        PlayerCardUI.m_Instance.UpdateCardPosition();
    }
    #endregion
}

