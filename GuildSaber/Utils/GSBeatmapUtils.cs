using UnityEngine;
using SongCore.Utilities;
using System.Linq;
using BeatSaberPlus.SDK.Game;
using System;

namespace GuildSaber.Utils;

internal class GSBeatmapUtils
{
    public static string DifficultyBeatmapToHash(IDifficultyBeatmap difficultyBeatmap)
    {
        if (difficultyBeatmap.level is CustomPreviewBeatmapLevel p_CustomLevel) {
            string l_Hash = Hashing.GetCustomLevelHash(p_CustomLevel);
            return l_Hash;
        }
        return null;
    }

    public static int DifficultyToNumber(BeatmapDifficulty p_Difficulty)
    {
        return p_Difficulty switch
        {
            BeatmapDifficulty.Easy => 1,
            BeatmapDifficulty.Normal => 3,
            BeatmapDifficulty.Hard => 5,
            BeatmapDifficulty.Expert => 7,
            BeatmapDifficulty.ExpertPlus => 9,
            _ => 0
        };
    }

    public static void PlaySong(IDifficultyBeatmap p_Beatmap, OverrideEnvironmentSettings p_OverrideEnvironmentSettings, ColorScheme p_ColorScheme, 
        GameplayModifiers p_Modifiers, PlayerSpecificSettings p_PlayerSpecificSettings, PracticeSettings p_PracticeSettings, string p_BackButtonText = "Menu", Action<LevelScenesTransitionSetupDataSO,LevelCompletionResults> p_OnMapFinished = null)
    {
        MenuTransitionsHelper l_MenuTransitionHelpers = Resources.FindObjectsOfTypeAll<MenuTransitionsHelper>().First();
        if (l_MenuTransitionHelpers == null) return;

        //Levels.PlaySong

        l_MenuTransitionHelpers.StartStandardLevel("Solo", p_Beatmap, p_Beatmap.level, 
            p_OverrideEnvironmentSettings, p_ColorScheme, p_Modifiers, p_PlayerSpecificSettings, p_PracticeSettings, p_BackButtonText, false, false
            ,null,p_OnMapFinished, null);
    }
}
