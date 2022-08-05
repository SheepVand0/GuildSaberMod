using HarmonyLib;
using System;

namespace GuildSaberProfile.Harmony
{
    [HarmonyPatch(typeof(LevelCollectionNavigationController),
        nameof(LevelCollectionNavigationController.HandleLevelCollectionViewControllerDidSelectLevel),
        new Type[] { typeof(LevelCollectionViewController), typeof(IPreviewBeatmapLevel) } )]
    class BeatmapPatch
    {
        public static void Prefix(LevelCollectionNavigationController __instance, ref LevelCollectionViewController viewController, ref IPreviewBeatmapLevel level)
        {
            Events.m_Instance.SelectBeatmap(__instance.selectedDifficultyBeatmap);
        }
    }
}
