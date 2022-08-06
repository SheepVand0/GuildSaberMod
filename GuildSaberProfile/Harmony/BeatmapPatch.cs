using HarmonyLib;
using System;
using GuildSaberProfile.UI.GuildSaber.Leaderboard;

namespace GuildSaberProfile.Harmony
{
    [HarmonyPatch(typeof(LevelCollectionNavigationController),
        nameof(LevelCollectionNavigationController.HandleLevelDetailViewControllerDidChangeDifficultyBeatmap),
        new Type[] { typeof(StandardLevelDetailViewController), typeof(IDifficultyBeatmap) } )]
    class BeatmapDifficultyPatch
    {
        private static void Postfix(ref StandardLevelDetailViewController viewController, ref IDifficultyBeatmap beatmap)
        {
            Plugin.Log.Info("Selected roger");
            Events.SelectBeatmap(beatmap);
        }
    }

    [HarmonyPatch(typeof(LevelCollectionNavigationController),nameof(LevelCollectionNavigationController.HandleLevelCollectionViewControllerDidSelectLevel),
        new Type[] { typeof(LevelCollectionViewController), typeof(IPreviewBeatmapLevel) })]
    class BeatmapSelectPatch
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="viewController"></param>
        /// <param name="level"></param>
        /// <param name="____levelDetailViewController"></param>
        private static void Postfix(LevelCollectionViewController viewController,IPreviewBeatmapLevel level, ref StandardLevelDetailViewController ____levelDetailViewController)
        {
            Events.SelectBeatmap(____levelDetailViewController.selectedDifficultyBeatmap);
        }
    }
}
