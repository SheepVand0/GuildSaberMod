using System;
using Zenject;

namespace GuildSaber
{
    public class Events : IInitializable
    {

        internal static bool m_EventsEnabled = true;

        public bool m_LeaderboardViewHasBeenLoaded;
        public static Events? Instance { get; private set; } = new Events();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void Initialize()
        {
            Instance = this;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////


        public static event Action e_OnLeaderboardPostLoad;

        public event Action<int> e_OnGuildSelected;

        public event Action<string> e_OnPointsTypeChange;

        public static event Action<bool> e_OnLeaderboardShown;

        public static event Action e_OnLeaderboardHide;

        public event Action<ELeaderboardScope> e_OnScopeSelected;

        public static event Action e_OnReload;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void SelectGuild(int p_GuildId)
        {
            if (!m_EventsEnabled)
            {
                return;
            }

            e_OnGuildSelected?.Invoke(p_GuildId);
        }

        public void SelectPointsTypes(string p_PointsNames)
        {
            if (!m_EventsEnabled)
            {
                return;
            }

            e_OnPointsTypeChange?.Invoke(p_PointsNames);
        }

        public void EventOnLeaderboardPostLoad()
        {
            if (!m_EventsEnabled)
            {
                return;
            }

            if (m_LeaderboardViewHasBeenLoaded)
            {
                return;
            }
            e_OnLeaderboardPostLoad?.Invoke();
            m_LeaderboardViewHasBeenLoaded = true;
        }

        public static void OnLeaderboardShow(bool p_FirstActivation)
        {
            if (!m_EventsEnabled)
            {
                return;
            }

            e_OnLeaderboardShown?.Invoke(p_FirstActivation);
        }

        public static void OnLeaderboardIsHide()
        {
            if (!m_EventsEnabled)
            {
                return;
            }

            e_OnLeaderboardHide?.Invoke();
        }

        public void SelectScope(ELeaderboardScope p_Scope)
        {
            if (!m_EventsEnabled)
            {
                return;
            }
            e_OnScopeSelected?.Invoke(p_Scope);
        }

        public static void InvokeOnReload()
        {
            e_OnReload?.Invoke();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
