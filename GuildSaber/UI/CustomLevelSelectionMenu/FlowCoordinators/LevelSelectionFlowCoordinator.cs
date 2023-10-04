using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers;
using CP_SDK.UI;
using GuildSaber.API;
using GuildSaber.UI.Others;
using BeatSaberMarkupLanguage;
using GuildSaber.UI.FlowCoordinator;
using HMUI;
using UnityEngine;
using System.Linq;
using LDViewController = GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.GuildSaberLeaderboardViewController;

namespace GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators
{
    internal class LevelsFlowCoordinator : CustomFlowCoordinator
    {
        internal static LevelsFlowCoordinator Instance;

        LevelSelectionViewController m_ViewController = BeatSaberUI.CreateViewController<LevelSelectionViewController>();

        protected override string Title => "Play";

        public void ShowWithLevels(int p_GuildId, ApiCategory p_Category)
        {
            m_ViewController.SetLevels(p_GuildId, p_Category);
            Present();
        }

        protected override (ViewController, ViewController, ViewController) GetUIImplementation()
        {
            var l_Result = Resources.FindObjectsOfTypeAll<GameplaySetupViewController>();
            GameplaySetupViewController l_GameplaySetupViewController = null;
            if (l_Result.Length == 0)
            {
                l_GameplaySetupViewController = new GameObject(nameof(GameplaySetupViewController)).AddComponent<GameplaySetupViewController>();
            }
            else
            {
                l_GameplaySetupViewController = l_Result.First();
            }
            //l_GameplaySetupViewController.transform.SetParent(EmptyViewController.Instance.transform);
            l_GameplaySetupViewController.__Activate(false, true);
            l_GameplaySetupViewController.Setup(true, true, true, false, PlayerSettingsPanelController.PlayerSettingsPanelLayout.All);

            if (LDViewController.Instance == null)
                LDViewController.Instance = BeatSaberUI.CreateViewController<LDViewController>();

            return (m_ViewController, l_GameplaySetupViewController, LDViewController.Instance);
        }
    }
}
