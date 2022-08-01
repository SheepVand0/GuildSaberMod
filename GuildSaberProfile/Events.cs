using BS_Utils.Utilities;
using Zenject;
using UnityEngine;
using GuildSaberProfile.Time;
using UnityEngine.SceneManagement;
using GuildSaberProfile.Utils;

namespace GuildSaberProfile;

class Events : IInitializable
{

    public void Initialize()
    {
        BSEvents.lateMenuSceneLoadedFresh += OnMenuSceneLoadedFresh;
        BSEvents.difficultySelected += BeatmapDifficultySelected;
    }

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

    private void BeatmapDifficultySelected(StandardLevelDetailViewController p_LevelDetailViewController, IDifficultyBeatmap p_Beatmap)
    {
        string l_Hash = GSBeatmapUtils.DifficultyBeatmapToString(p_Beatmap);

    }
}

