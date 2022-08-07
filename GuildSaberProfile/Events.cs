using BS_Utils.Utilities;
using UnityEngine;
using GuildSaberProfile.Time;
using UnityEngine.SceneManagement;
using GuildSaberProfile.Utils;
using Zenject;

namespace GuildSaberProfile;

public class Events : IInitializable
{
    public static Events m_Instance { get; private set; }

    public static bool m_IsGuildSaberLeaderboardShown = false;

    public bool m_LeaderboardViewHasBeenLoaded = false;

    #region Events
    public delegate void OnLeaderboardViewPostLoad();
    public event OnLeaderboardViewPostLoad e_OnLeaderboardPostLoad;

    public delegate void OnGuildSelected(string p_Guild);
    public event OnGuildSelected e_OnGuildSelected;

    public delegate void OnPointsTypeChange(string p_PointsName);
    public event OnPointsTypeChange e_OnPointsTypeChange;

    public delegate void OnLeaderboardShown(bool p_FirstActivation);
    public static event OnLeaderboardShown e_OnLeaderboardShown;

    public delegate void OnLeaderboardHide();
    public static event OnLeaderboardHide e_OnLeaderboardHide;
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
    public void SelectGuild(string p_Guild)
    {
        e_OnGuildSelected?.Invoke(p_Guild);
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
    #endregion

    #region Defaults Events Handlers
    private static void OnSceneChanged(Scene p_CurrentScene, Scene p_NextScene)
    {
        if (p_NextScene == null) return;

        Plugin.CurrentSceneName = p_NextScene.name;
        Plugin.PlayerCard.UpdateCardVisibility();
        Plugin.PlayerCard.UpdateCardPosition();
        Plugin.PlayerCard.CardViewController.UpdateToggleCardHandleVisibility();
    }

    private async void OnMenuSceneLoadedFresh(ScenesTransitionSetupDataSO p_Obj)
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        if (Plugin.m_TimeManager == null)
        {
            Plugin.m_TimeManager = new GameObject("CardPlayTime").AddComponent<TimeManager>();
            Object.DontDestroyOnLoad(Plugin.m_TimeManager);
        }

        if (Plugin.PlayerCard != null)
            await Plugin.DestroyCard();
        Plugin.CreateCard();
    }
    #endregion
}

