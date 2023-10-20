using UnityEngine;
using GuildSaber.UI.FlowCoordinator;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators
{
    internal class MapResultsFlowCoordinator : CustomFlowCoordinator
    {
        protected override string Title => string.Empty;

        protected override bool ShowBackButton => false;

        public static MapResultsFlowCoordinator Instance;

        protected ResultsViewController m_ViewController = Resources.FindObjectsOfTypeAll<ResultsViewController>().First();

        protected override void OnCreation()
        {
            Instance = this;
        }

        protected override (ViewController, ViewController, ViewController) GetUIImplementation()
        {
            return (m_ViewController, null, null);
        }

        public async void ShowWithData(StandardLevelScenesTransitionSetupDataSO p_SceneData, LevelCompletionResults p_Data, IDifficultyBeatmap p_Beatmap, PlayerData p_PlayerData)
        {

            m_ViewController.Init(p_Data,
                await p_Beatmap.GetBeatmapDataAsync(p_SceneData.environmentInfo, p_PlayerData.playerSpecificSettings),
                p_Beatmap, CustomLevelSelectionMenuReferences.IsInPractice, false);
            Present();
        }
    }
}
